using System.Security.Cryptography;
using ClientIdentity.Application.Abstractions;

namespace ClientIdentity.Infrastructure.Security;

public sealed class SecureTokenGenerator : ITokenGenerator
{
    public string GenerateUrlSafeToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}
