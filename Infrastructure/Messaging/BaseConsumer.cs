using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Messaging
{
    public abstract class BaseConsumer<T> : BackgroundService where T : class
    {
        private readonly RabbitMQConnectionFactory _connectionFactory;
        protected readonly ILogger _logger;
        private IModel? _channel;
        protected abstract string QueueName { get; }
        protected virtual ushort PrefetchCount => 1;

        protected BaseConsumer(RabbitMQConnectionFactory connectionFactory, ILogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = _connectionFactory.GetConnection();
            _channel = connection.CreateModel();
            RabbitMQTopology.DeclareAll(_channel);

            _channel.BasicQos(prefetchSize: 0, prefetchCount: PrefetchCount, global: false);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (_, ea) =>
            {
                try
                {
                    var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var message = JsonSerializer.Deserialize<T>(body);

                    if (message != null)
                        await HandleAsync(message, stoppingToken);

                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from {Queue}", QueueName);
                    // Reject and send to DLQ after max retries
                    _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        protected abstract Task HandleAsync(T message, CancellationToken cancellationToken);

        public override void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            base.Dispose();
        }
    }
}
