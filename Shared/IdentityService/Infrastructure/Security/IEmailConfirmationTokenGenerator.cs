namespace IdentityService.Infrastructure.Security
{
	public interface IEmailConfirmationTokenGenerator
	{
		string GenerateToken();
	}
}
