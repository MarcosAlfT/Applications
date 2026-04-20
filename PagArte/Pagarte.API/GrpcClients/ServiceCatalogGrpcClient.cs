using Pagarte.Contracts;

namespace Pagarte.API.GrpcClients
{
	public class ServiceCatalogGrpcClient(
		ServiceCatalogService.ServiceCatalogServiceClient client)
	{
		private readonly ServiceCatalogService.ServiceCatalogServiceClient _client = client;

		public async Task<GetServicesResponse> GetServicesAsync(string? category = null)
			=> await _client.GetServicesAsync(
				new GetServicesRequest { Category = category ?? string.Empty });

		public async Task<GetServiceResponse> GetServiceAsync(string serviceId)
			=> await _client.GetServiceAsync(
				new GetServiceRequest { ServiceId = serviceId });
	}
}
