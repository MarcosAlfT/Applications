using Infrastructure.Messaging;
using Microsoft.Extensions.Logging;
using Shared.Messaging;
using Shared.Messaging.Messages.Notification;
using Shared.Messaging.Messages.Payment;

namespace Pagarte.Worker.Consumers
{
    public class RefundConsumer(
        RabbitMQConnectionFactory connectionFactory,
        IMessagePublisher messagePublisher,
        ILogger<RefundConsumer> logger,
        IDLocalWorkerService dLocalService,
        IProcessingLogRepository processingLogRepository)
        : BaseConsumer<RefundRequestMessage>(connectionFactory, logger)
    {
        private readonly IMessagePublisher _messagePublisher = messagePublisher;
        private readonly IDLocalWorkerService _dLocalService = dLocalService;
        private readonly IProcessingLogRepository _processingLogRepository = processingLogRepository;
        private const int MaxRetries = 3;

        protected override string QueueName => QueueNames.Queues.RefundRequest;

        protected override async Task HandleAsync(RefundRequestMessage message, CancellationToken cancellationToken)
        {
            var log = Pagarte.Worker.Domain.ProcessingLog.Create(
                Pagarte.Worker.Domain.MessageType.RefundRequest,
                System.Text.Json.JsonSerializer.Serialize(message),
                message.PaymentId);

            await _processingLogRepository.CreateAsync(log);

            try
            {
                _logger.LogInformation("Processing refund for payment {PaymentId}, attempt {Retry}",
                    message.PaymentId, message.RetryCount + 1);

                var result = await _dLocalService.RefundAsync(
                    message.DLocalPaymentId, message.Amount,
                    message.Currency, message.Reason);

                if (result.Success)
                {
                    await _messagePublisher.PublishAsync(new PaymentResultMessage
                    {
                        PaymentId = message.PaymentId,
                        Success = false,
                        ErrorMessage = message.Reason
                    });

                    await _messagePublisher.PublishAsync(new StatusUpdateMessage
                    {
                        PaymentId = message.PaymentId,
                        Status = "Refunded",
                        ErrorMessage = message.Reason
                    });

                    log.Complete();
                }
                else if (message.RetryCount < MaxRetries - 1)
                {
                    // Retry - republish with incremented retry count
                    _logger.LogWarning("Refund failed for {PaymentId}, scheduling retry {Retry}/{Max}",
                        message.PaymentId, message.RetryCount + 1, MaxRetries);

                    await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);

                    await _messagePublisher.PublishAsync(new RefundRequestMessage
                    {
                        PaymentId = message.PaymentId,
                        DLocalPaymentId = message.DLocalPaymentId,
                        Amount = message.Amount,
                        Currency = message.Currency,
                        Reason = message.Reason,
                        RetryCount = message.RetryCount + 1
                    });

                    log.IncrementRetry();
                }
                else
                {
                    // Max retries reached - alert needed
                    _logger.LogError("Refund failed after {Max} attempts for payment {PaymentId}",
                        MaxRetries, message.PaymentId);

                    await _messagePublisher.PublishAsync(new StatusUpdateMessage
                    {
                        PaymentId = message.PaymentId,
                        Status = "RefundFailed",
                        ErrorMessage = $"Refund failed after {MaxRetries} attempts: {result.ErrorMessage}"
                    });

                    await _messagePublisher.PublishAsync(new AlertMessage
                    {
                        PaymentId = message.PaymentId,
                        Message = $"URGENT: Refund failed after {MaxRetries} attempts. Manual intervention required.",
                        CreatedAt = DateTime.UtcNow
                    });

                    log.MoveToDeadLetter(result.ErrorMessage ?? "Max retries reached");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund for {PaymentId}", message.PaymentId);
                log.Fail(ex.Message);
            }
            finally
            {
                await _processingLogRepository.UpdateAsync(log);
            }
        }
    }
}
