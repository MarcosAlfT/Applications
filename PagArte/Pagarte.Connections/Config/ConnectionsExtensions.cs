using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pagarte.Connections.Companies;
using Pagarte.Connections.DLocal;

namespace Pagarte.Connections.Config
{
    public static class ConnectionsExtensions
    {
        /// <summary>
        /// Registers all external connection adapters.
        /// Call from Pagarte.Worker and Pagarte.Engine Program.cs.
        /// </summary>
        public static IServiceCollection AddPagarteConnections(
            this IServiceCollection services, IConfiguration configuration)
        {
            // dLocal HttpClient with resilience
            services.AddHttpClient("dlocal", client =>
            {
                client.BaseAddress = new Uri(
                    configuration["DLocal:ApiUrl"]
                    ?? "https://sandbox.dlocal.com");
                client.Timeout = TimeSpan.FromSeconds(30);
            }).AddStandardResilienceHandler();

            // Company HttpClient with resilience
            services.AddHttpClient<CompanyAdapter>()
                .AddStandardResilienceHandler();

            // Register adapters
            services.AddScoped<IDLocalAdapter, DLocalAdapter>();
            services.AddScoped<ICompanyAdapter, CompanyAdapter>();

            return services;
        }
    }
}
