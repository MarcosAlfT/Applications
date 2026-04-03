using RabbitMQ.Client;
using Shared.Messaging;

namespace Infrastructure.Messaging
{
    public static class RabbitMQTopology
    {
        public static void DeclareAll(IModel channel)
        {
            // ── Dead Letter Exchanges ──────────────────────────────────────
            channel.ExchangeDeclare("dlx.payments", ExchangeType.Direct, durable: true);
            channel.ExchangeDeclare("dlx.notifications", ExchangeType.Direct, durable: true);

            // ── Main Exchanges ─────────────────────────────────────────────
            channel.ExchangeDeclare(QueueNames.Exchanges.Payments, ExchangeType.Direct, durable: true);
            channel.ExchangeDeclare(QueueNames.Exchanges.Notifications, ExchangeType.Direct, durable: true);
            channel.ExchangeDeclare(QueueNames.Exchanges.Status, ExchangeType.Direct, durable: true);

            // ── Dead Letter Queues ─────────────────────────────────────────
            channel.QueueDeclare(QueueNames.DeadLetterQueues.PaymentRequest, durable: true,
                exclusive: false, autoDelete: false);
            channel.QueueDeclare(QueueNames.DeadLetterQueues.RefundRequest, durable: true,
                exclusive: false, autoDelete: false);
            channel.QueueDeclare(QueueNames.DeadLetterQueues.EmailSend, durable: true,
                exclusive: false, autoDelete: false);

            channel.QueueBind(QueueNames.DeadLetterQueues.PaymentRequest,
                "dlx.payments", QueueNames.Queues.PaymentRequest);
            channel.QueueBind(QueueNames.DeadLetterQueues.RefundRequest,
                "dlx.payments", QueueNames.Queues.RefundRequest);
            channel.QueueBind(QueueNames.DeadLetterQueues.EmailSend,
                "dlx.notifications", QueueNames.Queues.EmailSend);

            // ── Main Queues ────────────────────────────────────────────────
            var paymentRequestArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "dlx.payments" },
                { "x-dead-letter-routing-key", QueueNames.Queues.PaymentRequest }
            };

            var refundRequestArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "dlx.payments" },
                { "x-dead-letter-routing-key", QueueNames.Queues.RefundRequest }
            };

            var emailArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "dlx.notifications" },
                { "x-dead-letter-routing-key", QueueNames.Queues.EmailSend }
            };

            channel.QueueDeclare(QueueNames.Queues.PaymentRequest, durable: true,
                exclusive: false, autoDelete: false, arguments: paymentRequestArgs);
            channel.QueueDeclare(QueueNames.Queues.PaymentResult, durable: true,
                exclusive: false, autoDelete: false);
            channel.QueueDeclare(QueueNames.Queues.RefundRequest, durable: true,
                exclusive: false, autoDelete: false, arguments: refundRequestArgs);
            channel.QueueDeclare(QueueNames.Queues.EmailSend, durable: true,
                exclusive: false, autoDelete: false, arguments: emailArgs);
            channel.QueueDeclare(QueueNames.Queues.AlertCreate, durable: true,
                exclusive: false, autoDelete: false);
            channel.QueueDeclare(QueueNames.Queues.StatusUpdate, durable: true,
                exclusive: false, autoDelete: false);

            // ── Bindings ───────────────────────────────────────────────────
            channel.QueueBind(QueueNames.Queues.PaymentRequest,
                QueueNames.Exchanges.Payments, QueueNames.Queues.PaymentRequest);
            channel.QueueBind(QueueNames.Queues.PaymentResult,
                QueueNames.Exchanges.Payments, QueueNames.Queues.PaymentResult);
            channel.QueueBind(QueueNames.Queues.RefundRequest,
                QueueNames.Exchanges.Payments, QueueNames.Queues.RefundRequest);
            channel.QueueBind(QueueNames.Queues.EmailSend,
                QueueNames.Exchanges.Notifications, QueueNames.Queues.EmailSend);
            channel.QueueBind(QueueNames.Queues.AlertCreate,
                QueueNames.Exchanges.Notifications, QueueNames.Queues.AlertCreate);
            channel.QueueBind(QueueNames.Queues.StatusUpdate,
                QueueNames.Exchanges.Status, QueueNames.Queues.StatusUpdate);
        }
    }
}
