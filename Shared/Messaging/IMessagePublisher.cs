namespace Shared.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, string? queueName = null) where T : class;
    }
}
