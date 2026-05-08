using Clients.API.Domain.Entities;

namespace Clients.API.Interfaces
{
	public interface IAddressRepository
	{
		Task<IEnumerable<Address>> GetByClientIdAsync(Guid clientId);
		Task<Address> CreateAsync(Address address);
		Task UpdateAsync(Address address);
		Task DeleteAsync(Guid clientId, Guid addressId);
	}
}
