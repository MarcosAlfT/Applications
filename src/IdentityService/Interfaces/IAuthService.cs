using FluentResults;
using IdentityService.Dtos.Auth;
using System.Security.Claims;


namespace IdentityService.Interfaces
{
	public interface IAuthService
	{
		Task<Result> RegisterAsync(RegisterUserRequest user);
		Task<Result> ConfirmEmailAsync(string token);
		Task<Result<ClaimsPrincipal>> AuthenticateUserAndBuildPrincipalAsync(string user, string password);
	}
}
