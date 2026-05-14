namespace ClientIdentity.Application.Abstractions;

public interface ITokenGenerator
{
    string GenerateUrlSafeToken();
}
