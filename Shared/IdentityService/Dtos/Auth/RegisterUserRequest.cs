using System.ComponentModel.DataAnnotations;

namespace IdentityService.Dtos.Auth
{
	public class RegisterUserRequest
	{
		public string Username { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;

		[EmailAddress(ErrorMessage = "Invalid email format.")]
		public string Email { get; set; } = string.Empty;
	}
}
