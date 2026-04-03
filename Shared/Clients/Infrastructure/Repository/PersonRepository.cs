using Clients.API.Domain.Entities;
using Clients.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clients.API.Infrastructure.Repository
{
	public class PersonRepository(ClientsDbContext context) : IPersonRepository
	{
		private readonly ClientsDbContext _context = context;
		public async Task<Person?> GetByClientIdAsync(Guid clientId)
		{
			return await _context.Persons.FirstOrDefaultAsync(p => p.ClientId == clientId);
		}
		public async Task<Person> CreateAsync(Person person)
		{
			_context.Persons.Add(person);
			await _context.SaveChangesAsync();
			return person;
		}
		public async Task UpdateAsync(Person person)
		{
			_context.Persons.Update(person);
			await _context.SaveChangesAsync();
		}
	}
}
