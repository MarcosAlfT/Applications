<<<<<<< HEAD
using Pagarte.Connections.PaymentOperators;
=======
using Pagarte.Connections.DLocal;
>>>>>>> origin/main
using Pagarte.Messaging;
using Pagarte.Messaging.Messages;
using Pagarte.Worker.Domain.Entities;
using Pagarte.Worker.Domain.Enums;
using Pagarte.Worker.Interfaces;
using Shared.RabbitMQ;

namespace Pagarte.Worker.Services
{
	public record PaymentResult(
		bool Success,
		Guid? PaymentId,
		string? Reference,
		string? Status,
		string? ErrorMessage);

	public class PaymentEngineService(
		IPaymentRepository paymentRepository,
		ICreditCardRepository creditCardRepository,
		IServiceRepository serviceRepository,
		IFeeConfigurationRepository feeConfigRepository,
<<<<<<< HEAD
		IPaymentOperatorAdapter paymentOperatorAdapter,
=======
		IDLocalAdapter dLocalAdapter,
>>>>>>> origin/main
		IMessagePublisher messagePublisher,
		ILogger<PaymentEngineService> logger)
	{
		private readonly IPaymentRepository _paymentRepository = paymentRepository;
		private readonly ICreditCardRepository _creditCardRepository = creditCardRepository;
		private readonly IServiceRepository _serviceRepository = serviceRepository;
		private readonly IFeeConfigurationRepository _feeConfigRepository = feeConfigRepository;
<<<<<<< HEAD
		private readonly IPaymentOperatorAdapter _paymentOperatorAdapter = paymentOperatorAdapter;
=======
		private readonly IDLocalAdapter _dLocalAdapter = dLocalAdapter;
>>>>>>> origin/main
		private readonly IMessagePublisher _messagePublisher = messagePublisher;
		private readonly ILogger<PaymentEngineService> _logger = logger;

		public async Task<PaymentResult> ProcessAsync(
			string clientId, Guid creditCardId, Guid serviceId, string currency)
		{
			// Validate card belongs to client
			var card = await _creditCardRepository.GetByIdAsync(creditCardId);
			if (card == null || card.ClientId != clientId)
				return new PaymentResult(false, null, null, null, "Credit card not found.");

			// Validate service exists
			var service = await _serviceRepository.GetByIdAsync(serviceId);
			if (service == null)
				return new PaymentResult(false, null, null, null, "Service not found.");

			// Calculate fees
			var fees = (await _feeConfigRepository.GetActiveFeesAsync()).ToList();
			var details = CalculateDetails(service, fees, currency);
			var totalAmount = details.Sum(d => d.IsPositive ? d.Amount : -d.Amount);

			// Create payment record
			var payment = Payment.Create(clientId, creditCardId, serviceId, currency);
			await _paymentRepository.CreateAsync(payment);

			// Add payment details
			foreach (var detail in details)
			{
				payment.Details.Add(PaymentDetail.Create(
					payment.Id, detail.Type, detail.Description,
					detail.Amount, currency));
			}

<<<<<<< HEAD
			// Charge card via payment operator
			payment.UpdateStatus(TransactionStatus.ChargingCard);
			await _paymentRepository.UpdateAsync(payment);

			var chargeResult = await _paymentOperatorAdapter.ChargeAsync(
				card.OperatorCardToken, totalAmount, currency, payment.Reference);
=======
			// Charge card via dLocal
			payment.UpdateStatus(TransactionStatus.ChargingCard);
			await _paymentRepository.UpdateAsync(payment);

			var chargeResult = await _dLocalAdapter.ChargeAsync(
				card.DLocalCardToken, totalAmount, currency, payment.Reference);
>>>>>>> origin/main

			if (!chargeResult.Success)
			{
				payment.UpdateStatus(TransactionStatus.Failed, chargeResult.ErrorMessage);
				await _paymentRepository.UpdateAsync(payment);

<<<<<<< HEAD
				_logger.LogWarning("Payment operator charge failed for payment {Reference}: {Error}",
=======
				_logger.LogWarning("dLocal charge failed for payment {Reference}: {Error}",
>>>>>>> origin/main
					payment.Reference, chargeResult.ErrorMessage);

				return new PaymentResult(false, payment.Id, payment.Reference,
					"Failed", chargeResult.ErrorMessage);
			}

			// Card charged successfully
<<<<<<< HEAD
			payment.SetOperatorPaymentId(chargeResult.OperatorPaymentId!);
=======
			payment.SetDLocalPaymentId(chargeResult.DLocalPaymentId!);
>>>>>>> origin/main
			payment.UpdateStatus(TransactionStatus.CardCharged);
			await _paymentRepository.UpdateAsync(payment);

			// Publish to Engine for async company processing
			await _messagePublisher.PublishAsync(
				new PaymentRequestMessage
				{
					PaymentId = payment.Id,
					CompanyId = service.CompanyId,
<<<<<<< HEAD
					OperatorPaymentId = chargeResult.OperatorPaymentId!,
=======
					DLocalPaymentId = chargeResult.DLocalPaymentId!,
>>>>>>> origin/main
					Amount = totalAmount,
					Currency = currency,
					Reference = payment.Reference,
					ClientId = clientId
				},
				PagarteQueues.Exchanges.Payments,
				PagarteQueues.Queues.PaymentRequest);

			_logger.LogInformation("Payment {Reference} charged, published to Engine",
				payment.Reference);

			return new PaymentResult(true, payment.Id, payment.Reference,
				"Pending", null);
		}

		private static List<(PaymentDetailType Type, string Description,
			decimal Amount, bool IsPositive)> CalculateDetails(
			Service service, List<FeeConfiguration> fees, string currency)
		{
			var details = new List<(PaymentDetailType, string, decimal, bool)>
			{
				(PaymentDetailType.ServiceAmount, service.Name, service.BaseAmount, true)
			};

			foreach (var fee in fees)
			{
				var amount = fee.CalculationType == CalculationType.Percentage
					? service.BaseAmount * fee.Value / 100
					: fee.Value;

				var type = fee.Type switch
				{
<<<<<<< HEAD
					FeeType.PaymentOperator => PaymentDetailType.PaymentOperatorFee,
=======
					FeeType.DLocal => PaymentDetailType.DLocalFee,
>>>>>>> origin/main
					FeeType.Company => PaymentDetailType.CompanyFee,
					FeeType.Pagarte => PaymentDetailType.PagarteFee,
					_ => PaymentDetailType.Tax
				};

				details.Add((type, $"{fee.Type} Fee", amount, false));
			}

			return details;
		}
	}
}
