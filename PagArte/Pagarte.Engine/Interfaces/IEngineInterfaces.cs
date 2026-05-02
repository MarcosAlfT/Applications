namespace Pagarte.Engine.Interfaces
{
    public interface IPaymentStatusRepository
    {
        Task UpdateStatusAsync(Guid paymentId, string status,
            string? companyReference = null, string? errorMessage = null);
        Task IncrementRetryAsync(Guid paymentId);
<<<<<<< HEAD
        Task<(string? OperatorPaymentId, decimal Amount, string Currency, string ClientId)?>
=======
        Task<(string? DLocalPaymentId, decimal Amount, string Currency, string ClientId)?>
>>>>>>> origin/main
            GetPaymentInfoAsync(Guid paymentId);
    }

    public interface IEmailSenderService
    {
        Task SendAsync(string to, string subject, string body, bool isHtml = true);
    }
}
