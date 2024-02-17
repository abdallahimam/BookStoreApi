using BookStoreApi.Helpers;
using BookStoreApi.Models;
using Microsoft.AspNetCore.Identity;

namespace BookStoreApi.Repositories
{
	public interface IAccountService
	{
		Task<AuthModel> RegisterAsync(RegisterModel registerModel);
		Task<AuthModel> LoginAsync(LoginModel loginModel);
		Task<AuthModel> AddUserToRole(AddUserToRoleModel model);
        Task<AuthModel> RemoveUserFromRole(AddUserToRoleModel model);
    }
}
