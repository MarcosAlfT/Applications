using Pagarte.Contracts;
using Pagarte.Worker.Interfaces;
using Pagarte.Worker.Services;
using Grpc.Core;

namespace Pagarte.Worker.GrpcServices
{
	public class PaymentGrpcService(
			IPaymentRepository paymentRepository,
			PaymentEngineService paymentEngine,
			ILogger<PaymentGrpcService> logger)
			: Pagarte.Contracts.PaymentService.PaymentServiceBase
	{
		private readonly IPaymentRepository _paymentRepository = paymentRepository;
		private readonly PaymentEngineService _paymentEngine = paymentEngine;
		private readonly ILogger<PaymentGrpcService> _logger = logger;

		public override async Task<ProcessPaymentResponse> ProcessPayment(
			ProcessPaymentRequest request, ServerCallContext context)
		{
			_logger.LogInformation("Processing payment for client {ClientId}", request.ClientId);

			var result = await _paymentEngine.ProcessAsync(
				request.ClientId,
				Guid.Parse(request.CreditCardId),
				Guid.Parse(request.ServiceId),
				request.Currency);

			return new ProcessPaymentResponse
			{
				Success = result.Success,
				PaymentId = result.PaymentId?.ToString() ?? string.Empty,
				Reference = result.Reference ?? string.Empty,
				Status = result.Status ?? string.Empty,
				ErrorMessage = result.ErrorMessage ?? string.Empty
			};
		}

		public override async Task<GetPaymentResponse> GetPayment(
			GetPaymentRequest request, ServerCallContext context)
		{
			var payment = await _paymentRepository.GetByIdAsync(Guid.Parse(request.PaymentId));
			if (payment == null || payment.ClientId != request.ClientId)
				return new GetPaymentResponse { Found = false };

			return new GetPaymentResponse { Found = true, Payment = MapPayment(payment) };
		}

		public override async Task<GetPaymentHistoryResponse> GetPaymentHistory(
			GetPaymentHistoryRequest request, ServerCallContext context)
		{
			var payments = await _paymentRepository.GetByClientIdAsync(
				request.ClientId, request.Page, request.PageSize);
			var total = await _paymentRepository.GetCountByClientIdAsync(request.ClientId);

			var response = new GetPaymentHistoryResponse { Total = total };
			response.Payments.AddRange(payments.Select(MapPayment));
			return response;
		}

		private static PaymentDto MapPayment(Domain.Entities.Payment payment)
		{
			var dto = new PaymentDto
			{
				Id = payment.Id.ToString(),
				Reference = payment.Reference,
				Status = payment.Status.ToString(),
				Currency = payment.Currency,
				TotalAmount = (double)payment.Details.Sum(d => d.Amount),
				CreatedAt = payment.CreatedAt.ToString("O"),
				ProcessedAt = payment.ProcessedAt?.ToString("O") ?? string.Empty,
				ErrorMessage = payment.ErrorMessage ?? string.Empty,
				ServiceName = payment.Service?.Name ?? string.Empty
			};

			dto.Details.AddRange(payment.Details.Select(d => new PaymentDetailDto
			{
				Type = d.Type.ToString(),
				Description = d.Description,
				Amount = (double)d.Amount,
				Currency = d.Currency
			}));

			return dto;
		}
	}
}
