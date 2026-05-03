using OpenIddict.Validation.AspNetCore;
using Pagarte.API.GrpcClients;
using Pagarte.Contracts;

namespace Pagarte.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

			builder.Services.AddControllers();

			// gRPC clients call Pagarte.Worker.
			var workerUrl = configuration["PagarteWorker:GrpcUrl"];

			if (string.IsNullOrWhiteSpace(workerUrl))
			{
				throw new InvalidOperationException("PagarteWorker:GrpcUrl is not configured.");
			}

			var allowUntrustedWorkerCertificate =
				builder.Environment.IsDevelopment()
				&& configuration.GetValue<bool>("PagarteWorker:AllowUntrustedCertificate");

			builder.Services.AddGrpcClient<CreditCardService.CreditCardServiceClient>(
				o => o.Address = new Uri(workerUrl))
				.ConfigurePrimaryHttpMessageHandler(() => CreateGrpcHttpHandler(allowUntrustedWorkerCertificate));
			builder.Services.AddGrpcClient<PaymentService.PaymentServiceClient>(
				o => o.Address = new Uri(workerUrl))
				.ConfigurePrimaryHttpMessageHandler(() => CreateGrpcHttpHandler(allowUntrustedWorkerCertificate));
			builder.Services.AddGrpcClient<ServiceCatalogService.ServiceCatalogServiceClient>(
				o => o.Address = new Uri(workerUrl))
				.ConfigurePrimaryHttpMessageHandler(() => CreateGrpcHttpHandler(allowUntrustedWorkerCertificate));

			// gRPC client wrappers
			builder.Services.AddScoped<CreditCardGrpcClient>();
			builder.Services.AddScoped<PaymentGrpcClient>();
			builder.Services.AddScoped<ServiceCatalogGrpcClient>();

			// OpenIddict validation
			builder.Services.AddOpenIddict()
                .AddValidation(options =>
                {
                    var strAuthority = configuration.GetValue<string>("AuthSettings:Authority")
                        ?? throw new InvalidOperationException("AuthSettings:Authority is not configured.");
                    var strAudience = configuration.GetValue<string>("AuthSettings:Audience")
                        ?? throw new InvalidOperationException("AuthSettings:Audience is not configured.");

                    options.SetIssuer(strAuthority);
                    options.AddAudiences(strAudience);
                    options.UseSystemNetHttp();
                    options.UseAspNetCore();
                });


			builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            });


			builder.Services.AddAuthorization();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

			app.Run();
        }

		private static HttpMessageHandler CreateGrpcHttpHandler(bool allowUntrustedCertificate)
		{
			var handler = new HttpClientHandler();

			if (allowUntrustedCertificate)
			{
				handler.ServerCertificateCustomValidationCallback =
					HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
			}

			return handler;
		}
    }
}
