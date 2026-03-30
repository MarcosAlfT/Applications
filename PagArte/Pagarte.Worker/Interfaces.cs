using Pagarte.Worker.Domain;

namespace Pagarte.Worker
{
    public interface IProcessingLogRepository
    {
        Task<ProcessingLog> CreateAsync(ProcessingLog log);
        Task UpdateAsync(ProcessingLog log);
        Task<IEnumerable<ProcessingLog>> GetPendingRetriesAsync();
    }

    public interface IPaymentWorkerRepository
    {
        Task UpdateStatusAsync(Guid paymentId, string status,
            string? companyReference = null, string? errorMessage = null);
    }

    public interface IEmailSenderService
    {
        Task SendAsync(string to, string subject, string body, bool isHtml = true);
    }

    public interface ICompanyService
    {
        Task<CompanyPaymentResult> SendPaymentAsync(Guid companyId, decimal amount,
            string currency, string reference, string clientId);
    }

    public interface IDLocalWorkerService
    {
        Task<DLocalRefundResult> RefundAsync(string dLocalPaymentId,
            decimal amount, string currency, string reason);
    }

    public record CompanyPaymentResult(
        bool Success,
        string? CompanyReference,
        string? ErrorMessage);

    public record DLocalRefundResult(
        bool Success,
        string? RefundId,
        string? ErrorMessage);
}
