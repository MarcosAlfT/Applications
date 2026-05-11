using Microsoft.EntityFrameworkCore;
using Pagarte.Services.Domain.Entities;
using Pagarte.Services.Interfaces;

namespace Pagarte.Services.Infrastructure.Repository
{
	public class PaymentQuoteRepository(PagarteDbContext context) : IPaymentQuoteRepository
	{
		private readonly PagarteDbContext _context = context;

		public async Task<PaymentQuote?> GetByIdAsync(Guid id)
			=> await _context.PaymentQuotes
				.Include(q => q.Details)
				.Include(q => q.Service)
				.FirstOrDefaultAsync(q => q.Id == id);

		public async Task<PaymentQuote> CreateAsync(PaymentQuote quote)
		{
			_context.PaymentQuotes.Add(quote);
			await _context.SaveChangesAsync();
			return quote;
		}

		public async Task UpdateAsync(PaymentQuote quote)
		{
			_context.PaymentQuotes.Update(quote);
			await _context.SaveChangesAsync();
		}
	}
}
