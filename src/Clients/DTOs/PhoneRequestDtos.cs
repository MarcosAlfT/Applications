using Clients.API.Domain;

namespace Clients.API.DTOs
{
	public record CreatePhoneRequest(
		string Number,
		PhoneType Type,
		bool IsPrimary);

	public record UpdatePhoneRequest(
		string Number,
		PhoneType Type,
		bool IsPrimary);
}
