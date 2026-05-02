using Microsoft.EntityFrameworkCore;
using Pagarte.Connections.Config;
<<<<<<< HEAD
using Pagarte.Messaging;
=======
>>>>>>> origin/main
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

<<<<<<< HEAD
			// Database - Worker owns PagarteDb
=======
			// Database — Worker owns PagarteDb
>>>>>>> origin/main
			builder.Services.AddDbContext<PagarteDbContext>(options =>
				options.UseSqlServer(configuration.GetConnectionString("PagarteDb")));

			// Repositories
			builder.Services.AddScoped<ICreditCardRepository, CreditCardRepository>();
			builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
			builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
			builder.Services.AddScoped<IFeeConfigurationRepository, FeeConfigurationRepository>();
<<<<<<< HEAD
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
=======

			// External connections (dLocal, Companies) with Polly resilience
			builder.Services.AddPagarteConnections(configuration);

			// RabbitMQ publisher — Worker publishes after sync operations
			//------------------------------------------------------------
			// This is to use RabbitMQ directly without going through an abstraction layer
			//builder.Services.AddSingleton(new RabbitMQConnectionFactory(
			//	configuration["RabbitMQ:Host"] ?? "localhost",
			//	configuration.GetValue<int>("RabbitMQ:Port"),
			//	configuration["RabbitMQ:Username"] ?? "guest",
			//	configuration["RabbitMQ:Password"] ?? "guest"));
			//builder.Services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();

			builder.Services.AddSingleton<RabbitMQConnectionFactory>(sp =>
			{
				var config = sp.GetRequiredService<IConfiguration>();
				var connectionString = config.GetConnectionString("PagQueue");
				Console.WriteLine($"connString: {connectionString}");

				return new RabbitMQConnectionFactory(connectionString!);
			});

			// Business services
			builder.Services.AddScoped<PaymentEngineService>();

			// gRPC server
			builder.Services.AddGrpc();

			var app = builder.Build();
>>>>>>> origin/main

			// Migrations and seed data
			using (var scope = app.Services.CreateScope())
			{
<<<<<<< HEAD
				logger.LogInformation("Applying Pagarte database migrations.");

				var db = scope.ServiceProvider.GetRequiredService<PagarteDbContext>();
				await db.Database.MigrateAsync();
				await PagarteDbSeeder.SeedAsync(db);

				logger.LogInformation("Pagarte database migrations and seed data completed.");
			}

			// gRPC endpoints - only accessible from private subnet
=======
				var db = scope.ServiceProvider.GetRequiredService<PagarteDbContext>();
				await db.Database.MigrateAsync();
				await PagarteDbSeeder.SeedAsync(db);
			}

			// Declare RabbitMQ topology on startup
			var rabbitFactory = app.Services.GetRequiredService<RabbitMQConnectionFactory>();
			var connection = await rabbitFactory.GetConnectionAsync();
			using (var channel = await connection.CreateChannelAsync())
			
			// gRPC endpoints — only accessible from private subnet
>>>>>>> origin/main
			app.MapGrpcService<CreditCardGrpcService>();
			app.MapGrpcService<PaymentGrpcService>();
			app.MapGrpcService<ServiceCatalogGrpcService>();

<<<<<<< HEAD
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

=======
>>>>>>> origin/main
			app.Run();
		}
	}
}
