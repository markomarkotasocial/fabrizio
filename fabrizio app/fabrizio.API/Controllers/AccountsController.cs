using fabrizio.API.Services;
using fabrizio.BLL;
using fabrizio.Shared.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace fabrizio.API.Controllers
{
	[ApiController]
	[Route("api/accounts")]
	public class AccountsController : ControllerBase
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
			return Ok(new { Token = token });
		}

		/// <summary>
		/// Google login. Generates a JWT token if credentials are valid.
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		[HttpPost("google-login")]
		[AllowAnonymous]
		public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
		{
			throw new NotImplementedException();


			//// Validate Google token
			//var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken);

			//// Check if user exists
			//var account = await _accountService.GetByEmailAsync(payload.Email);
			//if (account == null)
			//{
			//	account = await _accountService.CreateFromGoogleAsync(payload.Email, payload.Name);
			//}

			//// Issue your JWT
			//var token = _jwtTokenService.GenerateToken(account);
			//return Ok(new { Token = token });
		}




		/// <summary>
		/// Create a new account.
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost("register")]
		[AllowAnonymous]
		public async Task<IActionResult> Create([FromBody] CreateAccountRequest dto)
		{

			throw new NotImplementedException();

			//var account = await _accountService.Create(dto);
			//return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
		}

		/// <summary>
		/// Activate an account.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPost("activate")]
		[AllowAnonymous]
		public async Task<IActionResult> Activate([FromBody] ActivateAccountDto dto)
		{
			await _accountService.Activate(dto.Token);
			return NoContent();
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
			return Ok(result);
		}

		/// <summary>
		/// Get account info.
		/// </summary>
		/// <returns></returns>
		[HttpGet("info")]
		[Authorize]
		public async Task<IActionResult> GetInfo()
		{
			var accountIdClaim = User.FindFirstValue("accountId");
			if (!int.TryParse(accountIdClaim, out var accountId) || accountId <= 0) return Unauthorized();

			var result = await _accountService.GetAccountInfoById(accountId);
			return Ok(result);
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
			var accountIdClaim = User.FindFirstValue("accountId");
			if (!int.TryParse(accountIdClaim, out var accountId) || accountId <= 0) return Unauthorized();

			await _accountService.Update(accountId, dto);
			return NoContent();
		}
		
		/// <summary>
		/// Delete an account.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("{id:int}")]
		[Authorize]
		public async Task<IActionResult> Delete(int id)
		{
			await _accountService.Delete(id);
			return NoContent();
		}
	}
}
