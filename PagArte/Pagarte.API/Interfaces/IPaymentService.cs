using Pagarte.API.DTOs.Requests;
using Pagarte.API.DTOs.Responses;
using Shared.Responses;

namespace Pagarte.API.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse<IEnumerable<PaymentResponse>>> GetByClientIdAsync(string clientId);
        Task<ApiResponse<PaymentResponse>> GetByIdAsync(Guid id, string clientId);
        Task<ApiResponse<PaymentResponse>> ProcessPaymentAsync(string clientId, ProcessPaymentRequest request);
    }
}
