namespace Pagarte.Connections.DLocal
{
    public interface IDLocalAdapter
    {
        /// <summary>
        /// Registers card using RSA encrypted data from React Native app.
        /// dLocal decrypts with their private key — we never see plain card data.
        /// </summary>
        Task<DLocalCardResult> RegisterCardAsync(
            string encryptedCardData, string cardHolderName);

        Task<DLocalChargeResult> ChargeAsync(
            string cardToken, decimal amount, string currency, string reference);

        Task<DLocalRefundResult> RefundAsync(
            string dLocalPaymentId, decimal amount, string currency, string reason);
    }

    public record DLocalCardResult(
        bool Success,
        string? CardToken,
        string? Last4Digits,
        string? CardType,
        int ExpiryMonth,
        int ExpiryYear,
        string? ErrorMessage);

    public record DLocalChargeResult(
        bool Success,
        string? DLocalPaymentId,
        string? ErrorMessage);

    public record DLocalRefundResult(
        bool Success,
        string? RefundId,
        string? ErrorMessage);
}
