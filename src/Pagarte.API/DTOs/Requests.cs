namespace Pagarte.API.DTOs
{
	public record RegisterCreditCardRequest(
		string CardNumber,
		string Cvv,
		string CardHolderName,
		int ExpiryMonth,
		int ExpiryYear,
		bool IsDefault);

	public record UpdateCreditCardRequest(
		string CardHolderName,
		bool IsDefault);

	public record CreatePaymentQuoteRequest(
		string ServiceId,
		string Currency);

	public record ConfirmPaymentRequest(
		string QuoteId,
		string CreditCardId);
}
