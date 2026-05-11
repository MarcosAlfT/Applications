using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagarte.API.GrpcClients;
using Pagarte.API.DTOs;
using Infrastructure.Responses;

namespace Pagarte.API.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/payment")]
	public class PaymentController(PaymentGrpcClient grpcClient) : BaseController
	{
		private readonly PaymentGrpcClient _grpcClient = grpcClient;

		[HttpGet]
		public async Task<IActionResult> GetHistoryAsync(
			[FromQuery] int page = 1, [FromQuery] int pageSize = 20)
		{
			var validation = ValidateClientId();
			if (validation != null) return validation;

			var result = await _grpcClient.GetPaymentHistoryAsync(
				GetClientId()!, page, pageSize);

			return Ok(ApiResponse<object>.CreateSuccess(new
			{
				result.Payments,
				result.Total,
				Page = page,
				PageSize = pageSize
			}));
		}

		[HttpGet("{paymentId}")]
		public async Task<IActionResult> GetByIdAsync(string paymentId)
		{
			var validation = ValidateClientId();
			if (validation != null) return validation;

			var result = await _grpcClient.GetPaymentAsync(paymentId, GetClientId()!);
			if (!result.Found)
				return Ok(ApiResponse.CreateFailure("Payment not found."));

			return Ok(ApiResponse<object>.CreateSuccess(result.Payment));
		}

		[HttpPost("quote")]
		public async Task<IActionResult> CreateQuoteAsync(
			[FromBody] CreatePaymentQuoteRequest request)
		{
			var validation = ValidateClientId();
			if (validation != null) return validation;

			var result = await _grpcClient.CreatePaymentQuoteAsync(
				GetClientId()!, request.ServiceId, request.Currency);

			if (!result.Success)
				return Ok(ApiResponse.CreateFailure(result.ErrorMessage));

			return Ok(ApiResponse<object>.CreateSuccess(result.Quote,
				"Payment quote created successfully."));
		}

		[HttpPost("confirm")]
		public async Task<IActionResult> ConfirmAsync(
			[FromBody] ConfirmPaymentRequest request)
		{
			var validation = ValidateClientId();
			if (validation != null) return validation;

			var result = await _grpcClient.ConfirmPaymentAsync(
				GetClientId()!, request.QuoteId, request.CreditCardId);

			if (!result.Success)
				return Ok(ApiResponse.CreateFailure(result.ErrorMessage));

			return Ok(ApiResponse<object>.CreateSuccess(new
			{
				result.PaymentId,
				result.Reference,
				result.Status
			}, "Payment confirmed successfully."));
		}
	}
}
