namespace Pagarte.API.Constants
{
    public static class Messages
    {
        public static class CreditCard
        {
            public const string NotFound = "Credit card not found.";
            public const string Created = "Credit card registered successfully.";
            public const string Updated = "Credit card updated successfully.";
            public const string Deleted = "Credit card removed successfully.";
            public const string DLocalError = "Failed to register card with payment provider.";
            public const string AlreadyDefault = "A default card already exists.";
        }

        public static class Payment
        {
            public const string NotFound = "Payment not found.";
            public const string Created = "Payment initiated successfully.";
            public const string ServiceNotFound = "Service not found or inactive.";
            public const string CardNotFound = "Credit card not found for this client.";
            public const string DLocalError = "Failed to process payment with payment provider.";
            public const string CompanyError = "Failed to process payment with service provider.";
            public const string Refunding = "Payment failed, initiating refund.";
        }

        public static class Auth
        {
            public const string InvalidToken = "Invalid or missing token.";
            public const string Unauthorized = "Unauthorized access.";
        }
    }
}
