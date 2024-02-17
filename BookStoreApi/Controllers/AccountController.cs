using BookStoreApi.Errors;
using BookStoreApi.Models;
using BookStoreApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{

		private readonly IAccountService _accountRepository;

		public AccountController(IAccountService accountRepository)
		{
			_accountRepository = accountRepository;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
		{
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountRepository.RegisterAsync(registerModel);

            if (!result.IsSuccessed)
                return BadRequest(new ApiExceptionError(400, result.Message));

            return Ok(result);
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _accountRepository.LoginAsync(loginModel);

            if (!result.IsSuccessed)
                return BadRequest(new ApiExceptionError(400, result.Message));

            return Ok(result);
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("addUserToRole")]
		public async Task<IActionResult> AddUserToRole([FromBody] AddUserToRoleModel model)
		{
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountRepository.AddUserToRole(model);

            if (!result.IsSuccessed)
                return BadRequest(new ApiExceptionError(400, result.Message));

            return Ok(result);
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("removeUserFromRole")]
        public async Task<IActionResult> RemoveUserFromRole([FromBody] AddUserToRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountRepository.RemoveUserFromRole(model);

            if (!result.IsSuccessed)
                return BadRequest(new ApiExceptionError(400, result.Message));

            return Ok(result);
        }
    }
}
