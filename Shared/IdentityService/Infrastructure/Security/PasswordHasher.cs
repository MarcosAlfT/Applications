namespace IdentityService.Infrastructure.Security
{
	public class PasswordHasher: IPasswordHasher
	{
		// This method wraps the BCrypt library call.
		public string Hash(string password)
		{
			return BCrypt.Net.BCrypt.HashPassword(password);
		}

		// This method wraps the verification call.
		public bool Verify(string password, string hashedPassword)
		{
			return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
		}
	}
}
