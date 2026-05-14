using ClientIdentity.Application.Abstractions;
using ClientIdentity.Domain.Tokens;
using Microsoft.EntityFrameworkCore;

namespace ClientIdentity.Persistence.Repositories;

public sealed class PasswordResetTokenRepository(ClientIdentityDbContext context) : IPasswordResetTokenRepository
{
    public async Task AddAsync(PasswordResetToken token, CancellationToken cancellationToken)
    {
        await context.PasswordResetTokens.AddAsync(token, cancellationToken);
    }

    public Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return context.PasswordResetTokens.FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);
    }
}
