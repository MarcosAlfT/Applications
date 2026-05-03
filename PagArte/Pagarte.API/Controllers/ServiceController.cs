using Microsoft.AspNetCore.Mvc;
using Pagarte.API.GrpcClients;
using Infrastructure.Responses;

namespace Pagarte.API.Controllers
{
	[ApiController]
	[Route("api/services")]
	public class ServiceController(ServiceCatalogGrpcClient grpcClient) : BaseController
	{
		private readonly ServiceCatalogGrpcClient _grpcClient = grpcClient;

		[HttpGet]
		public async Task<IActionResult> GetAsync([FromQuery] string? category = null)
		{
			var result = await _grpcClient.GetServicesAsync(category);
			return Ok(ApiResponse<object>.CreateSuccess(result.Services));
		}

		[HttpGet("{serviceId}")]
		public async Task<IActionResult> GetByIdAsync(string serviceId)
		{
			var result = await _grpcClient.GetServiceAsync(serviceId);
			if (!result.Found)
				return Ok(ApiResponse.CreateFailure("Service not found."));

			return Ok(ApiResponse<object>.CreateSuccess(result.Service));
		}
	}
}
