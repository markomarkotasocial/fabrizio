using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

using fabrizio.API.Services;
using fabrizio.API.Extensions;
using fabrizio.BLL;
using fabrizio.DAL.Entities;
using fabrizio.Shared.DTO;

namespace fabrizio.API.Controllers
{
	[ApiController]
	[Route("api/trips")]
	public class TripsController : AuthorizedControllerBase
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
		/// <param name="startdate"></param>
		/// <param name="enddate"></param>
		/// <returns></returns>
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int take = 100,
												[FromQuery] string? name = null,
												[FromQuery] TripFilter filter = TripFilter.CurrentAndUpcoming)
		{
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			var result = await _tripsService.GetAllTrips(accountId, skip, take, name, filter);
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
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			var result = await _tripsService.GetTripById(accountId, id);
			return Ok(result);
		}

		/// <summary>
		/// Get current, previous and next trip overview.
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		[HttpGet("overview")]
		[Authorize]
		public async Task<IActionResult> GetOverview(DateTime? date = null)
		{
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			var result = await _tripsService.GetTripOverview(accountId, date);
			return Ok(result);
		}

		/// <summary>
		/// Create a new trip.
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost("")]
		[Authorize]
		public async Task<IActionResult> Create([FromBody] CreateTripRequest dto)
		{
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			var result = await _tripsService.CreateTrip(accountId, dto);
			if (!result.IsSuccess) return result.ToProblem();
			return Ok(result.Value);
		}

		/// <summary>
		/// Create a new travel booking for a specific trip.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost("{id:Guid}/travelbooking")]
		public async Task<IActionResult> CreateTravelBooking(Guid id, [FromBody] CreateTravelBookingRequest dto)
		{
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			var result = await _tripsService.CreateTravelBooking(accountId, id, dto);
			if (!result.IsSuccess) return result.ToProblem();
			return Ok(result.Value);
		}

		/// <summary>
		/// Create a new accommodation booking for a specific trip.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost("{id:Guid}/accommodationbooking")]
		public async Task<IActionResult> CreateAccommodationBooking(Guid id, [FromBody] CreateAccommodationBookingRequest dto)
		{
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			var result = await _tripsService.CreateAccommodationBooking(accountId, id, dto);
			if (!result.IsSuccess) return result.ToProblem();
			return Ok(result.Value);
		}

		/// <summary>
		/// Create a new destination for a specific trip.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost("{id:Guid}/destination")]
		public async Task<IActionResult> CreateDestination(Guid id, [FromBody] CreateDestinationRequest dto)
		{
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			var result = await _tripsService.CreateDestination(accountId, id, dto);
			if (!result.IsSuccess) return result.ToProblem();
			return Ok(result.Value);
		}

		/// <summary>
		/// Update an existing trip.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPut("{id:Guid}")]
		[Authorize]
		public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTripRequest dto)
		{
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			var result = await _tripsService.UpdateTrip(accountId, id, dto);
			if (!result.IsSuccess) return result.ToProblem();
			return Ok(result.Value);
		}

		/// <summary>
		/// Update an existing travel booking for a specific trip.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPut("{id:Guid}/travelbooking")]
		[Authorize]
		public async Task<IActionResult> UpdateTravelBooking(Guid id, [FromBody] UpdateTravelBookingRequest dto)
		{
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			await _tripsService.UpdateTravelBooking(accountId, id, dto);
			return NoContent();
		}

		/// <summary>
		/// Update an existing accommodation booking for a specific trip.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPut("{id:Guid}/accommodationbooking")]
		[Authorize]
		public async Task<IActionResult> UpdateAccommodationBooking(Guid id, [FromBody] UpdateAccommodationBookingRequest dto)
		{
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			await _tripsService.UpdateAccommodationBooking(accountId, id, dto);
			return NoContent();
		}

		/// <summary>
		/// Update an existing destination for a specific trip.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPut("{id:Guid}/destination")]
		[Authorize]
		public async Task<IActionResult> UpdateDestination(Guid id, [FromBody] UpdateDestinationRequest dto)
		{
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			var result = await _tripsService.UpdateDestination(accountId, id, dto);
			if (!result.IsSuccess) return result.ToProblem();
			return Ok(result.Value);
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
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			await _tripsService.DeleteTrip(accountId, id);
			return NoContent();
		}

		/// <summary>
		/// Delete an travel booking.
		/// </summary>
		/// <param name="tripid"></param>
		/// <param name="travelbookingid"></param>
		/// <returns></returns>
		[HttpDelete("{tripid:Guid}/travelbooking/{travelbookingid:Guid}")]
		[Authorize]
		public async Task<IActionResult> DeleteTravelBooking(Guid tripid, Guid travelbookingid)
		{
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			await _tripsService.DeleteTravelBooking(accountId, tripid, travelbookingid);
			return NoContent();
		}

		/// <summary>
		/// Delete an accommodation booking.
		/// </summary>
		/// <param name="tripid"></param>
		/// <param name="accommodationbookingid"></param>
		/// <returns></returns>
		[HttpDelete("{tripid:Guid}/accommodationbooking/{accommodationbookingid:Guid}")]
		[Authorize]
		public async Task<IActionResult> DeleteAccommodationBooking(Guid tripid, Guid accommodationbookingid)
		{
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			await _tripsService.DeleteAccommodationBooking(accountId, tripid, accommodationbookingid);
			return NoContent();
		}

		/// <summary>
		/// Delete an destination.
		/// </summary>
		/// <param name="tripid"></param>
		/// <param name="destinationid"></param>
		/// <returns></returns>
		[HttpDelete("{tripid:Guid}/destination/{destinationid:Guid}")]
		[Authorize]
		public async Task<IActionResult> DeleteDestination(Guid tripid, Guid destinationid)
		{
			if (!TryGetAccountId(out var accountId)) return Unauthorized();

			await _tripsService.DeleteDestination(accountId, tripid, destinationid);
			return NoContent();
		}


	}
}
