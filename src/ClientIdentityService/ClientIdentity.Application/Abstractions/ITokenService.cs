using System.Security.Claims;
using ClientIdentity.Domain.Users;

namespace ClientIdentity.Application.Abstractions;

public interface ITokenService
{
    ClaimsPrincipal CreatePrincipal(User user);
}
