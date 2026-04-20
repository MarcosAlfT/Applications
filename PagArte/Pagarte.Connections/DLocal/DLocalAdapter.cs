using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Pagarte.Connections.DLocal
{
    public class DLocalAdapter(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<DLocalAdapter> logger) : IDLocalAdapter
    {
        private readonly HttpClient _httpClient =
            httpClientFactory.CreateClient("dlocal");
        private readonly string _apiKey =
            configuration["DLocal:ApiKey"]
            ?? throw new InvalidOperationException("DLocal:ApiKey not configured.");
        private readonly string _apiSecret =
            configuration["DLocal:ApiSecret"]
            ?? throw new InvalidOperationException("DLocal:ApiSecret not configured.");
        private readonly ILogger<DLocalAdapter> _logger = logger;

        public async Task<DLocalCardResult> RegisterCardAsync(
            string encryptedCardData, string cardHolderName)
        {
            try
            {
                var payload = new
                {
                    card = new
                    {
                        encrypted_data = encryptedCardData,
                        holder_name = cardHolderName
                    }
                };

                var response = await PostAsync("/secure_cards", payload);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("dLocal card registration failed: {Error}", error);
                    return new DLocalCardResult(false, null, null, null, 0, 0,
                        "Card registration failed.");
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                return new DLocalCardResult(
                    true,
                    result.GetProperty("card_id").GetString(),
                    result.GetProperty("last4").GetString(),
                    result.GetProperty("brand").GetString(),
                    result.GetProperty("expiration_month").GetInt32(),
                    result.GetProperty("expiration_year").GetInt32(),
                    null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering card with dLocal");
                return new DLocalCardResult(false, null, null, null, 0, 0, ex.Message);
            }
        }

        public async Task<DLocalChargeResult> ChargeAsync(
            string cardToken, decimal amount, string currency, string reference)
        {
            try
            {
                var payload = new
                {
                    amount,
                    currency,
                    payment_method_id = "CARD",
                    payment_method_flow = "DIRECT",
                    order_id = reference,
                    card = new { card_id = cardToken }
                };

                var response = await PostAsync("/payments", payload);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("dLocal charge failed: {Error}", error);
                    return new DLocalChargeResult(false, null, "Payment charge failed.");
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                return new DLocalChargeResult(
                    true,
                    result.GetProperty("id").GetString(),
                    null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error charging with dLocal");
                return new DLocalChargeResult(false, null, ex.Message);
            }
        }

        public async Task<DLocalRefundResult> RefundAsync(
            string dLocalPaymentId, decimal amount, string currency, string reason)
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

                var response = await PostAsync("/refunds", payload);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("dLocal refund failed: {Error}", error);
                    return new DLocalRefundResult(false, null, "Refund failed.");
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                return new DLocalRefundResult(
                    true,
                    result.GetProperty("id").GetString(),
                    null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refunding with dLocal");
                return new DLocalRefundResult(false, null, ex.Message);
            }
        }

        private async Task<HttpResponseMessage> PostAsync(string endpoint, object payload)
        {
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

            return await _httpClient.PostAsync(endpoint, content);
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
