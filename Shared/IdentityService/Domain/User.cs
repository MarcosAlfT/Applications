using FluentResults;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityService.Domain
{
	public class User
	{
		[Key]
		public Guid Id { get; private set; }
		public string? Username { get; private set; }
		[Required]
		public string? Email { get; private set; }
		[Required]
		public string? PasswordHash { get; private set; }
		[Required]
		public DateTime? CreationDate { get; private set; } = null;
		public DateTime? UpdateDate { get; private set; } = null;
		public string? ConfirmationToken { get; private set; }
		public DateTime? SentAt { get; private set; } = null;
		[Required]
		public bool IsEmailConfirmed { get; private set; } = false;
		public DateTime? EmailConfirmedAt { get; private set; } = null;
		[Timestamp]
		public byte[]? RowVersionStamp { get; private set; }
		[Required]
		public bool IsActive { get; private set; } = false;

		// This is the ONLY way to create a new user, ensuring it's always in a valid initial state.
		public static User CreateNew(
			Guid id,
			string userName,
			string email,
			string passwordHash,
			string confirmationToken)
		{
			// Enforce business rules here if needed
			return new User
			{
				Id = id,
				Username = userName,
				Email = email,
				PasswordHash = passwordHash,
				ConfirmationToken = confirmationToken,
				CreationDate = DateTime.UtcNow,
				IsEmailConfirmed = false, // Always starts as not confirmed
				IsActive = false         // User is not active until email is confirmed
			};
		}

		// --- BUSINESS LOGIC METHOD ---
		public Result ConfirmEmail()
		{
			if (IsEmailConfirmed)
			{
				return Result.Fail("This email has already been confirmed");
			}

			IsEmailConfirmed = true;
			IsActive = true;
			ConfirmationToken = null;
			UpdateDate = DateTime.UtcNow;
			EmailConfirmedAt = DateTime.UtcNow;

			return Result.Ok();
		}
	}
}
