using Microsoft.EntityFrameworkCore;
using Pagarte.API.Domain.Entities;
using Pagarte.API.Domain.Enums;
using Pagarte.API.Interfaces;

namespace Pagarte.API.Infrastructure.Repository
{
    public class PaymentRepository(PagarteDbContext context) : IPaymentRepository
    {
        private readonly PagarteDbContext _context = context;

        public async Task<IEnumerable<Payment>> GetByClientIdAsync(string clientId)
        {
            return await _context.Payments
                .Include(p => p.Details)
                .Include(p => p.Service)
                .Where(p => p.ClientId == clientId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _context.Payments
                .Include(p => p.Details)
                .Include(p => p.Service)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payment?> GetByReferenceAsync(string reference)
        {
            return await _context.Payments
                .Include(p => p.Details)
                .FirstOrDefaultAsync(p => p.Reference == reference);
        }

        public async Task<Payment> CreateAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Payment>> GetPendingRefundsAsync()
        {
            return await _context.Payments
                .Where(p => p.Status == TransactionStatus.Refunding
                    && p.RetryCount < 3
                    && p.NextRetryAt <= DateTime.UtcNow)
                .ToListAsync();
        }
    }
}
