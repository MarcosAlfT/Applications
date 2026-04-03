using RabbitMQ.Client;
using Shared.Messaging;
using Shared.Messaging.Messages.Email;
using Shared.Messaging.Messages.Payment;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Messaging
{
    public class RabbitMQPublisher : IMessagePublisher, IDisposable
    {
        private readonly RabbitMQConnectionFactory _connectionFactory;
        private IModel? _channel;

        public RabbitMQPublisher(RabbitMQConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            InitializeChannel();
        }

        private void InitializeChannel()
        {
            var connection = _connectionFactory.GetConnection();
            _channel = connection.CreateModel();
            RabbitMQTopology.DeclareAll(_channel);
        }

        public Task PublishAsync<T>(T message, string? queueName = null) where T : class
        {
            var (exchange, routingKey) = ResolveRouting(message, queueName);
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel!.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            return Task.CompletedTask;
        }

        private static (string exchange, string routingKey) ResolveRouting<T>(T message, string? queueName)
        {
            if (queueName != null)
                return (ResolveExchangeFromQueue(queueName), queueName);

            return message switch
            {
                PaymentRequestMessage => (QueueNames.Exchanges.Payments, QueueNames.Queues.PaymentRequest),
                PaymentResultMessage => (QueueNames.Exchanges.Payments, QueueNames.Queues.PaymentResult),
                RefundRequestMessage => (QueueNames.Exchanges.Payments, QueueNames.Queues.RefundRequest),
                StatusUpdateMessage => (QueueNames.Exchanges.Status, QueueNames.Queues.StatusUpdate),
                EmailMessage => (QueueNames.Exchanges.Notifications, QueueNames.Queues.EmailSend),
                _ => throw new InvalidOperationException($"Unknown message type: {typeof(T).Name}")
            };
        }

        private static string ResolveExchangeFromQueue(string queueName)
        {
            if (queueName.StartsWith("payment") || queueName.StartsWith("refund"))
                return QueueNames.Exchanges.Payments;
            if (queueName.StartsWith("email") || queueName.StartsWith("alert"))
                return QueueNames.Exchanges.Notifications;
            if (queueName.StartsWith("status"))
                return QueueNames.Exchanges.Status;

            throw new InvalidOperationException($"Cannot resolve exchange for queue: {queueName}");
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
        }
    }
}
