using System.Security.Cryptography;

namespace IdentityService.Infrastructure.Security
{
	public class EmailConfirmationTokenGenerator : IEmailConfirmationTokenGenerator
	{
		public string GenerateToken()
		{
			// Generate 32 bytes of random data, which is a good length for a token.
			var randomBytes = RandomNumberGenerator.GetBytes(32);
			// Convert the bytes to a URL-safe Base64 string.
			return Convert.ToBase64String(randomBytes)
				.Replace('+', '-')
				.Replace('/', '_');
		}
	}
}
