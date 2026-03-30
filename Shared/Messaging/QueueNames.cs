namespace Shared.Messaging
{
    public static class QueueNames
    {
        public static class Exchanges
        {
            public const string Payments = "pagarte.payments";
            public const string Notifications = "pagarte.notifications";
            public const string Status = "pagarte.status";
        }

        public static class Queues
        {
            public const string PaymentRequest = "payment.request";
            public const string PaymentResult = "payment.result";
            public const string RefundRequest = "refund.request";
            public const string EmailSend = "email.send";
            public const string AlertCreate = "alert.create";
            public const string StatusUpdate = "status.update";
        }

        public static class DeadLetterQueues
        {
            public const string PaymentRequest = "payment.request.dlq";
            public const string RefundRequest = "refund.request.dlq";
            public const string EmailSend = "email.send.dlq";
        }
    }
}
