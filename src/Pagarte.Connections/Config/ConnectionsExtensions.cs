using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pagarte.Connections.Companies;
using Pagarte.Connections.PaymentOperators;

namespace Pagarte.Connections.Config
{
    public static class ConnectionsExtensions
    {
        /// <summary>
        /// Registers all external connection adapters.
        /// Call from Pagarte.Services and Pagarte.Engine Program.cs.
        /// </summary>
        public static IServiceCollection AddPagarteConnections(
            this IServiceCollection services, IConfiguration configuration)
        {
            // Company HttpClient with resilience
            services.AddHttpClient<CompanyAdapter>()
                .AddStandardResilienceHandler();

            services.AddHttpClient("payment-operator", client =>
            {
                var apiUrl = configuration["PaymentOperator:ApiUrl"];
                if (!string.IsNullOrWhiteSpace(apiUrl))
                {
                    client.BaseAddress = new Uri(apiUrl);
                }

                client.Timeout = TimeSpan.FromSeconds(30);
            }).AddStandardResilienceHandler();

            services.AddScoped<MockPaymentOperatorAdapter>();
            services.AddScoped<DLocalPaymentOperatorAdapter>();
            services.AddScoped<IPaymentOperatorAdapterFactory, PaymentOperatorAdapterFactory>();
            services.AddScoped<ICompanyAdapter, CompanyAdapter>();

            return services;
        }
    }
}
