using Clients.API.Domain.Entities;

namespace Clients.API.Interfaces
{
	public interface IOrganizationRepository
	{
		Task<Organization?> GetByClientIdAsync(Guid clientId);
		Task<Organization> CreateAsync(Organization organization);
		Task UpdateAsync(Organization organization);
	}
}
