namespace Pagarte.API.DTOs
{
	/// <summary>
	/// Card data RSA encrypted by Mobil app using dLocal public key.
	/// Pagarte.API never decrypts — forwards to Worker via RabbitMQ as-is.
	/// </summary>

	public record RegisterCreditCardRequest(
	string EncryptedCardData,
	string CardHolderName,
	bool IsDefault);

	public record UpdateCreditCardRequest(
		string CardHolderName,
		bool IsDefault);

	public record ProcessPaymentRequest(
		string CreditCardId,
		string ServiceId,
		string Currency);

}
