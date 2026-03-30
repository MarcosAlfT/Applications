using Pagarte.API.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Pagarte.API.Services
{
    public class DLocalService : IDLocalService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly ILogger<DLocalService> _logger;

        public DLocalService(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<DLocalService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("dlocal");
            _apiKey = configuration["DLocal:ApiKey"]
                ?? throw new InvalidOperationException("DLocal:ApiKey not configured.");
            _apiSecret = configuration["DLocal:ApiSecret"]
                ?? throw new InvalidOperationException("DLocal:ApiSecret not configured.");
            _logger = logger;
        }

        public async Task<DLocalCardResult> RegisterCardAsync(string cardNumber, string cardHolderName,
            string expiryMonth, string expiryYear, string cvv)
        {
            try
            {
                var payload = new
                {
                    card = new
                    {
                        number = cardNumber,
                        holder_name = cardHolderName,
                        expiration_month = expiryMonth,
                        expiration_year = expiryYear,
                        cvv
                    }
                };

                var response = await PostAsync("/secure_cards", payload);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("dLocal card registration failed: {Error}", error);
                    return new DLocalCardResult(false, null, null, null, "Card registration failed.");
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                return new DLocalCardResult(
                    true,
                    result.GetProperty("card_id").GetString(),
                    result.GetProperty("last4").GetString(),
                    result.GetProperty("brand").GetString(),
                    null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering card with dLocal");
                return new DLocalCardResult(false, null, null, null, ex.Message);
            }
        }

        public async Task<DLocalPaymentResult> ChargeAsync(string cardToken, decimal amount,
            string currency, string reference)
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
                    return new DLocalPaymentResult(false, null, "Payment charge failed.");
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);

                return new DLocalPaymentResult(
                    true,
                    result.GetProperty("id").GetString(),
                    null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error charging card with dLocal");
                return new DLocalPaymentResult(false, null, ex.Message);
            }
        }

        public async Task<DLocalRefundResult> RefundAsync(string dLocalPaymentId, decimal amount,
            string currency, string reason)
        {
            try
            {
                var payload = new
                {
                    payment_id = dLocalPaymentId,
                    amount,
                    currency,
                    notification_url = "",
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
                _logger.LogError(ex, "Error refunding payment with dLocal");
                return new DLocalRefundResult(false, null, ex.Message);
            }
        }

        private async Task<HttpResponseMessage> PostAsync(string endpoint, object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // dLocal uses HMAC signature for authentication
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
            // dLocal signature: HMAC-SHA256 of (apiKey + timestamp + body)
            var message = _apiKey + timestamp + body;
            using var hmac = new System.Security.Cryptography.HMACSHA256(
                Encoding.UTF8.GetBytes(_apiSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return Convert.ToHexString(hash).ToLower();
        }
    }
}
