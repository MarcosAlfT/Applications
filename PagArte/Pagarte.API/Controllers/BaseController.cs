using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using Infrastructure.Responses;

namespace Pagarte.API.Controllers
{
    public class BaseController : ControllerBase
    {
        protected string? GetClientId()
        {
            return User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        }

        protected IActionResult? ValidateClientId()
        {
            var clientId = GetClientId();
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized(ApiResponse.CreateFailure("Invalid or missing token."));
            return null;
        }
    }
}
