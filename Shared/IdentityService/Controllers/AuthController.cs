using FluentResults;
using Shared.Responses;
using IdentityService.Dtos.Auth;
using IdentityService.Interfaces;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace IdentityService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController(IAuthService authService) : Controller
	{
		private readonly IAuthService _authService = authService;

		[HttpPost("register")]
		[ProducesResponseType(typeof(ApiResponse<string>), 200)]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public async Task<ActionResult<ApiResponse>> RegisterAsync(RegisterUserRequest newUser)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse<string>.CreateFailure("Input data error"));
			}

			var registerResponse = await _authService.RegisterAsync(newUser);

			if (registerResponse.IsFailed)
			{
				return BadRequest(ApiResponse.CreateFailure(registerResponse.Errors[0].Message));
			}

			return Ok(ApiResponse.CreateSuccess(registerResponse.Successes[0].Message));
		}

		[HttpGet("confirm-email")]
		[ProducesResponseType(typeof(ApiResponse), 200)]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		public async Task<ActionResult<ApiResponse>> ConfirmEmailAsync([FromQuery] string token)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.CreateFailure("Input data error."));
			}
			if (string.IsNullOrEmpty(token))
			{
				return BadRequest(ApiResponse.CreateFailure("Missing confirmation token."));
			}

			var confirmResponse = await _authService.ConfirmEmailAsync(token);

			if (confirmResponse.IsFailed)
			{
				return BadRequest(ApiResponse.CreateFailure(confirmResponse.Errors[0].Message));
			}

			return Ok(ApiResponse.CreateSuccess(confirmResponse.Successes[0].Message));
		}

		[HttpPost("~/connect/token")]
		public async Task<IActionResult> Exchange()
		{
			var request = HttpContext.GetOpenIddictServerRequest();

			if (request == null)
			{
				return BadRequest(ApiResponse.CreateFailure("Invalid OpenIddict request."));
			}

			// Validate that Username and Password are not null or empty
			if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
			{
				return BadRequest(ApiResponse.CreateFailure("Username cannot be null or empty."));
			}

			switch (request.GrantType)
			{
				case OpenIddictConstants.GrantTypes.Password:

					var principalResult = await _authService.AuthenticateUserAndBuildPrincipalAsync(request.Username, request.Password);

					if (principalResult.IsFailed)
						return ReturnForbidInvalidGrant(principalResult.Errors);

					return SignIn(principalResult.Value, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

				case OpenIddictConstants.GrantTypes.RefreshToken:
					return Ok("Refresh token flow handled."); // Placeholder

				case OpenIddictConstants.GrantTypes.ClientCredentials:
					return Ok("Client credentials flow handled."); // Placeholder

				// Add other grant types as needed (e.g., AuthorizationCode)

				default:
					// Handle unsupported grant types
					return Forbid(
						authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
						properties: new AuthenticationProperties(new Dictionary<string, string?>
						{
							[OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.UnsupportedGrantType,
							[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The specified grant type is not supported by this endpoint."
						}));
			}
		}

		private ForbidResult ReturnForbidInvalidGrant(IEnumerable<IError> errors)
		{
			string errorDescription = string.Join(" ", errors.Select(e => e.Message));

			return Forbid(
					authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
					properties: new AuthenticationProperties(new Dictionary<string, string?>
					{
						[OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
						[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = errorDescription
				}));
		}
	}
}
