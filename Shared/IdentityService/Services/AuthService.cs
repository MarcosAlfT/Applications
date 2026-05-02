using FluentResults;
using IdentityService.Domain;
using IdentityService.Dtos.Auth;
using IdentityService.Infrastructure.Security;
using IdentityService.Interfaces;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;

namespace IdentityService.Services
{
	public class AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher,
		IEmailConfirmationTokenGenerator emailConfirmationTokenGenerator,
		//IEmailSender emailSender,
		IConfiguration _configuration) : IAuthService
	{
		private readonly IUserRepository _userRepository = userRepository;
		private readonly IPasswordHasher _passwordHasher = passwordHasher;
		private readonly IEmailConfirmationTokenGenerator _emailTokenGenerator = emailConfirmationTokenGenerator;
		//private readonly IEmailSender _emailSender = emailSender;
		private readonly IConfiguration _configuration = _configuration;


		public async Task<Result> RegisterAsync(RegisterUserRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrEmpty(request.Email) || string.IsNullOrWhiteSpace(request.Password))
			{
				return Result.Fail("Name, email, and password are required.");
			}

			if (string.IsNullOrEmpty(_configuration["Application:Url"]))
			{
				return Result.Fail("Url not found to sent the confirmation email");
			}

			Result validateExistance = await IsUsernameAndEmailAvailable(request.Username, request.Email);

			if (validateExistance.IsFailed)
			{
				return validateExistance;
			}
			
			string hashedPassword = _passwordHasher.Hash(request.Password);
			string confirmationToken = _emailTokenGenerator.GenerateToken();

			var newUser = User.CreateNew(Guid.NewGuid(), request.Username, request.Email, hashedPassword, confirmationToken);

			Result createUserResult = await  _userRepository.AddUserAsync(newUser);

			if (createUserResult.IsFailed)
			{
				return createUserResult;
			}

			// Send confirmation email

			string? applicationUrl = _configuration["Application:Url"];
			Console.WriteLine($"applicationUrl: {applicationUrl}");

			string confirmationLink = $"{applicationUrl}/api/Auth/confirm-email?token={confirmationToken}";
			string emailBody = $"Please confirm your email by clicking the link: <a href='{confirmationLink}'>Confirm Email</a>";
			Console.WriteLine($"emailBody: {emailBody}");
			//_emailSender.SendEmail(!string.IsNullOrEmpty(newUser.Email) ? newUser.Email : "", "Confirm your email", emailBody);

			return Result.Ok().WithSuccess("Registration success. Please chech your email.");
		}

		private async Task<Result> IsUsernameAndEmailAvailable(string username, string email)
		{
			Result<UserExistenceDto> existenceCheck = await _userRepository.CheckExistenceAsync(username, email);

			if (existenceCheck.IsFailed)
			{
				return existenceCheck.ToResult();
			}

			var validationDto = existenceCheck.Value;

			if (validationDto.ExistUsername && validationDto.ExistEmail)
			{
				return Result.Fail("Username and Email already exists.");
			}
			else if (validationDto.ExistUsername)
			{
				return Result.Fail("Username already exists.");
			}
			else if (validationDto.ExistEmail)
			{
				return Result.Fail("Email already exists.");
			}
			return Result.Ok();
		}

		public async Task<Result> ConfirmEmailAsync(string token)
		{
			Result<User> userResult = await _userRepository.GetUserByTokenAsync(token);

			if (userResult.IsFailed)
			{
				return userResult.ToResult(); 
			}

			User user = userResult.Value;

			var userConfirmed = user.ConfirmEmail();

			if (userConfirmed.IsFailed)
			{
				return userConfirmed;
			}

			var result = await _userRepository.UpdateEmailConfirmationStatusAsync(user);

			return result;

		}

		public async Task<Result<ClaimsPrincipal>> AuthenticateUserAndBuildPrincipalAsync(string username, string password)
		{
			Result<User> userResult = await _userRepository.GetUserByUsernameOrEmailAsync(username);

			if (userResult.IsFailed)
			{
				return userResult.ToResult<ClaimsPrincipal>();
			}

			User user = userResult.Value;

			if (user is null || string.IsNullOrEmpty(user.PasswordHash) || !_passwordHasher.Verify(password, user.PasswordHash))
			{
				return Result.Fail<ClaimsPrincipal>("Invalid username or password.");
			}

			if (!user.IsEmailConfirmed)
			{
				return Result.Fail<ClaimsPrincipal>("Email not confirmed. Please check your email for the confirmation link.");
			}

			if (!user.IsActive)
			{
				return Result.Fail<ClaimsPrincipal>("User is not active. Please contact support.");
			}

			var audiences = GetConfiguredAudiences();

			if (audiences.Length == 0)
			{
				return Result.Fail<ClaimsPrincipal>("Audience not configured.");
			}

			var claims = new List<Claim>
			{
				new (OpenIddictConstants.Claims.Subject, user.Id.ToString()),
				new (OpenIddictConstants.Claims.Email, user.Email ?? string.Empty),
                new (OpenIddictConstants.Claims.Name, user.Username ?? string.Empty),
			};

			claims.AddRange(audiences.Select(audience => new Claim(OpenIddictConstants.Claims.Audience, audience)));

			var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

			claimsPrincipal.SetAudiences(audiences);

			//claimsPrincipal.SetScopes("api");
			claimsPrincipal.SetScopes(OpenIddictConstants.Scopes.OfflineAccess, OpenIddictConstants.Scopes.Profile, "api");

			claimsPrincipal.SetResources(audiences);


			claimsPrincipal.SetDestinations(static claim => claim.Type switch
			{
				OpenIddictConstants.Claims.Audience => [OpenIddictConstants.Destinations.AccessToken],
				OpenIddictConstants.Claims.Subject => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],
				OpenIddictConstants.Claims.Email => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],
				OpenIddictConstants.Claims.Name => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],
				OpenIddictConstants.Claims.Scope => [OpenIddictConstants.Destinations.AccessToken],

				_ => Array.Empty<string>()
			});

			// Return the completed ClaimsPrincipal object, wrapped in a success Result.
			return Result.Ok(claimsPrincipal);
		}

		private string[] GetConfiguredAudiences()
		{
			var audiences = _configuration.GetSection("AuthSettings:Audiences").Get<string[]>() ?? [];

			if (audiences.Length == 0)
			{
				var audience = _configuration.GetValue<string>("AuthSettings:Audience");

				if (!string.IsNullOrWhiteSpace(audience))
				{
					audiences = [audience];
				}
			}

			return audiences
				.Where(audience => !string.IsNullOrWhiteSpace(audience))
				.Select(audience => audience.Trim())
				.Distinct(StringComparer.Ordinal)
				.ToArray();
		}
	}
}
