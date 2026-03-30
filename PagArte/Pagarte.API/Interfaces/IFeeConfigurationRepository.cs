using Pagarte.API.Domain.Entities;

namespace Pagarte.API.Interfaces
{
    public interface IFeeConfigurationRepository
    {
        Task<IEnumerable<FeeConfiguration>> GetActiveFeesAsync();
    }
}
