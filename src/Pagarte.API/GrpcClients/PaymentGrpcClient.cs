using Pagarte.Contracts;

namespace Pagarte.API.GrpcClients
{
	public class PaymentGrpcClient(PaymentService.PaymentServiceClient client)
	{
		private readonly PaymentService.PaymentServiceClient _client = client;

		public async Task<CreatePaymentQuoteResponse> CreatePaymentQuoteAsync(
			string clientId, string serviceId, string currency)
			=> await _client.CreatePaymentQuoteAsync(new CreatePaymentQuoteRequest
			{
				ClientId = clientId,
				ServiceId = serviceId,
				Currency = currency
			});

		public async Task<ProcessPaymentResponse> ConfirmPaymentAsync(
			string clientId, string quoteId, string creditCardId)
			=> await _client.ConfirmPaymentAsync(new ConfirmPaymentRequest
			{
				ClientId = clientId,
				QuoteId = quoteId,
				CreditCardId = creditCardId
			});

		public async Task<GetPaymentResponse> GetPaymentAsync(
			string paymentId, string clientId)
			=> await _client.GetPaymentAsync(
				new GetPaymentRequest { PaymentId = paymentId, ClientId = clientId });

		public async Task<GetPaymentHistoryResponse> GetPaymentHistoryAsync(
			string clientId, int page = 1, int pageSize = 20)
			=> await _client.GetPaymentHistoryAsync(new GetPaymentHistoryRequest
			{
				ClientId = clientId,
				Page = page,
				PageSize = pageSize
			});
	}
}
