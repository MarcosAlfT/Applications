using Pagarte.API.DTOs.Requests;
using Pagarte.API.DTOs.Responses;
using Shared.Responses;

namespace Pagarte.API.Interfaces
{
    public interface ICreditCardService
    {
        Task<ApiResponse<IEnumerable<CreditCardResponse>>> GetByClientIdAsync(string clientId);
        Task<ApiResponse<CreditCardResponse>> RegisterAsync(string clientId, RegisterCreditCardRequest request);
        Task<ApiResponse> UpdateAsync(string clientId, Guid cardId, UpdateCreditCardRequest request);
        Task<ApiResponse> DeleteAsync(string clientId, Guid cardId);
    }
}
