using Pagarte.Services.Domain.Entities;

namespace Pagarte.Services.Interfaces
{
	public interface IPaymentQuoteRepository
	{
		Task<PaymentQuote?> GetByIdAsync(Guid id);
		Task<PaymentQuote> CreateAsync(PaymentQuote quote);
		Task UpdateAsync(PaymentQuote quote);
	}
}
