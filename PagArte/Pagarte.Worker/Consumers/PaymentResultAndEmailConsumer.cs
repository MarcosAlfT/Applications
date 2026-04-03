using Infrastructure.Messaging;
using Microsoft.Extensions.Logging;
using Shared.Messaging;
using Shared.Messaging.Messages.Email;
using Shared.Messaging.Messages.Payment;

namespace Pagarte.Worker.Consumers
{
    // Updates payment status in the database
    public class PaymentResultConsumer(
        RabbitMQConnectionFactory connectionFactory,
        ILogger<PaymentResultConsumer> logger,
        IPaymentWorkerRepository paymentRepository,
        IProcessingLogRepository processingLogRepository)
        : BaseConsumer<PaymentResultMessage>(connectionFactory, logger)
    {
        private readonly IPaymentWorkerRepository _paymentRepository = paymentRepository;
        private readonly IProcessingLogRepository _processingLogRepository = processingLogRepository;

        protected override string QueueName => QueueNames.Queues.PaymentResult;

        protected override async Task HandleAsync(PaymentResultMessage message, CancellationToken cancellationToken)
        {
            var log = Pagarte.Worker.Domain.ProcessingLog.Create(
                Pagarte.Worker.Domain.MessageType.PaymentResult,
                System.Text.Json.JsonSerializer.Serialize(message),
                message.PaymentId);

            await _processingLogRepository.CreateAsync(log);

            try
            {
                await _paymentRepository.UpdateStatusAsync(
                    message.PaymentId,
                    message.Success ? "Completed" : "Failed",
                    message.CompanyReference,
                    message.ErrorMessage);

                log.Complete();
                _logger.LogInformation("Payment {PaymentId} status updated to {Status}",
                    message.PaymentId, message.Success ? "Completed" : "Failed");
            }
            catch (Exception ex)
            {
                log.Fail(ex.Message);
                _logger.LogError(ex, "Error updating payment status {PaymentId}", message.PaymentId);
            }
            finally
            {
                await _processingLogRepository.UpdateAsync(log);
            }
        }
    }

    // Sends emails asynchronously
    public class EmailConsumer(
        RabbitMQConnectionFactory connectionFactory,
        ILogger<EmailConsumer> logger,
        IEmailSenderService emailSender,
        IProcessingLogRepository processingLogRepository)
        : BaseConsumer<EmailMessage>(connectionFactory, logger)
    {
        private readonly IEmailSenderService _emailSender = emailSender;
        private readonly IProcessingLogRepository _processingLogRepository = processingLogRepository;

        protected override string QueueName => QueueNames.Queues.EmailSend;

        protected override async Task HandleAsync(EmailMessage message, CancellationToken cancellationToken)
        {
            var log = Pagarte.Worker.Domain.ProcessingLog.Create(
                Pagarte.Worker.Domain.MessageType.EmailSend,
                System.Text.Json.JsonSerializer.Serialize(message));

            await _processingLogRepository.CreateAsync(log);

            try
            {
                await _emailSender.SendAsync(message.To, message.Subject, message.Body, message.IsHtml);
                log.Complete();
                _logger.LogInformation("Email sent to {To}", message.To);
            }
            catch (Exception ex)
            {
                log.Fail(ex.Message);
                _logger.LogError(ex, "Error sending email to {To}", message.To);
            }
            finally
            {
                await _processingLogRepository.UpdateAsync(log);
            }
        }
    }
}
