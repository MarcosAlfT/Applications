using Pagarte.API.Domain.Entities;

namespace Pagarte.API.Interfaces
{
    public interface IServiceRepository
    {
        Task<IEnumerable<Service>> GetAllActiveAsync();
        Task<Service?> GetByIdAsync(Guid id);
        Task<IEnumerable<Service>> GetByCategoryAsync(string category);
    }
}
