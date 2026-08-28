using fabrizio.API.Services;
using fabrizio.API.Extensions;
using fabrizio.BLL;
using fabrizio.Shared.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fabrizio.API.Controllers
{
	[ApiController]
	[Route("api/accounts")]
	public class AccountsController : AuthorizedControllerBase
	{
		private readonly IAccountService _accountService;
		private readonly IJwtTokenService _jwtTokenService;

		public AccountsController(IAccountService accountService, IJwtTokenService jwtTokenService)
		{
			_accountService = accountService;
			_jwtTokenService = jwtTokenService;
		}



		/// <summary>
		/// Login. Generates a JWT token if credentials are valid.
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromBody] LoginDto dto)
		{
			var account = await _accountService.ValidateCredentials(dto.Email, dto.Password);
			if (account == null) return Unauthorized();

			var token = _jwtTokenService.GenerateToken(account);
			return Ok(new LoginResponseDto { Token = token });
		}

		/// <summary>
		/// Filter all accounts.
		/// </summary>
		/// <param name="skip"></param>
		/// <param name="take"></param>
		/// <param name="name"></param>
		/// <param name="email"></param>
		/// <returns></returns>
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int take = 100,
												[FromQuery] string? name = null, [FromQuery] string? email = null)
		{
			var result = await _accountService.GetAll(skip, take, name, email);
			return result.ToActionResult();
		}

		/// <summary>
		/// Get account info.
		/// </summary>
		/// <returns></returns>
		[HttpGet("info")]
		[Authorize]
		public async Task<IActionResult> GetInfo()
		{
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			var result = await _accountService.GetAccountInfoById(accountId);
			return result.ToActionResult();
		}
		
		/// <summary>
		/// Update an existing account info.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPut("info")]
		[Authorize]
		public async Task<IActionResult> Update(int id, [FromBody] UpdateAccountProfileRequest dto)
		{
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			var result = await _accountService.Update(accountId, dto);
			return result.ToActionResult();
		}
		
		/// <summary>
		/// Soft-delete the authenticated account.
		/// </summary>
		/// <returns></returns>
		[HttpDelete]
		[Authorize]
		public async Task<IActionResult> Delete()
		{
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			await _accountService.Delete(accountId);
			return NoContent();
		}
	}
}
