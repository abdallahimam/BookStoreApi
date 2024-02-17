using BookStoreApi.Data;
using BookStoreApi.Helpers;
using BookStoreApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStoreApi.Repositories
{
    public class AccountService : IAccountService
	{

		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;

        public AccountService(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
        }

		public async Task<AuthModel> RegisterAsync(RegisterModel model)
		{
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Message = "Username is already registered!" };

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                Fullname = model.Fullname
            };

			var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new AuthModel { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "User");

            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthModel
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsSuccessed = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName
            };
        }

		public async Task<AuthModel> LoginAsync(LoginModel loginModel)
		{
            var authModel = new AuthModel();

            var userName = loginModel.Email;

            if (userName.IndexOf('@') > -1)
            {
                var userCheck = await _userManager.FindByEmailAsync(loginModel.Email);
                if (userCheck is null)
				{
                    authModel.Message = "Invalid username or email";
					return authModel;
				}

                userName = userCheck.UserName;
            }

            var result = await _signInManager.PasswordSignInAsync(userName, loginModel.Password, false, false);
			// Check if username is exists
			if (!result.Succeeded)
			{
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var user = await _userManager.FindByEmailAsync(loginModel.Email);

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsSuccessed = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();

            return authModel;
        }

        public async Task<AuthModel> AddUserToRole(AddUserToRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            var authModel = new AuthModel { IsSuccessed = false };

            if (user is null)
            {
                authModel.Message = "Invalid User";
                return authModel;
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                authModel.Message = "Role not exists";
                return authModel;
            }

            if (await _userManager.IsInRoleAsync(user, model.Role))
            {
                authModel.Message = "User already assigned to this role";
                return authModel;
            }

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            if (!result.Succeeded)
            {
                authModel.Message = "Someting went wrong";
                return authModel;
            }

            authModel.IsSuccessed = true;
            authModel.Message = "User successfully assigned to the role";
            authModel.Username = user.Fullname;
            authModel.Roles = (await _userManager.GetRolesAsync(user)).ToList();

            return authModel;
        }


        public async Task<AuthModel> RemoveUserFromRole(AddUserToRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            var authModel = new AuthModel { IsSuccessed = false };

            if (user is null)
            {
                authModel.Message = "Invalid User";
                return authModel;
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                authModel.Message = "Role not exists";
                return authModel;
            }

            if (!await _userManager.IsInRoleAsync(user, model.Role))
            {
                authModel.Message = "User not already assigned to this role";
                return authModel;
            }

            var result = await _userManager.RemoveFromRoleAsync(user, model.Role);

            if (!result.Succeeded)
            {
                authModel.Message = "Someting went wrong";
                return authModel;
            }

            authModel.IsSuccessed = true;
            authModel.Message = "User successfully removed from the role";
            authModel.Username = user.Fullname;
            authModel.Roles = (await _userManager.GetRolesAsync(user)).ToList();

            return authModel;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}
