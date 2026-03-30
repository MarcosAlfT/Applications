using Infrastructure.Messaging;
using Microsoft.Extensions.Logging;
using Shared.Messaging;
using Shared.Messaging.Messages.Payment;

namespace Pagarte.Worker.Consumers
{
    public class PaymentRequestConsumer(
        RabbitMQConnectionFactory connectionFactory,
        IMessagePublisher messagePublisher,
        ILogger<PaymentRequestConsumer> logger,
        ICompanyService companyService,
        IProcessingLogRepository processingLogRepository)
        : BaseConsumer<PaymentRequestMessage>(connectionFactory, logger)
    {
        private readonly IMessagePublisher _messagePublisher = messagePublisher;
        private readonly ICompanyService _companyService = companyService;
        private readonly IProcessingLogRepository _processingLogRepository = processingLogRepository;

        protected override string QueueName => QueueNames.Queues.PaymentRequest;

        protected override async Task HandleAsync(PaymentRequestMessage message, CancellationToken cancellationToken)
        {
            var log = Pagarte.Worker.Domain.ProcessingLog.Create(
                Pagarte.Worker.Domain.MessageType.PaymentRequest,
                System.Text.Json.JsonSerializer.Serialize(message),
                message.PaymentId);

            await _processingLogRepository.CreateAsync(log);

            try
            {
                _logger.LogInformation("Processing payment request {PaymentId}", message.PaymentId);

                // Send payment to company
                var result = await _companyService.SendPaymentAsync(
                    message.CompanyId, message.Amount, message.Currency,
                    message.Reference, message.ClientId);

                if (result.Success)
                {
                    // Publish success result
                    await _messagePublisher.PublishAsync(new PaymentResultMessage
                    {
                        PaymentId = message.PaymentId,
                        Success = true,
                        CompanyReference = result.CompanyReference
                    });

                    // Publish status update for SignalR
                    await _messagePublisher.PublishAsync(new StatusUpdateMessage
                    {
                        PaymentId = message.PaymentId,
                        ClientId = message.ClientId,
                        Status = "Completed",
                        Reference = message.Reference
                    });

                    log.Complete();
                }
                else
                {
                    _logger.LogWarning("Company rejected payment {PaymentId}: {Error}",
                        message.PaymentId, result.ErrorMessage);

                    // Publish refund request
                    await _messagePublisher.PublishAsync(new RefundRequestMessage
                    {
                        PaymentId = message.PaymentId,
                        DLocalPaymentId = message.DLocalPaymentId,
                        Amount = message.Amount,
                        Currency = message.Currency,
                        Reason = result.ErrorMessage ?? "Company rejected payment"
                    });

                    log.Fail(result.ErrorMessage ?? "Company rejected payment");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment {PaymentId}", message.PaymentId);
                log.Fail(ex.Message);

                // Trigger refund
                await _messagePublisher.PublishAsync(new RefundRequestMessage
                {
                    PaymentId = message.PaymentId,
                    DLocalPaymentId = message.DLocalPaymentId,
                    Amount = message.Amount,
                    Currency = message.Currency,
                    Reason = ex.Message
                });
            }
            finally
            {
                await _processingLogRepository.UpdateAsync(log);
            }
        }
    }
}
