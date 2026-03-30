namespace Pagarte.API.Interfaces
{
    public interface IDLocalService
    {
        Task<DLocalCardResult> RegisterCardAsync(string cardNumber, string cardHolderName,
            string expiryMonth, string expiryYear, string cvv);
        Task<DLocalPaymentResult> ChargeAsync(string cardToken, decimal amount,
            string currency, string reference);
        Task<DLocalRefundResult> RefundAsync(string dLocalPaymentId, decimal amount,
            string currency, string reason);
    }

    public record DLocalCardResult(
        bool Success,
        string? CardToken,
        string? Last4Digits,
        string? CardType,
        string? ErrorMessage);

    public record DLocalPaymentResult(
        bool Success,
        string? DLocalPaymentId,
        string? ErrorMessage);

    public record DLocalRefundResult(
        bool Success,
        string? RefundId,
        string? ErrorMessage);
}
