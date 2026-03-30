using Clients.API.Domain.Entities;

namespace Clients.API.Interfaces
{
	public interface IPersonRepository
	{
		Task<Person?> GetByClientIdAsync(Guid clientId);
		Task<Person> CreateAsync(Person person);
		Task UpdateAsync(Person person);
	}
}
