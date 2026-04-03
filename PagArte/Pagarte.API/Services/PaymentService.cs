using Mapster;
using Pagarte.API.Constants;
using Pagarte.API.Domain.Entities;
using Pagarte.API.Domain.Enums;
using Pagarte.API.DTOs.Requests;
using Pagarte.API.DTOs.Responses;
using Pagarte.API.Interfaces;
using Shared.Messaging;
using Shared.Messaging.Messages.Payment;
using Shared.Responses;

namespace Pagarte.API.Services
{
    public class PaymentService(
        IPaymentRepository paymentRepository,
        ICreditCardRepository creditCardRepository,
        IServiceRepository serviceRepository,
        IFeeConfigurationRepository feeConfigurationRepository,
        IDLocalService dLocalService,
        IMessagePublisher messagePublisher) : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository = paymentRepository;
        private readonly ICreditCardRepository _creditCardRepository = creditCardRepository;
        private readonly IServiceRepository _serviceRepository = serviceRepository;
        private readonly IFeeConfigurationRepository _feeConfigurationRepository = feeConfigurationRepository;
        private readonly IDLocalService _dLocalService = dLocalService;
        private readonly IMessagePublisher _messagePublisher = messagePublisher;

        public async Task<ApiResponse<IEnumerable<PaymentResponse>>> GetByClientIdAsync(string clientId)
        {
            try
            {
                var payments = await _paymentRepository.GetByClientIdAsync(clientId);
                return ApiResponse<IEnumerable<PaymentResponse>>.CreateSuccess(
                    payments.Adapt<IEnumerable<PaymentResponse>>());
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<PaymentResponse>>.CreateFailure(
                    $"An error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PaymentResponse>> GetByIdAsync(Guid id, string clientId)
        {
            try
            {
                var payment = await _paymentRepository.GetByIdAsync(id);
                if (payment == null || payment.ClientId != clientId)
                    return ApiResponse<PaymentResponse>.CreateFailure(Messages.Payment.NotFound);

                return ApiResponse<PaymentResponse>.CreateSuccess(payment.Adapt<PaymentResponse>());
            }
            catch (Exception ex)
            {
                return ApiResponse<PaymentResponse>.CreateFailure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PaymentResponse>> ProcessPaymentAsync(string clientId, ProcessPaymentRequest request)
        {
            try
            {
                // Validate credit card belongs to client
                var card = await _creditCardRepository.GetByIdAsync(request.CreditCardId);
                if (card == null || card.ClientId != clientId)
                    return ApiResponse<PaymentResponse>.CreateFailure(Messages.Payment.CardNotFound);

                // Validate service exists and is active
                var service = await _serviceRepository.GetByIdAsync(request.ServiceId);
                if (service == null)
                    return ApiResponse<PaymentResponse>.CreateFailure(Messages.Payment.ServiceNotFound);

                // Calculate fees
                var fees = await _feeConfigurationRepository.GetActiveFeesAsync();
                var details = CalculatePaymentDetails(service, fees.ToList(), request.Currency);
                var totalAmount = details.Sum(d => d.IsPositive ? d.Amount : -d.Amount);

                // Create payment record
                var payment = Payment.Create(clientId, request.CreditCardId, request.ServiceId, request.Currency);
                await _paymentRepository.CreateAsync(payment);

                // Add payment details
                foreach (var detail in details)
                {
                    payment.Details.Add(PaymentDetail.Create(
                        payment.Id, detail.Type, detail.Description, detail.Amount, request.Currency));
                }

                // Charge card via dLocal
                payment.UpdateStatus(TransactionStatus.ChargingCard);
                await _paymentRepository.UpdateAsync(payment);

                var dLocalResult = await _dLocalService.ChargeAsync(
                    card.DLocalCardToken, totalAmount, request.Currency, payment.Reference);

                if (!dLocalResult.Success)
                {
                    payment.UpdateStatus(TransactionStatus.Failed, dLocalResult.ErrorMessage);
                    await _paymentRepository.UpdateAsync(payment);
                    return ApiResponse<PaymentResponse>.CreateFailure(Messages.Payment.DLocalError);
                }

                // Card charged successfully - send to company via RabbitMQ
                payment.SetDLocalPaymentId(dLocalResult.DLocalPaymentId!);
                payment.UpdateStatus(TransactionStatus.CardCharged);
                await _paymentRepository.UpdateAsync(payment);

                await _messagePublisher.PublishAsync(new PaymentRequestMessage
                {
                    PaymentId = payment.Id,
                    ServiceId = service.Id,
                    CompanyId = service.CompanyId,
                    Amount = service.BaseAmount,
                    Currency = request.Currency,
                    Reference = payment.Reference,
                    ClientId = clientId,
                    DLocalPaymentId = dLocalResult.DLocalPaymentId!
                });

                return ApiResponse<PaymentResponse>.CreateSuccess(
                    payment.Adapt<PaymentResponse>(), Messages.Payment.Created);
            }
            catch (Exception ex)
            {
                return ApiResponse<PaymentResponse>.CreateFailure($"An error occurred: {ex.Message}");
            }
        }

        private List<(PaymentDetailType Type, string Description, decimal Amount, bool IsPositive)> CalculatePaymentDetails(
            Service service, List<FeeConfiguration> fees, string currency)
        {
            var details = new List<(PaymentDetailType, string, decimal, bool)>
            {
                (PaymentDetailType.ServiceAmount, service.Name, service.BaseAmount, true)
            };

            foreach (var fee in fees)
            {
                decimal feeAmount = fee.CalculationType == CalculationType.Percentage
                    ? service.BaseAmount * fee.Value / 100
                    : fee.Value;

                var detailType = fee.Type switch
                {
                    FeeType.DLocal => PaymentDetailType.DLocalFee,
                    FeeType.Company => PaymentDetailType.CompanyFee,
                    FeeType.Pagarte => PaymentDetailType.PagarteFee,
                    _ => PaymentDetailType.Tax
                };

                details.Add((detailType, $"{fee.Type} Fee", feeAmount, false));
            }

            return details;
        }
    }
}
