using Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using Pagarte.API.Hubs;
using Pagarte.API.Infrastructure.Repository;
using Pagarte.API.Interfaces;
using Pagarte.API.Services;
using Shared.Messaging;
using System.Text.Json.Serialization;

namespace Pagarte.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            builder.AddServiceDefaults();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler =
                        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });

            // Database
            builder.Services.AddDbContext<PagarteDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("PagarteDb")));

            // Repositories
            // builder.Services.AddScoped<ICreditCardRepository, CreditCardRepository>();
            // builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            // builder.Services.AddScoped<IServiceRepository, ServiceRepository>();

            // Services
            // builder.Services.AddScoped<ICreditCardService, CreditCardService>();
            // builder.Services.AddScoped<IPaymentService, PaymentService>();

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

            // SignalR
            builder.Services.AddSignalR();

            var app = builder.Build();

            app.MapDefaultEndpoints();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
