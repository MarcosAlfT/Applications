using Microsoft.EntityFrameworkCore;
using Pagarte.Connections.Config;
using Pagarte.Messaging;
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

			// Database - Worker owns PagarteDb
			builder.Services.AddDbContext<PagarteDbContext>(options =>
				options.UseSqlServer(configuration.GetConnectionString("PagarteDb")));

			// Repositories
			builder.Services.AddScoped<ICreditCardRepository, CreditCardRepository>();
			builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
			builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
			builder.Services.AddScoped<IFeeConfigurationRepository, FeeConfigurationRepository>();
			builder.Services.AddScoped<IMessagePublisher, RabbitMQPublisher>();

			// External connections (payment operator, companies) with Polly resilience
			builder.Services.AddPagarteConnections(configuration);

			// Business services
			builder.Services.AddScoped<PaymentEngineService>();

			// gRPC server
			builder.Services.AddGrpc();

			// RabbitMQ publisher
			builder.Services.AddRabbitMq(builder.Configuration);

			var app = builder.Build();
			var logger = app.Services.GetRequiredService<ILogger<Program>>();

			// Migrations and seed data
			using (var scope = app.Services.CreateScope())
			{
				logger.LogInformation("Applying Pagarte database migrations.");

				var db = scope.ServiceProvider.GetRequiredService<PagarteDbContext>();
				await db.Database.MigrateAsync();
				await PagarteDbSeeder.SeedAsync(db);

				logger.LogInformation("Pagarte database migrations and seed data completed.");
			}

			// gRPC endpoints - only accessible from private subnet
			app.MapGrpcService<CreditCardGrpcService>();
			app.MapGrpcService<PaymentGrpcService>();
			app.MapGrpcService<ServiceCatalogGrpcService>();

			app.Lifetime.ApplicationStarted.Register(() =>
			{
				_ = Task.Run(async () =>
				{
					try
					{
						using var scope = app.Services.CreateScope();
						var rabbitFactory = scope.ServiceProvider.GetRequiredService<RabbitMQConnectionFactory>();
						var connection = await rabbitFactory.GetConnectionAsync();
						using var channel = await connection.CreateChannelAsync();
						await PagarteTopology.DeclareAllAsync(channel);
					}
					catch (Exception ex)
					{
						logger.LogError(ex, "Failed to declare Pagarte RabbitMQ topology.");
					}
				});
			});

			app.Run();
		}
	}
}
