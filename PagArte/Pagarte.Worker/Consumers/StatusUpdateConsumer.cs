using Infrastructure.Messaging;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Messaging;
using Shared.Messaging.Messages.Payment;

namespace Pagarte.Worker.Consumers
{
    public class StatusUpdateConsumer(
        RabbitMQConnectionFactory connectionFactory,
        ILogger<StatusUpdateConsumer> logger,
        IConfiguration configuration,
        IProcessingLogRepository processingLogRepository)
        : BaseConsumer<StatusUpdateMessage>(connectionFactory, logger)
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IProcessingLogRepository _processingLogRepository = processingLogRepository;

        protected override string QueueName => QueueNames.Queues.StatusUpdate;

        protected override async Task HandleAsync(StatusUpdateMessage message, CancellationToken cancellationToken)
        {
            var log = Pagarte.Worker.Domain.ProcessingLog.Create(
                Pagarte.Worker.Domain.MessageType.StatusUpdate,
                System.Text.Json.JsonSerializer.Serialize(message),
                message.PaymentId);

            await _processingLogRepository.CreateAsync(log);

            try
            {
                // Connect to Pagarte.API SignalR hub and push status update
                var hubUrl = _configuration["PagarteAPI:SignalRUrl"]
                    ?? throw new InvalidOperationException("PagarteAPI:SignalRUrl not configured.");

                var hubConnection = new HubConnectionBuilder()
                    .WithUrl(hubUrl)
                    .WithAutomaticReconnect()
                    .Build();

                await hubConnection.StartAsync(cancellationToken);

                // Push to payment group and client group
                await hubConnection.InvokeAsync("SendPaymentUpdate",
                    message.PaymentId.ToString(),
                    message.ClientId,
                    message.Status,
                    message.ErrorMessage,
                    cancellationToken: cancellationToken);

                await hubConnection.StopAsync(cancellationToken);

                log.Complete();
                _logger.LogInformation("Status update pushed for payment {PaymentId}: {Status}",
                    message.PaymentId, message.Status);
            }
            catch (Exception ex)
            {
                log.Fail(ex.Message);
                _logger.LogError(ex, "Error pushing status update for payment {PaymentId}", message.PaymentId);
            }
            finally
            {
                await _processingLogRepository.UpdateAsync(log);
            }
        }
    }
}
