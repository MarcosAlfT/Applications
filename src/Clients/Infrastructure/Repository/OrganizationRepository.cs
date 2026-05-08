using Microsoft.EntityFrameworkCore;
using Clients.API.Domain.Entities;
using Clients.API.Interfaces;

namespace Clients.API.Infrastructure.Repository
{
	public class OrganizationRepository(ClientsDbContext context) : IOrganizationRepository
	{
		private readonly ClientsDbContext _context = context;
		public async Task<Organization?> GetByClientIdAsync(Guid clientId)
		{
			return await _context.Organizations.FirstOrDefaultAsync(o => o.ClientId == clientId);
		}
		public async Task<Organization> CreateAsync(Organization organization)
		{
			_context.Organizations.Add(organization);
			await _context.SaveChangesAsync();
			return organization;
		}
		public async Task UpdateAsync(Organization organization)
		{
			_context.Organizations.Update(organization);
			await _context.SaveChangesAsync();
		}
	}
}