using Pagarte.Worker.Domain.Enums;

namespace Pagarte.Worker.Domain.Entities
{
	public class Payment
	{
		public Guid Id { get; set; }
		public string ClientId { get; set; } = string.Empty;
		public Guid CreditCardId { get; set; }
		public Guid ServiceId { get; set; }
<<<<<<< HEAD
		public string? OperatorPaymentId { get; set; }
=======
		public string? DLocalPaymentId { get; set; }
>>>>>>> origin/main
		public string? CompanyReference { get; set; }
		public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
		public string Currency { get; set; } = string.Empty;
		public string Reference { get; set; } = string.Empty;
		public string? ErrorMessage { get; set; }
		public int RetryCount { get; set; } = 0;
		public DateTime? NextRetryAt { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? ProcessedAt { get; set; }
		public DateTime? LastUpdatedAt { get; set; }

		public CreditCard CreditCard { get; set; } = null!;
		public Service Service { get; set; } = null!;
		public ICollection<PaymentDetail> Details { get; set; } = [];

		public static Payment Create(string clientId, Guid creditCardId,
			Guid serviceId, string currency)
		{
			return new Payment
			{
				Id = Guid.NewGuid(),
				ClientId = clientId,
				CreditCardId = creditCardId,
				ServiceId = serviceId,
				Currency = currency,
				Reference = GenerateReference(),
				Status = TransactionStatus.Pending,
				CreatedAt = DateTime.UtcNow
			};
		}

		public void UpdateStatus(TransactionStatus status, string? errorMessage = null)
		{
			Status = status;
			ErrorMessage = errorMessage;
			LastUpdatedAt = DateTime.UtcNow;

			if (status is TransactionStatus.Completed
				or TransactionStatus.Failed
				or TransactionStatus.Refunded
				or TransactionStatus.RefundFailed)
			{
				ProcessedAt = DateTime.UtcNow;
			}
		}

<<<<<<< HEAD
		public void SetOperatorPaymentId(string operatorPaymentId)
		{
			OperatorPaymentId = operatorPaymentId;
=======
		public void SetDLocalPaymentId(string dLocalPaymentId)
		{
			DLocalPaymentId = dLocalPaymentId;
>>>>>>> origin/main
			LastUpdatedAt = DateTime.UtcNow;
		}

		public void SetCompanyReference(string companyReference)
		{
			CompanyReference = companyReference;
			LastUpdatedAt = DateTime.UtcNow;
		}

		public void IncrementRetry()
		{
			RetryCount++;
			NextRetryAt = DateTime.UtcNow.AddMinutes(5);
			LastUpdatedAt = DateTime.UtcNow;
		}

		private static string GenerateReference() =>
			$"PAG-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
	}
}
