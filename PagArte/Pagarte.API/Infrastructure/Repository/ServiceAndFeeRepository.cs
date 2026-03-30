using Microsoft.EntityFrameworkCore;
using Pagarte.API.Domain.Entities;
using Pagarte.API.Interfaces;

namespace Pagarte.API.Infrastructure.Repository
{
    public class ServiceRepository(PagarteDbContext context) : IServiceRepository
    {
        private readonly PagarteDbContext _context = context;

        public async Task<IEnumerable<Service>> GetAllActiveAsync()
        {
            return await _context.Services
                .Include(s => s.Company)
                .ToListAsync();
        }

        public async Task<Service?> GetByIdAsync(Guid id)
        {
            return await _context.Services
                .Include(s => s.Company)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Service>> GetByCategoryAsync(string category)
        {
            return await _context.Services
                .Include(s => s.Company)
                .Where(s => s.Category == category)
                .ToListAsync();
        }
    }

    public class FeeConfigurationRepository(PagarteDbContext context) : IFeeConfigurationRepository
    {
        private readonly PagarteDbContext _context = context;

        public async Task<IEnumerable<FeeConfiguration>> GetActiveFeesAsync()
        {
            return await _context.FeeConfigurations
                .Where(f => f.EffectiveDate <= DateTime.UtcNow
                    && (f.EndDate == null || f.EndDate >= DateTime.UtcNow))
                .ToListAsync();
        }
    }
}
