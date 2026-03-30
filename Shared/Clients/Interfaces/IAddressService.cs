using Shared.Responses;
using Clients.API.DTOs.Responses;

namespace Clients.API.Interfaces
{
	public interface IAddressService
	{
		Task<ApiResponse<IEnumerable<AddressResponse>>> GetByClientIdAsync(Guid clientId);
		Task<ApiResponse<AddressResponse>> CreateAsync(Guid clientId, string street, string city, string state, string postalCode, string country, bool isPrimary);
		Task<ApiResponse> UpdateAsync(Guid clientId, Guid addressId, string street, string city, string state, string postalCode, string country, bool isPrimary);
		Task<ApiResponse> DeleteAsync(Guid clientId, Guid addressId);
	}
}