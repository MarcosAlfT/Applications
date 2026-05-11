using Pagarte.Services.Domain.Entities;
using Pagarte.Services.Domain.Enums;

namespace Pagarte.Services.Interfaces
{
	public interface IPaymentOperatorRepository
	{
		Task<PaymentOperator?> GetActiveAsync(PaymentOperatorScope scope);
		Task<PaymentOperator?> GetByCodeAsync(string code);
		Task<PaymentOperator> CreateAsync(PaymentOperator paymentOperator);
	}
}
