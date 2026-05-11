using Pagarte.Services.Domain.Entities;
using Pagarte.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Pagarte.Services.Infrastructure;

namespace Pagarte.Services.Infrastructure.Repository
{
	public class FeeConfigurationRepository(PagarteDbContext context) : IFeeConfigurationRepository
	{
		private readonly PagarteDbContext _context = context;

		public async Task<IEnumerable<FeeConfiguration>> GetActiveFeesAsync()
			=> await _context.FeeConfigurations
				.Where(f => f.EffectiveDate <= DateTime.UtcNow
					&& (f.EndDate == null || f.EndDate >= DateTime.UtcNow))
				.ToListAsync();
	}
}
