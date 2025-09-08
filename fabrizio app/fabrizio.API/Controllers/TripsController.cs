using fabrizio.API.Services;
using fabrizio.BLL;
using fabrizio.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace fabrizio.API.Controllers
{
	[ApiController]
	[Route("api/trips")]
	public class TripsController : ControllerBase
	{
		private readonly ITripService _tripsService;
		private readonly IJwtTokenService _jwtTokenService;

		public TripsController(ITripService tripService, IJwtTokenService jwtTokenService)
		{
			_tripsService = tripService;
			_jwtTokenService = jwtTokenService;
		}




		/// <summary>
		/// Filter all trips.
		/// </summary>
		/// <param name="skip"></param>
		/// <param name="take"></param>
		/// <param name="name"></param>
		/// <param name="destination"></param>
		/// <param name="startdate"></param>
		/// <param name="enddate"></param>
		/// <returns></returns>
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int take = 100,
												[FromQuery] string? name = null, [FromQuery] string? destination = null, 
												[FromQuery] DateTime? startdate = null, [FromQuery] DateTime? enddate = null)
		{
			var accountIdClaim = User.FindFirstValue("accountId");
			if (!int.TryParse(accountIdClaim, out var accountId) || accountId <= 0) return Unauthorized();

			var result = await _tripsService.GetAll(accountId, skip, take, name, destination, startdate, enddate);
			return Ok(result);
		}

		/// <summary>
		/// Get trip by ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id:Guid}")]
		[Authorize]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _tripsService.GetById(id);
			return Ok(result);
		}

		/// <summary>
		/// Create a new trip.
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost("")]
		[Authorize]
		public async Task<IActionResult> Create([FromBody] POSTTrip dto)
		{
			var accountIdClaim = User.FindFirstValue("accountId");
			if (!int.TryParse(accountIdClaim, out var accountId) || accountId <= 0) return Unauthorized();

			var trip = await _tripsService.Create(accountId, dto);
			return CreatedAtAction(nameof(GetById), new { id = trip.Id }, trip);
		}

		/// <summary>
		/// Update an existing trip.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPut("{id:Guid}")]
		[Authorize]
		public async Task<IActionResult> Update(Guid id, [FromBody] PUTTrip dto)
		{
			var accountIdClaim = User.FindFirstValue("accountId");
			if (!int.TryParse(accountIdClaim, out var accountId) || accountId <= 0) return Unauthorized();

			await _tripsService.Update(accountId, id, dto);
			return NoContent();
		}

		/// <summary>
		/// Delete an trip.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("{id:Guid}")]
		[Authorize]
		public async Task<IActionResult> Delete(Guid id)
		{
			var accountIdClaim = User.FindFirstValue("accountId");
			if (!int.TryParse(accountIdClaim, out var accountId) || accountId <= 0) return Unauthorized();

			await _tripsService.Delete(accountId, id);
			return NoContent();
		}

	}
}
