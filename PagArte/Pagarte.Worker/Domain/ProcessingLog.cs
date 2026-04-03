namespace Pagarte.Worker.Domain
{
    public enum MessageStatus
    {
        Processing,
        Completed,
        Failed,
        DeadLetter
    }

    public enum MessageType
    {
        PaymentRequest,
        PaymentResult,
        RefundRequest,
        EmailSend,
        StatusUpdate
    }

    public class ProcessingLog
    {
        public Guid Id { get; set; }
        public MessageType MessageType { get; set; }
        public string MessagePayload { get; set; } = string.Empty;  // full JSON
        public MessageStatus Status { get; set; } = MessageStatus.Processing;
        public Guid? PaymentId { get; set; }
        public int RetryCount { get; set; } = 0;
        public DateTime? NextRetryAt { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }

        public static ProcessingLog Create(MessageType type, string payload, Guid? paymentId = null)
        {
            return new ProcessingLog
            {
                Id = Guid.NewGuid(),
                MessageType = type,
                MessagePayload = payload,
                PaymentId = paymentId,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Complete()
        {
            Status = MessageStatus.Completed;
            ProcessedAt = DateTime.UtcNow;
        }

        public void Fail(string errorMessage)
        {
            Status = MessageStatus.Failed;
            ErrorMessage = errorMessage;
            ProcessedAt = DateTime.UtcNow;
        }

        public void MoveToDeadLetter(string errorMessage)
        {
            Status = MessageStatus.DeadLetter;
            ErrorMessage = errorMessage;
            ProcessedAt = DateTime.UtcNow;
        }

        public void IncrementRetry()
        {
            RetryCount++;
            NextRetryAt = DateTime.UtcNow.AddMinutes(5);
        }
    }
}
