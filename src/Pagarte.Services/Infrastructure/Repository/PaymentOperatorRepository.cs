using Microsoft.EntityFrameworkCore;
using Pagarte.Services.Domain.Entities;
using Pagarte.Services.Domain.Enums;
using Pagarte.Services.Interfaces;

namespace Pagarte.Services.Infrastructure.Repository
{
	public class PaymentOperatorRepository(PagarteDbContext context) : IPaymentOperatorRepository
	{
		private readonly PagarteDbContext _context = context;

		public async Task<PaymentOperator?> GetActiveAsync(PaymentOperatorScope scope)
			=> await _context.PaymentOperators
				.Where(o => o.IsActive && o.Scope == scope)
				.OrderBy(o => o.Priority)
				.FirstOrDefaultAsync();

		public async Task<PaymentOperator?> GetByCodeAsync(string code)
			=> await _context.PaymentOperators
				.FirstOrDefaultAsync(o => o.Code == code);

		public async Task<PaymentOperator> CreateAsync(PaymentOperator paymentOperator)
		{
			_context.PaymentOperators.Add(paymentOperator);
			await _context.SaveChangesAsync();
			return paymentOperator;
		}
	}
}
