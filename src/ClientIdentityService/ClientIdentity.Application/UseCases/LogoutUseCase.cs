using ClientIdentity.Application.Abstractions;
using FluentResults;

namespace ClientIdentity.Application.UseCases;

public sealed class LogoutUseCase(
    IRefreshTokenRepository refreshTokens,
    ITokenHasher tokenHasher,
    ICurrentActorProvider actorProvider,
    IClock clock,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> ExecuteAsync(LogoutRequest request, CancellationToken cancellationToken)
    {
        var token = await refreshTokens.GetByTokenHashAsync(tokenHasher.Hash(request.RefreshToken), cancellationToken);
        if (token is null)
        {
            return Result.Ok();
        }

        token.Revoke(clock.UtcNow, actorProvider.GetActorId(), actorProvider.GetIpAddress());
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok().WithSuccess("Logged out successfully.");
    }
}
