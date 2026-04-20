using RabbitMQ.Client;

namespace Pagarte.Messaging
{
	public static class PagarteTopology
	{
		public static async Task DeclareAllAsync(IChannel channel)  // IModel → IChannel
		{
			await channel.ExchangeDeclareAsync("dlx.payments",
				ExchangeType.Direct, durable: true);
			await channel.ExchangeDeclareAsync("dlx.notifications",
				ExchangeType.Direct, durable: true);

			await channel.ExchangeDeclareAsync(PagarteQueues.Exchanges.Payments,
				ExchangeType.Direct, durable: true);
			await channel.ExchangeDeclareAsync(PagarteQueues.Exchanges.Notifications,
				ExchangeType.Direct, durable: true);

			await channel.QueueDeclareAsync(PagarteQueues.DeadLetterQueues.PaymentRequest,
				durable: true, exclusive: false, autoDelete: false);
			await channel.QueueDeclareAsync(PagarteQueues.DeadLetterQueues.RefundRequest,
				durable: true, exclusive: false, autoDelete: false);
			await channel.QueueDeclareAsync(PagarteQueues.DeadLetterQueues.EmailSend,
				durable: true, exclusive: false, autoDelete: false);

			await channel.QueueBindAsync(PagarteQueues.DeadLetterQueues.PaymentRequest,
				"dlx.payments", PagarteQueues.Queues.PaymentRequest);
			await channel.QueueBindAsync(PagarteQueues.DeadLetterQueues.RefundRequest,
				"dlx.payments", PagarteQueues.Queues.RefundRequest);
			await channel.QueueBindAsync(PagarteQueues.DeadLetterQueues.EmailSend,
				"dlx.notifications", PagarteQueues.Queues.EmailSend);

			await channel.QueueDeclareAsync(PagarteQueues.Queues.PaymentRequest,
				durable: true, exclusive: false, autoDelete: false,
				arguments: new Dictionary<string, object?>
				{
					{ "x-dead-letter-exchange", "dlx.payments" },
					{ "x-dead-letter-routing-key", PagarteQueues.Queues.PaymentRequest }
				});

			await channel.QueueDeclareAsync(PagarteQueues.Queues.RefundRequest,
				durable: true, exclusive: false, autoDelete: false,
				arguments: new Dictionary<string, object?>
				{
					{ "x-dead-letter-exchange", "dlx.payments" },
					{ "x-dead-letter-routing-key", PagarteQueues.Queues.RefundRequest }
				});

			await channel.QueueDeclareAsync(PagarteQueues.Queues.EmailSend,
				durable: true, exclusive: false, autoDelete: false,
				arguments: new Dictionary<string, object?>
				{
					{ "x-dead-letter-exchange", "dlx.notifications" },
					{ "x-dead-letter-routing-key", PagarteQueues.Queues.EmailSend }
				});

			await channel.QueueDeclareAsync(PagarteQueues.Queues.AlertCreate,
				durable: true, exclusive: false, autoDelete: false);

			await channel.QueueBindAsync(PagarteQueues.Queues.PaymentRequest,
				PagarteQueues.Exchanges.Payments, PagarteQueues.Queues.PaymentRequest);
			await channel.QueueBindAsync(PagarteQueues.Queues.RefundRequest,
				PagarteQueues.Exchanges.Payments, PagarteQueues.Queues.RefundRequest);
			await channel.QueueBindAsync(PagarteQueues.Queues.EmailSend,
				PagarteQueues.Exchanges.Notifications, PagarteQueues.Queues.EmailSend);
			await channel.QueueBindAsync(PagarteQueues.Queues.AlertCreate,
				PagarteQueues.Exchanges.Notifications, PagarteQueues.Queues.AlertCreate);
		}
	}
}