namespace Shared.Messaging.Messages.Payment
{
    public class PaymentRequestMessage
    {
        public Guid PaymentId { get; set; }
        public Guid ServiceId { get; set; }
        public Guid CompanyId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string DLocalPaymentId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class PaymentResultMessage
    {
        public Guid PaymentId { get; set; }
        public bool Success { get; set; }
        public string? CompanyReference { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }

    public class RefundRequestMessage
    {
        public Guid PaymentId { get; set; }
        public string DLocalPaymentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public int RetryCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class StatusUpdateMessage
    {
        public Guid PaymentId { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
