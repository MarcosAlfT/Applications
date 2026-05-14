namespace ClientIdentity.Application.Abstractions;

public interface ICurrentActorProvider
{
    string GetActorId();
    string? GetIpAddress();
    string? GetUserAgent();
}
