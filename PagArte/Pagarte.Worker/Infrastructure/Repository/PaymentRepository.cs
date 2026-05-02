<<<<<<< HEAD
using Pagarte.Worker.Domain.Entities;
=======
﻿using Pagarte.Worker.Domain.Entities;
>>>>>>> origin/main
using Pagarte.Worker.Interfaces;
using Microsoft.EntityFrameworkCore;
using Pagarte.Worker.Domain.Enums;
namespace Pagarte.Worker.Infrastructure.Repository
{
	public class PaymentRepository(PagarteDbContext context) : IPaymentRepository
	{
		private readonly PagarteDbContext _context = context;

		public async Task<Payment?> GetByIdAsync(Guid id)
			=> await _context.Payments
				.Include(p => p.Details)
				.Include(p => p.Service)
				.Include(p => p.CreditCard)
				.FirstOrDefaultAsync(p => p.Id == id);

		public async Task<IEnumerable<Payment>> GetByClientIdAsync(
			string clientId, int page, int pageSize)
			=> await _context.Payments
				.Include(p => p.Details)
				.Include(p => p.Service)
				.Where(p => p.ClientId == clientId)
				.OrderByDescending(p => p.CreatedAt)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

		public async Task<int> GetCountByClientIdAsync(string clientId)
			=> await _context.Payments.CountAsync(p => p.ClientId == clientId);

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
			=> await _context.Payments
				.Where(p => p.Status == TransactionStatus.Refunding
					&& p.RetryCount < 3
					&& p.NextRetryAt <= DateTime.UtcNow)
				.ToListAsync();
	}
}
