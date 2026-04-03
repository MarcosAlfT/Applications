using Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Pagarte.Worker.Consumers;
using Pagarte.Worker.Infrastructure;
using Pagarte.Worker.Repositories;
using Pagarte.Worker.Services;
using Shared.Messaging;

namespace Pagarte.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;

                    // Worker database
                    services.AddDbContext<WorkerDbContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("PagarteWorkerDb")));

                    // Repositories
                    services.AddScoped<IProcessingLogRepository, ProcessingLogRepository>();
                    services.AddScoped<IPaymentWorkerRepository, PaymentWorkerRepository>();
                    services.AddScoped<ICompanyConfigRepository, CompanyConfigRepository>();

                    // Services
                    services.AddScoped<ICompanyService, CompanyService>();
                    services.AddScoped<IDLocalWorkerService, DLocalWorkerService>();
                    services.AddScoped<IEmailSenderService, EmailSenderService>();

                    // RabbitMQ
                    services.AddSingleton(new RabbitMQConnectionFactory(
                        configuration["RabbitMQ:Host"] ?? "localhost",
                        configuration.GetValue<int>("RabbitMQ:Port"),
                        configuration["RabbitMQ:Username"] ?? "guest",
                        configuration["RabbitMQ:Password"] ?? "guest"));
                    services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();

                    // HttpClients
                    services.AddHttpClient("dlocal", client =>
                    {
                        client.BaseAddress = new Uri(configuration["DLocal:ApiUrl"]
                            ?? "https://sandbox.dlocal.com");
                        client.Timeout = TimeSpan.FromSeconds(30);
                    });
                    services.AddHttpClient();

                    // RabbitMQ Consumers as hosted services
                    services.AddHostedService<PaymentRequestConsumer>();
                    services.AddHostedService<PaymentResultConsumer>();
                    services.AddHostedService<RefundConsumer>();
                    services.AddHostedService<EmailConsumer>();
                    services.AddHostedService<StatusUpdateConsumer>();
                })
                .Build();

            host.Run();
        }
    }
}
