using ClientIdentity.Application.Abstractions;

namespace ClientIdentity.Infrastructure.Security;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
