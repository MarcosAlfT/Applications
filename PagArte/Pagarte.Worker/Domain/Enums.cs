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
<<<<<<< HEAD
		PaymentOperator,
=======
		DLocal,
>>>>>>> origin/main
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
<<<<<<< HEAD
		PaymentOperatorFee,
=======
		DLocalFee,
>>>>>>> origin/main
		CompanyFee,
		PagarteFee,
		Tax
	}
}
