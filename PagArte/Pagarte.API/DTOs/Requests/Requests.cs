using Pagarte.API.Domain.Enums;

namespace Pagarte.API.DTOs.Requests
{
    public record RegisterCreditCardRequest(
        string CardNumber,
        string CardHolderName,
        string ExpiryMonth,
        string ExpiryYear,
        string Cvv,
        bool IsDefault);

    public record UpdateCreditCardRequest(
        string CardHolderName,
        bool IsDefault);

    public record ProcessPaymentRequest(
        Guid CreditCardId,
        Guid ServiceId,
        string Currency);
}
