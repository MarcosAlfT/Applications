namespace Pagarte.Worker.Domain.Enums
{
	public enum TransactionStatus
	{
		Pending,
		ChargingCard,
		CardCharged,
		SendingToCompany,
		Completed,
		Failed,
		Refunding,
		Refunded,
		RefundFailed
	}

	public enum CardType {
		Visa, 
		Mastercard, 
		Amex, 
		Other 
	}

	public enum FeeType {
		DLocal,
		Company, 
		Pagarte 
	}

	public enum CalculationType { 
		Percentage,
		FixedAmount 
	}

	public enum PaymentDetailType
	{
		ServiceAmount,
		DLocalFee,
		CompanyFee,
		PagarteFee,
		Tax
	}
}
