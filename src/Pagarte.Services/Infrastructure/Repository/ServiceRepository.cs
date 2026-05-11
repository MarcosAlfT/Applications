using Pagarte.Services.Domain.Entities;
using Pagarte.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Pagarte.Services.Infrastructure;

namespace Pagarte.Services.Infrastructure.Repository
{
	public class ServiceRepository(PagarteDbContext context) : IServiceRepository
	{
		private readonly PagarteDbContext _context = context;

		public async Task<IEnumerable<Service>> GetAllActiveAsync(string? category = null)
		{
			var query = _context.Services.Include(s => s.Company).AsQueryable();
			if (!string.IsNullOrEmpty(category))
				query = query.Where(s => s.Category == category);
			return await query.ToListAsync();
		}

		public async Task<Service?> GetByIdAsync(Guid id)
			=> await _context.Services
				.Include(s => s.Company)
				.FirstOrDefaultAsync(s => s.Id == id);
	}
}