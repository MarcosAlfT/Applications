using Pagarte.Services.Domain.Entities;

namespace Pagarte.Services.Interfaces
{
	public interface ICreditCardRepository
	{
		Task<IEnumerable<CreditCard>> GetByClientIdAsync(string clientId);
		Task<CreditCard?> GetByIdAsync(Guid id);
		Task<CreditCard> CreateAsync(CreditCard card);
		Task UpdateAsync(CreditCard card);
		Task DeleteAsync(Guid id, string clientId);
	}
}
