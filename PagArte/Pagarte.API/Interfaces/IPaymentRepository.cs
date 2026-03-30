using Pagarte.API.Domain.Entities;

namespace Pagarte.API.Interfaces
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetByClientIdAsync(string clientId);
        Task<Payment?> GetByIdAsync(Guid id);
        Task<Payment?> GetByReferenceAsync(string reference);
        Task<Payment> CreateAsync(Payment payment);
        Task UpdateAsync(Payment payment);
        Task<IEnumerable<Payment>> GetPendingRefundsAsync();  // for retry logic
    }
}
