using ClientIdentity.Domain.Tokens;

namespace ClientIdentity.Application.Abstractions;

public interface IEmailConfirmationTokenRepository
{
    Task AddAsync(EmailConfirmationToken token, CancellationToken cancellationToken);
    Task<EmailConfirmationToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);
}
