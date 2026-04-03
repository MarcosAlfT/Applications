using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Pagarte.Worker.Services
{
    public class CompanyService(
        IHttpClientFactory httpClientFactory,
        ILogger<CompanyService> logger,
        ICompanyConfigRepository companyConfigRepository) : ICompanyService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly ILogger<CompanyService> _logger = logger;
        private readonly ICompanyConfigRepository _companyConfigRepository = companyConfigRepository;

        public async Task<CompanyPaymentResult> SendPaymentAsync(Guid companyId, decimal amount,
            string currency, string reference, string clientId)
        {
            try
            {
                var company = await _companyConfigRepository.GetByIdAsync(companyId);
                if (company == null)
                    return new CompanyPaymentResult(false, null, "Company not found.");

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("X-Api-Key", company.ApiKey);
                client.DefaultRequestHeaders.Add("X-Reference", reference);

                var payload = new
                {
                    amount,
                    currency,
                    reference,
                    client_id = clientId
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(company.ApiEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Company {CompanyId} rejected payment {Reference}: {Error}",
                        companyId, reference, error);
                    return new CompanyPaymentResult(false, null, $"Company rejected payment: {error}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var companyReference = result.GetProperty("reference").GetString();

                _logger.LogInformation("Company {CompanyId} accepted payment {Reference}",
                    companyId, reference);

                return new CompanyPaymentResult(true, companyReference, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending payment to company {CompanyId}", companyId);
                return new CompanyPaymentResult(false, null, ex.Message);
            }
        }
    }

    // Simple config model for company lookup in worker
    public record CompanyConfig(Guid Id, string Name, string ApiEndpoint, string ApiKey);

    public interface ICompanyConfigRepository
    {
        Task<CompanyConfig?> GetByIdAsync(Guid id);
    }
}
