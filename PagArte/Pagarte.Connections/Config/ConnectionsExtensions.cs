using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pagarte.Connections.Companies;
<<<<<<< HEAD
using Pagarte.Connections.PaymentOperators;
=======
using Pagarte.Connections.DLocal;
>>>>>>> origin/main

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
<<<<<<< HEAD
            var paymentOperatorProvider = configuration["PaymentOperator:Provider"];

            if (string.IsNullOrWhiteSpace(paymentOperatorProvider))
            {
                throw new InvalidOperationException("PaymentOperator:Provider is not configured.");
            }
=======
            // dLocal HttpClient with resilience
            services.AddHttpClient("dlocal", client =>
            {
                client.BaseAddress = new Uri(
                    configuration["DLocal:ApiUrl"]
                    ?? "https://sandbox.dlocal.com");
                client.Timeout = TimeSpan.FromSeconds(30);
            }).AddStandardResilienceHandler();
>>>>>>> origin/main

            // Company HttpClient with resilience
            services.AddHttpClient<CompanyAdapter>()
                .AddStandardResilienceHandler();

<<<<<<< HEAD
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

=======
            // Register adapters
            services.AddScoped<IDLocalAdapter, DLocalAdapter>();
>>>>>>> origin/main
            services.AddScoped<ICompanyAdapter, CompanyAdapter>();

            return services;
        }
    }
}
