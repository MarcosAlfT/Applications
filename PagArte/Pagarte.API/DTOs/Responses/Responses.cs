using Pagarte.API.Domain.Enums;

namespace Pagarte.API.DTOs.Responses
{
    public record CreditCardResponse(
        Guid Id,
        string CardHolderName,
        string Last4Digits,
        string CardType,
        int ExpiryMonth,
        int ExpiryYear,
        bool IsDefault,
        DateTime CreatedAt);

    public record PaymentResponse(
        Guid Id,
        string Reference,
        string Status,
        string Currency,
        decimal TotalAmount,
        DateTime CreatedAt,
        DateTime? ProcessedAt,
        string? ErrorMessage,
        IEnumerable<PaymentDetailResponse> Details);

    public record PaymentDetailResponse(
        string Type,
        string Description,
        decimal Amount,
        string Currency);

    public record ServiceResponse(
        Guid Id,
        string Name,
        string Description,
        string Category,
        decimal BaseAmount,
        string Currency);
}
