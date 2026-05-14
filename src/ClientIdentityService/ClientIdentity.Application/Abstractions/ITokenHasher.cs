namespace ClientIdentity.Application.Abstractions;

public interface ITokenHasher
{
    string Hash(string token);
}
