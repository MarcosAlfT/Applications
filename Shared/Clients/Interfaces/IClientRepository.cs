using Clients.API.Domain.Entities;

namespace Clients.API.Interfaces
{
	public interface IClientRepository
	{
		Task<Client?> GetByUserIdAsync(string userId);
		Task<Client?> GetByClientIdAsync(Guid id);
		Task<IEnumerable<Client>> GetAllAsync();
		Task<Client> CreateAsync(Client client);
		Task UpdateAsync(Client client);
		Task DeleteAsync(Guid id);
	}
}
