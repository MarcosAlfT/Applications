using ClientIdentity.Application.Abstractions;
using ClientIdentity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ClientIdentity.Persistence.Repositories;

public sealed class UserRepository(ClientIdentityDbContext context) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await context.Users.AddAsync(user, cancellationToken);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return context.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public Task<User?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return context.Users.FirstOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public Task<bool> ExistsByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return context.Users.AnyAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);
    }
}
