using ClientIdentity.Application.Policies;

namespace ClientIdentity.Application.Abstractions;

public interface IPolicyProvider
{
    PasswordPolicy GetPasswordPolicy();
    TokenPolicy GetTokenPolicy();
    LockoutPolicy GetLockoutPolicy();
    EmailConfirmationPolicy GetEmailConfirmationPolicy();
    PasswordResetPolicy GetPasswordResetPolicy();
    PasskeyPolicy GetPasskeyPolicy();
}
