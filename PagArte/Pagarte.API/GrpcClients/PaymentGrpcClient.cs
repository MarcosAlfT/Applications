<<<<<<< HEAD
using Pagarte.Contracts;
=======
﻿using Pagarte.Contracts;
>>>>>>> origin/main

namespace Pagarte.API.GrpcClients
{
	public class PaymentGrpcClient(PaymentService.PaymentServiceClient client)
	{
		private readonly PaymentService.PaymentServiceClient _client = client;

		public async Task<ProcessPaymentResponse> ProcessPaymentAsync(
			string clientId, string creditCardId,
			string serviceId, string currency)
			=> await _client.ProcessPaymentAsync(new ProcessPaymentRequest
			{
				ClientId = clientId,
				CreditCardId = creditCardId,
				ServiceId = serviceId,
				Currency = currency
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
