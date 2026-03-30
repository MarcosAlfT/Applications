
using Clients.API.Infrastructure.Repository;
using Clients.API.Interfaces;
using Clients.API.Services;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using System.Text.Json.Serialization;
using Clients.API.DTOs.Responses;

namespace Clients.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
		var configuration = builder.Configuration;
		builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
		});

		// Configure Entity Framework Core with SQL Server
		builder.Services.AddDbContext<ClientsDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("ClientsDb")));

		// Register the Repositories
        builder.Services.AddScoped<IClientRepository, ClientRepository>();
        builder.Services.AddScoped<IPersonRepository, PersonRepository>();
        builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
		builder.Services.AddScoped<IAddressRepository, AddressRepository>();
        builder.Services.AddScoped<IPhoneRepository, PhoneRepository>();

		// Register the Services
        builder.Services.AddScoped<IClientService, ClientService>();
        builder.Services.AddScoped<IAddressService, AddressService>();
        builder.Services.AddScoped<IPhoneService, PhoneService>();

		//Register the mappers
        MappingConfig.Configure();

		// Configure OpenIddict validation
		builder.Services.AddOpenIddict()
            .AddValidation(options =>
            {

				var strAuthority = configuration.GetValue<string>("AuthSettings:Authority")
    				?? throw new InvalidOperationException("AuthSettings:Authority is not configured."); ;
				var strAudience = configuration.GetValue<string>("AuthSettings:Audience")
    				?? throw new InvalidOperationException("AuthSettings:Authority is not configured."); ;

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


		// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
		builder.Services.AddOpenApi();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
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
