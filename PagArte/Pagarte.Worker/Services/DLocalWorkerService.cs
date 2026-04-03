using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Pagarte.Worker.Services
{
    public class DLocalWorkerService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<DLocalWorkerService> logger) : IDLocalWorkerService
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("dlocal");
        private readonly string _apiKey = configuration["DLocal:ApiKey"]
            ?? throw new InvalidOperationException("DLocal:ApiKey not configured.");
        private readonly string _apiSecret = configuration["DLocal:ApiSecret"]
            ?? throw new InvalidOperationException("DLocal:ApiSecret not configured.");
        private readonly ILogger<DLocalWorkerService> _logger = logger;

        public async Task<DLocalRefundResult> RefundAsync(string dLocalPaymentId,
            decimal amount, string currency, string reason)
        {
            try
            {
                var payload = new
                {
                    payment_id = dLocalPaymentId,
                    amount,
                    currency,
                    description = reason
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                var signature = GenerateSignature(timestamp, json);

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-Date", timestamp);
                _httpClient.DefaultRequestHeaders.Add("X-Login", _apiKey);
                _httpClient.DefaultRequestHeaders.Add("X-Trans-Key", _apiSecret);
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("V2-HMAC-SHA256",
                        $"Signature={signature}");

                var response = await _httpClient.PostAsync("/refunds", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("dLocal refund failed for {PaymentId}: {Error}",
                        dLocalPaymentId, error);
                    return new DLocalRefundResult(false, null, "Refund failed.");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                return new DLocalRefundResult(
                    true,
                    result.GetProperty("id").GetString(),
                    null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund with dLocal for {PaymentId}",
                    dLocalPaymentId);
                return new DLocalRefundResult(false, null, ex.Message);
            }
        }

        private string GenerateSignature(string timestamp, string body)
        {
            var message = _apiKey + timestamp + body;
            using var hmac = new System.Security.Cryptography.HMACSHA256(
                Encoding.UTF8.GetBytes(_apiSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return Convert.ToHexString(hash).ToLower();
        }
    }
}
