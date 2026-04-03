//using Api.Contrats.Shared.Response;
//using IdentityService.Dtos.Auth;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//namespace IdentityService.Controllers
//{
//	[ApiController]
//	[Route("api/[controller]")]
//	public class UsersController : Controller
//	{
//		//private readonly IUserService _userService;

//		public UsersController()
//		{
//			//_userService = useService;
//		}
//		[Authorize]
//		public IActionResult GetCurrentUserProfile()
//		{
//			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

//			if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
//			{
//				return Unauthorized();
//			}

//			//var userProfileResult = _userService.GetUserProfileById(Guid.Parse(userIdString));

//			//if (userProfileResult.isFailed)
//			//{
//			//	return NotFound(ApiResponse.CreateFailure(userProfileResult.Error.First().Message));
//			//}

//			//return Ok(ApiResponse<UserDto>.CreateSuccess(userProfileResult.Value));

//			return Ok(ApiResponse.CreateSuccess("This is a placeholder for user profile data."));
//		}
//	}
//}
