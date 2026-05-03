using Grpc.Core;
using Pagarte.Contracts;
using Pagarte.Worker.Interfaces;

namespace Pagarte.Worker.GrpcServices
{
	public class ServiceCatalogGrpcService(
		IServiceRepository serviceRepository)
		: Pagarte.Contracts.ServiceCatalogService.ServiceCatalogServiceBase
	{
		private readonly IServiceRepository _serviceRepository = serviceRepository;

		public override async Task<GetServicesResponse> GetServices(
			GetServicesRequest request, ServerCallContext context)
		{
			var services = await _serviceRepository.GetAllActiveAsync(
				string.IsNullOrEmpty(request.Category) ? null : request.Category);
			var response = new GetServicesResponse();
			response.Services.AddRange(services.Select(MapService));
			return response;
		}

		public override async Task<GetServiceResponse> GetService(
			GetServiceRequest request, ServerCallContext context)
		{
			var service = await _serviceRepository.GetByIdAsync(Guid.Parse(request.ServiceId));
			if (service == null)
				return new GetServiceResponse { Found = false };

			return new GetServiceResponse { Found = true, Service = MapService(service) };
		}

		private static ServiceDto MapService(Domain.Entities.Service service) =>
			new()
			{
				Id = service.Id.ToString(),
				Name = service.Name,
				Description = service.Description,
				Category = service.Category,
				BaseAmount = (double)service.BaseAmount,
				Currency = service.Currency
			};
	}
}
