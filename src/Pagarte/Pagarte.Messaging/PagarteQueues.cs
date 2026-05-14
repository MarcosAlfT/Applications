namespace Pagarte.Messaging
{
    /// <summary>
    /// All RabbitMQ exchange and queue name constants for Pagarte.
    /// Used by Worker (publisher) and Engine (consumer).
    /// </summary>
    public static class PagarteQueues
    {
        public static class Exchanges
        {
            public const string Payments = "pagarte.payments";
            public const string Notifications = "pagarte.notifications";
        }

        public static class Queues
        {
            // Published by Worker, consumed by Engine
            public const string PaymentRequest = "payment.request";
            public const string EmailSend = "email.send";

            // Published by Engine internally
            public const string RefundRequest = "refund.request";
            public const string AlertCreate = "alert.create";
        }

        public static class DeadLetterQueues
        {
            public const string PaymentRequest = "payment.request.dlq";
            public const string RefundRequest = "refund.request.dlq";
            public const string EmailSend = "email.send.dlq";
        }
    }
}
