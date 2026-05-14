using ClientIdentity.Application.Policies;

namespace ClientIdentity.Application.PasswordValidation;

public interface IPasswordRule
{
    string? Validate(string password, string email, PasswordPolicy policy);
}
