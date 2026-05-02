using Pagarte.Connections.Config;
using Pagarte.Engine.Consumers;
using Pagarte.Engine.Interfaces;
using Pagarte.Engine.Services;
using Pagarte.Messaging;
using Shared.RabbitMQ;

<<<<<<< HEAD
=======


>>>>>>> origin/main
namespace Pagarte.Engine
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;

<<<<<<< HEAD
                    // External connections (payment operator, companies) with Polly resilience.
                    services.AddPagarteConnections(configuration);

                    // Repository uses raw SQL to PagarteDb to avoid referencing Worker.
=======
                    // External connections (dLocal, Companies) with Polly resilience
                    services.AddPagarteConnections(configuration);

                    // Repository — raw SQL to PagarteDb (no EF Core dependency)
>>>>>>> origin/main
                    services.AddScoped<IPaymentStatusRepository, PaymentStatusRepository>();

                    // Email service
                    services.AddScoped<IEmailSenderService, EmailSenderService>();

                    // RabbitMQ
<<<<<<< HEAD
                    services.AddRabbitMq(configuration);

                    // Each consumer listens to one queue.
=======
                    services.AddSingleton(new RabbitMQConnectionFactory(
                        configuration["RabbitMQ:Host"] ?? "localhost",
                        configuration.GetValue<int>("RabbitMQ:Port"),
                        configuration["RabbitMQ:Username"] ?? "guest",
                        configuration["RabbitMQ:Password"] ?? "guest"));
                    services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();

                    // Consumers — each listens to one queue, that's it
>>>>>>> origin/main
                    services.AddHostedService<PaymentRequestConsumer>();
                    services.AddHostedService<RefundConsumer>();
                    services.AddHostedService<EmailConsumer>();
                })
                .Build();

<<<<<<< HEAD
            var rabbitFactory = host.Services.GetRequiredService<RabbitMQConnectionFactory>();
=======
            //var host = builder.Build();

            // Declare RabbitMQ topology on startup
            var rabbitFactory = host.Services.GetRequiredService<RabbitMQConnectionFactory>();

>>>>>>> origin/main
            var connection = await rabbitFactory.GetConnectionAsync();
            var channel = await connection.CreateChannelAsync();
            await PagarteTopology.DeclareAllAsync(channel);

<<<<<<< HEAD
            host.Run();
=======
			host.Run();
>>>>>>> origin/main
        }
    }
}
