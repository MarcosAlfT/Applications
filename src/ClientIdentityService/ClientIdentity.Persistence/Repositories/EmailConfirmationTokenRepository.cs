using ClientIdentity.Application.Abstractions;
using ClientIdentity.Domain.Tokens;
using Microsoft.EntityFrameworkCore;

namespace ClientIdentity.Persistence.Repositories;

public sealed class EmailConfirmationTokenRepository(ClientIdentityDbContext context) : IEmailConfirmationTokenRepository
{
    public async Task AddAsync(EmailConfirmationToken token, CancellationToken cancellationToken)
    {
        await context.EmailConfirmationTokens.AddAsync(token, cancellationToken);
    }

    public Task<EmailConfirmationToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return context.EmailConfirmationTokens.FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);
    }
}
