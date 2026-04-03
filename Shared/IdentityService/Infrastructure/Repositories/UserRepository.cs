using FluentResults;
using IdentityService.Domain;
using IdentityService.Dtos.Auth;
using IdentityService.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace IdentityService.Infrastructure.Repositories
{
	public class UserRepository(IdentityDbContext context) : IUserRepository
	{
		private readonly IdentityDbContext _context = context;

		/// <summary>
		/// Creates a new user in the database.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public async Task<Result> AddUserAsync(User user)
		{
			try
			{
				_context.ChangeTracker.AutoDetectChangesEnabled = false;
				
				await _context.Users.AddAsync(user);
				await _context.SaveChangesAsync();
				_context.ChangeTracker.AutoDetectChangesEnabled = true;

				return Result.Ok().WithSuccess("User was created successfully");
			}
			catch (DbUpdateException ex)
			{
				return Result.Fail($"Database error: { ex.Message}");
			}
			catch (Exception ex)
			{
				return Result.Fail($"An unexpected error occured: {ex.Message}");
			}
			finally
			{
				_context.ChangeTracker.AutoDetectChangesEnabled = true;
			}
		}

		/// <summary>
		/// Checks if a user with the given username or email already exists in the database.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="email"></param>
		/// <returns></returns>
		public async Task<Result<UserExistenceDto>> CheckExistenceAsync(string userName, string email)
		{
			try
			{
				bool userExists = await _context.Users.AsNoTracking()
					.AnyAsync(u => u.Username == userName);
				bool emailExists = await _context.Users.AsNoTracking()
					.AnyAsync(u => u.Email == email);

				var existenceResult = new UserExistenceDto
				{
					ExistUsername = userExists,
					ExistEmail = emailExists,
				};

				return Result.Ok(existenceResult);
			}
			catch (Exception ex)
			{
				return Result.Fail<UserExistenceDto>($"An unexpected error occurred: {ex.Message}");
			}
		}

		/// <summary>
		/// Retrieves a user from the database by their authentication token.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task<Result<User>> GetUserByTokenAsync(string token)
		{
			try
			{
				var user = await _context.Users
					.AsNoTracking()
					.FirstOrDefaultAsync(u => u.ConfirmationToken == token);

				if (user == null)
					return Result.Fail<User>($"User with token '{token}' not found.");

				return Result.Ok(user);
			}
			catch (Exception ex)
			{
				return Result.Fail(ex.Message);
			}
		}

		public async Task<Result<User>> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
		{
			try
			{
				var user = await _context.Users
					.AsNoTracking()
					.FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);

				if (user == null)
					return Result.Fail<User>($"User with username or email '{usernameOrEmail}' not found.");

				return Result.Ok(user);
			}
			catch (Exception ex)
			{
				return Result.Fail(ex.Message);
			}
		}

		/// <summary>
		/// Updates an existing user in the database.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public async Task<Result> UpdateEmailConfirmationStatusAsync(User user)
		{
			try
			{
				int rowsAffected = await _context.Users
					.AsNoTracking()
					.Where(u => u.Id == user.Id)
					.ExecuteUpdateAsync(updates => updates
						.SetProperty(u => u.IsEmailConfirmed, user.IsEmailConfirmed)
						.SetProperty(u => u.ConfirmationToken, (string?)null)
						.SetProperty(u => u.IsActive, user.IsActive)
						.SetProperty(u => u.EmailConfirmedAt, user.EmailConfirmedAt)
						.SetProperty(u => u.UpdateDate, user.UpdateDate)
						);

				if (rowsAffected == 0)
				{
					return Result.Fail("User could not be found to update.");
				}

				//return Result.Ok();
				return Result.Ok().WithSuccess("Email confirmed successfully. You can now log in.");
			}
			catch (SqlException ex)
			{
				return Result.Fail(ex.Message);
			}
		}
	}
}
