namespace Pagarte.Connections.PaymentOperators
{
	public interface IPaymentOperatorAdapterFactory
	{
		IPaymentOperatorAdapter GetRequiredAdapter(string providerCode);
	}
}
