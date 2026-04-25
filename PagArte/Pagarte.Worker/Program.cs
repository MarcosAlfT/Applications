using Microsoft.EntityFrameworkCore;
using Pagarte.Connections.Config;
using Pagarte.Worker.GrpcServices;
using Pagarte.Worker.Infrastructure;
using Pagarte.Worker.Infrastructure.Repository;
using Pagarte.Worker.Interfaces;
using Pagarte.Worker.Services;
using Shared.RabbitMQ;

namespace Pagarte.Worker
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			var configuration = builder.Configuration;

			// Database — Worker owns PagarteDb
			builder.Services.AddDbContext<PagarteDbContext>(options =>
				options.UseSqlServer(configuration.GetConnectionString("PagarteDb")));

			// Repositories
			builder.Services.AddScoped<ICreditCardRepository, CreditCardRepository>();
			builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
			builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
			builder.Services.AddScoped<IFeeConfigurationRepository, FeeConfigurationRepository>();
			builder.Services.AddScoped<IMessagePublisher, RabbitMQPublisher>();

			// External connections (dLocal, Companies) with Polly resilience
			builder.Services.AddPagarteConnections(configuration);

			builder.Services.AddSingleton<RabbitMQConnectionFactory>(sp =>
			{
				var config = sp.GetRequiredService<IConfiguration>();
				var connectionString = 
					config.GetConnectionString("PagQueue") ??
					config["ConnectionStrings:PagQueue"] ??
					config["RABBITMQ_CONNECTION_STRING"];
			
				Console.WriteLine($"connString: {connectionString}");

				if (string.IsNullOrWhiteSpace(connectionString))
					throw new Exception("RabbitMQ connection string not found");

				return new RabbitMQConnectionFactory(connectionString!);
			});

			// Business services
			builder.Services.AddScoped<PaymentEngineService>();

			// gRPC server
			builder.Services.AddGrpc();

			var app = builder.Build();

			// Migrations and seed data
			using (var scope = app.Services.CreateScope())
			{
				var db = scope.ServiceProvider.GetRequiredService<PagarteDbContext>();
				await db.Database.MigrateAsync();
				await PagarteDbSeeder.SeedAsync(db);
			}

			// Declare RabbitMQ topology on startup
			var rabbitFactory = app.Services.GetRequiredService<RabbitMQConnectionFactory>();
			var connection = await rabbitFactory.GetConnectionAsync();
			using (var channel = await connection.CreateChannelAsync())
			
			// gRPC endpoints — only accessible from private subnet
			app.MapGrpcService<CreditCardGrpcService>();
			app.MapGrpcService<PaymentGrpcService>();
			app.MapGrpcService<ServiceCatalogGrpcService>();

			app.Run();
		}
	}
}
