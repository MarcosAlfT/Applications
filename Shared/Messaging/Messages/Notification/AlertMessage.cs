namespace Shared.Messaging.Messages.Notification
{
    public class AlertMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? PaymentId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Severity { get; set; } = "High";   // Low, Medium, High, Critical
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
