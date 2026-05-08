using IdentityService.Domain;
using FluentResults;
using IdentityService.Dtos.Auth;

namespace IdentityService.Interfaces
{
	public interface IUserRepository
	{
		Task<Result> AddUserAsync(User user);
		Task<Result<UserExistenceDto>> CheckExistenceAsync(string userName, string email);
		Task<Result> UpdateEmailConfirmationStatusAsync(User user);
		Task<Result<User>> GetUserByTokenAsync(string token);
		Task<Result<User>> GetUserByUsernameOrEmailAsync(string userEmailOrUsername);

		//Result<User> GetUserById(Guid userId);
		//Result Update(User user);
		//Result Delete(Guid userId);
		//Result<string> LoginAsync(LoginRequest user);
		//Result<User> GetUserByUsernameOrEmailAsync(string email);
		//Result<User> GetUserByUsername(string username);
		//Result<User> GetUserByUsernameOrEmailAsync(string email, string username);
		//Result<User> GetUserByEmailOrUsernameAndPassword(string email, string username, string password);
		//Result<User> GetUserByEmailAndPassword(string email, string password);
		//Result<User> GetUserByUsernameAndPassword(string username, string password);
		//Result<User> GetUserByIdAndPassword(Guid userId, string password);
		//Result<User> GetUserByIdAndPassword(Guid userId, string password, byte[] rowVersionStamp);
		//Result<User> GetUserByIdAndPassword(Guid userId, string password, byte[] rowVersionStamp, bool isActiveCheck = true);
		//Result<User> GetUserByIdAndPassword(Guid userId, string password, byte[] rowVersionStamp, bool isActiveCheck = true, bool isEmailConfirmedCheck = true);
		//Result<User> GetUserByIdAndPassword(Guid userId, string password, byte[] rowVersionStamp, bool isActiveCheck = true, bool isEmailConfirmedCheck = true, bool isTokenCheck = true);

	}
}
