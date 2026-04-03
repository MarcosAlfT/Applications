using Microsoft.EntityFrameworkCore;
using Clients.API.Domain.Entities;
using Clients.API.Interfaces;

namespace Clients.API.Infrastructure.Repository
{
	public class AddressRepository(ClientsDbContext context) : IAddressRepository
	{
		private readonly ClientsDbContext _context = context;
		public async Task<IEnumerable<Address>> GetByClientIdAsync(Guid clientId)
		{
			return await _context.Addresses.Where(a => a.ClientId == clientId).ToListAsync();
		}
		public async Task<Address> CreateAsync(Address address)
		{
			_context.Addresses.Add(address);
			await _context.SaveChangesAsync();
			return address;
		}
		public async Task UpdateAsync(Address address)
		{
			_context.Addresses.Update(address);
			await _context.SaveChangesAsync();
		}
		 public async Task DeleteAsync(Guid clientId, Guid addressId)
		{
			var address = await _context.Addresses.FirstOrDefaultAsync(a => a.ClientId == clientId && a.Id == addressId);
			if (address != null)
			{
				address.DeleteAddress();
				await _context.SaveChangesAsync();
			}
		}
	}
}
