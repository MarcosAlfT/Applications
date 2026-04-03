using Pagarte.API.Domain.Entities;

namespace Pagarte.API.Interfaces
{
    public interface ICreditCardRepository
    {
        Task<IEnumerable<CreditCard>> GetByClientIdAsync(string clientId);
        Task<CreditCard?> GetByIdAsync(Guid id);
        Task<CreditCard> CreateAsync(CreditCard creditCard);
        Task UpdateAsync(CreditCard creditCard);
        Task DeleteAsync(Guid id, string clientId);
    }
}
