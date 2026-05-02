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
        /// Call from Pagarte.Worker and Pagarte.Engine Program.cs.
        /// </summary>
        public static IServiceCollection AddPagarteConnections(
            this IServiceCollection services, IConfiguration configuration)
        {
            var paymentOperatorProvider = configuration["PaymentOperator:Provider"];

            if (string.IsNullOrWhiteSpace(paymentOperatorProvider))
            {
                throw new InvalidOperationException("PaymentOperator:Provider is not configured.");
            }

            // Company HttpClient with resilience
            services.AddHttpClient<CompanyAdapter>()
                .AddStandardResilienceHandler();

            if (paymentOperatorProvider.Equals("Mock", StringComparison.OrdinalIgnoreCase))
            {
                services.AddScoped<IPaymentOperatorAdapter, MockPaymentOperatorAdapter>();
            }
            else if (paymentOperatorProvider.Equals("DLocal", StringComparison.OrdinalIgnoreCase))
            {
                services.AddHttpClient("payment-operator", client =>
                {
                    client.BaseAddress = new Uri(
                        configuration["PaymentOperator:ApiUrl"]
                        ?? "https://sandbox.dlocal.com");
                    client.Timeout = TimeSpan.FromSeconds(30);
                }).AddStandardResilienceHandler();

                services.AddScoped<IPaymentOperatorAdapter, DLocalPaymentOperatorAdapter>();
            }
            else
            {
                throw new InvalidOperationException(
                    $"Payment operator provider '{paymentOperatorProvider}' is not supported.");
            }

            services.AddScoped<ICompanyAdapter, CompanyAdapter>();

            return services;
        }
    }
}
