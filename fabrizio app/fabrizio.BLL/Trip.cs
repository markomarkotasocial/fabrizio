using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Net.NetworkInformation;
using System.Numerics;

using fabrizio.DAL;
using fabrizio.DAL.Entities;
using fabrizio.Shared.DTO;
using fabrizio.Shared.Contracts;
using fabrizio.Repository;


namespace fabrizio.BLL
{
	public interface ITripService
	{
		Task<Result<GETTripOverview>> GetTripOverview(int accountid, DateTime? date = null);
		Task<Result<GETTrip>> GetTripById(int accountid, Guid id);
		Task<Result<PagedResult<GETTripList>>> GetAllTrips(int accountid, int skip = 0, int take = 100, string? name = null, DateTime? startdate = null, DateTime? enddate = null);
		Task<Result<GETTrip>> CreateTrip(int accountid, POSTTrip dto);
		Task<Result> UpdateTrip(int accountid, Guid id, PUTTrip dto);
		Task<Result> DeleteTrip(int accountid, Guid id);


		Task<Result<GETTravelBooking>> CreateTravelBooking(int accountid, Guid tripid, POSTTravelBooking dto);
		Task<Result> UpdateTravelBooking(int accountid, Guid id, PUTTravelBooking dto);
		Task<Result> DeleteTravelBooking(int accountid, Guid tripid, Guid travelbookingid);

		Task<Result<GETAccommodationBooking>> CreateAccommodationBooking(int accountid, Guid tripid, POSTAccommodationBooking dto);
		Task<Result> UpdateAccommodationBooking(int accountid, Guid tripid, PUTAccommodationBooking dto);
		Task<Result> DeleteAccommodationBooking(int accountid, Guid tripid, Guid accommodationbookingid);

		Task<Result<GETDestination>> CreateDestination(int accountid, Guid tripid, POSTDestination dto);
		Task<Result> UpdateDestination(int accountid, Guid tripid, PUTDestination dto);
		Task<Result> DeleteDestination(int accountid, Guid tripid, Guid destinationid);
	}


	public partial class TripService : ITripService
	{
		private readonly ITripRepository _tripRepository;
		private readonly ITravelBookingRepository _travelBookingRepository;
		private readonly IAccommodationBookingRepository _accommodationBookingRepository;
		private readonly IDestinationRepository _destinationRepository;
		private readonly AppDbContext _context;

		public TripService(ITripRepository repository, ITravelBookingRepository travelBookingRepository, IAccommodationBookingRepository accommodationBookingRepository, IDestinationRepository destinationRepository, AppDbContext context)
		{
			_tripRepository = repository;
			_travelBookingRepository = travelBookingRepository;
			_accommodationBookingRepository = accommodationBookingRepository;
			_destinationRepository = destinationRepository;
			_context = context;	
		}



		public async Task<Result<GETTrip>> GetTripById(int accountid, Guid id)
		{
			#region Validate

			if (id.Equals(Guid.Empty)) throw new ArgumentException("Id is not correct.", nameof(id));
			Trip? trip = await _tripRepository.GetById(id);
			if (trip == null)
			{
				return Result<GETTrip>.Fail(new BusinessError("trip_not_found", "There is no trip with specified ID.", 404));
			}

			if (trip.AccountId != accountid)
			{
				return Result<GETTrip>.Fail(new BusinessError("forbidden", "You do not have access to this trip.", 403));
			}

			#endregion Validate

			return Result<GETTrip>.Success(new GETTrip
			{
				Id = trip.Id,
				Status = (int)trip.Status,
				Name = trip.Name,
				Notes = trip.Notes ?? string.Empty,
				StartDate = trip.StartDate,
				EndDate = trip.EndDate,
				Destinations = trip.Destinations.Select(tb => new GETDestination 
				{
					Id = tb.Id,
					Name = tb.Name,
					Order = tb.Order,
					TripId = tb.TripId,
				}),
				TravelBookings = trip.TravelBookings.Select(tb => new GETTravelBooking
				{
					Id = tb.Id,
					TripId = tb.TripId,
					Arrival = tb.Arrival,
					Carrier = tb.Carrier,
					Departure = tb.Departure,
					Reference = tb.Reference,
					Note = tb.Note,
					Destination = tb.Destination,
					Origin = tb.Origin,
					Type = (int)tb.Type
				}),
				AccommodationBookings = trip.AccommodationBookings.Select(ab => new GETAccommodationBooking
				{
					Id = ab.Id,
					TripId = ab.TripId,
					From = ab.From,
					To = ab.To,					
					Location = ab.Location, 
					Name = ab.Name, 
					Note = ab.Note, 
					Reference = ab.Reference,
					Type = (int)ab.Type
				}),
			});
		}

		public async Task<Result<PagedResult<GETTripList>>> GetAllTrips(int accountid, int skip = 0, int take = 100, string? name = null, DateTime? startdate = null,  DateTime? enddate = null)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			if (take <= 0) throw new ArgumentException("Take must be greater than zero.", nameof(take));
			
			#endregion Validate

			var query = _tripRepository.QueryAll(accountid);

			#region Filters

			if (!string.IsNullOrWhiteSpace(name))
			{
				var trimmedName = name.Trim();
				if (trimmedName.Length > 0)
					query = query.Where(t => t.Name.Contains(trimmedName));
			}

			if (startdate.HasValue)
				query = query.Where(t => t.StartDate >= startdate.Value);

			if (enddate.HasValue)
				query = query.Where(t => t.EndDate == null || t.EndDate <= enddate.Value);

			#endregion Filters

			query = query.OrderByDescending(t => t.StartDate).ThenBy(t => t.Name);

			// Paging
			var totalCount = await query.CountAsync();
			var items = await query.Skip(skip).Take(take).ToListAsync();

			// Map to DTOs
			var dtoItems = items.Select(trip => new GETTripList
			{
				Id = trip.Id,
				Status = (int)trip.Status,
				Name = trip.Name,
				Notes = trip.Notes ?? string.Empty,
				StartDate = trip.StartDate,
				EndDate = trip.EndDate,				
			});

			return Result<PagedResult<GETTripList>>.Success(new PagedResult<GETTripList>
			{
				TotalCount = totalCount,
				Items = dtoItems
			});
		}

		public async Task<Result<GETTrip>> CreateTrip(int accountid, POSTTrip dto)
		{
			#region Validate

			if(accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			ArgumentNullException.ThrowIfNull(dto, nameof(dto));

			if (string.IsNullOrWhiteSpace(dto.Name))
			{
				return Result<GETTrip>.Fail(new BusinessError("trip_name_required", "Name must be provided.", 400));
			}

			if (dto.EndDate != null)
			{
				if (dto.EndDate < dto.StartDate)
				{
					return Result<GETTrip>.Fail(new BusinessError("trip_dates_inconsistency", "End date cannot be earlier than start date.", 400));
				}
			}

			var hasOverlap = await _tripRepository.HasOverlappingTrip(accountid, dto.StartDate, dto.EndDate, excludeTripId: null);
			if (hasOverlap)
			{
				return Result<GETTrip>.Fail(new BusinessError("trip_overlap", "Trip dates overlap.", 409));
			}

			#endregion Validate

			var trip = new Trip
			{
				AccountId = accountid,
				Name = dto.Name, 
				Notes = dto.Notes, 
				StartDate = dto.StartDate, 
				EndDate = dto.EndDate
			};

			if (trip.StartDate > DateTime.UtcNow) trip.Status = TripStatus.Planned;
			else if (trip.EndDate == null || trip.EndDate >= DateTime.UtcNow) trip.Status = TripStatus.Ongoing;
			else trip.Status = TripStatus.Completed;

			_tripRepository.Add(trip);
			await _tripRepository.SaveChangesAsync();
			return Result<GETTrip>.Success(new GETTrip
			{
				Id = trip.Id, 
				Name = trip.Name, 
				Notes = trip.Notes ?? string.Empty, 
				StartDate = trip.StartDate, 
				EndDate = trip.EndDate, 
				Status = (int)trip.Status, 
				AccommodationBookings = Enumerable.Empty<GETAccommodationBooking>(), 
				TravelBookings = Enumerable.Empty<GETTravelBooking>(), 
				Destinations = Enumerable.Empty<GETDestination>()
			});
		}

		public async Task<Result> UpdateTrip(int accountid, Guid id, PUTTrip dto)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			ArgumentNullException.ThrowIfNull(dto, nameof(dto));

			if (string.IsNullOrWhiteSpace(dto.Name))
			{
				return Result.Fail(new BusinessError("trip_name_required", "Name must be provided.", 400));
			}

			if (dto.EndDate != null)
			{
				if (dto.EndDate < dto.StartDate)
				{
					return Result.Fail(new BusinessError("trip_dates_inconsistency", "End date cannot be earlier than start date.", 400));
				}
			}

			var trip = await _tripRepository.GetById(id);
			if (trip == null)
			{
				return Result.Fail(new BusinessError("trip_not_found", "There is no trip with specified ID.", 404));
			}

			if (trip.AccountId != accountid)
			{
				return Result.Fail(new BusinessError("forbidden", "You do not have access to this trip.", 403));
			}

			var hasOverlap = await _tripRepository.HasOverlappingTrip(accountid, dto.StartDate, dto.EndDate, excludeTripId: id);
			if (hasOverlap)
			{
				return Result<Trip>.Fail(new BusinessError("trip_overlap", "Trip dates overlap.", 409));
			}

			#endregion Validate

			trip.Name = dto.Name;
			trip.Notes = dto.Notes;
			trip.StartDate = dto.StartDate;
			trip.EndDate = dto.EndDate;

			if (trip.Status != TripStatus.Cancelled)
			{
				if (trip.StartDate > DateTime.UtcNow) trip.Status = TripStatus.Planned;
				else if (trip.EndDate == null || trip.EndDate >= DateTime.UtcNow) trip.Status = TripStatus.Ongoing;
				else trip.Status = TripStatus.Completed;
			}

			await _tripRepository.SaveChangesAsync();
			return Result.Success();
		}

		public async Task<Result> DeleteTrip(int accountid, Guid id)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			if (id.Equals(Guid.Empty)) throw new ArgumentException("Id is not correct.", nameof(id));

			Trip? trip = await _tripRepository.GetById(id);
			if (trip == null)
			{
				return Result.Fail(new BusinessError("trip_not_found", "There is no trip with specified ID.", 404));
			}

			if (trip.AccountId != accountid)
			{
				return Result.Fail(new BusinessError("forbidden", "You do not have access to this trip.", 403));
			}

			#endregion Validate

			_tripRepository.Delete(trip);
			await _tripRepository.SaveChangesAsync();
			return Result.Success();
		}

		public async Task<Result<GETTripOverview>> GetTripOverview(int accountid, DateTime? date = null)
		{
			var refDate = (date ?? DateTime.UtcNow).Date;

			var trips = _tripRepository.QueryAll(accountid);

			var current = trips
				.Where(t => t.StartDate <= refDate && t.EndDate >= refDate && t.Status != TripStatus.Cancelled)
				.OrderBy(t => t.StartDate)
				.FirstOrDefault();

			var previous = trips
				.Where(t => t.EndDate < refDate && t.Status != TripStatus.Cancelled)
				.OrderByDescending(t => t.EndDate)
				.FirstOrDefault();

			var next = trips
				.Where(t => t.StartDate > refDate && t.Status != TripStatus.Cancelled)
				.OrderBy(t => t.StartDate)
				.FirstOrDefault();

			return Result<GETTripOverview>.Success(new GETTripOverview
			{
				Previous = previous != null ? MapToGetTrip(previous) : null,
				Current = current != null ? MapToGetTrip(current) : null,
				Next = next != null ? MapToGetTrip(next) : null
			});
		}



		private GETTrip MapToGetTrip(Trip trip)
		{
			return new GETTrip
			{
				Id = trip.Id,
				Status = (int)trip.Status,
				Name = trip.Name,
				Notes = trip.Notes ?? string.Empty,
				StartDate = trip.StartDate,
				EndDate = trip.EndDate,

				Destinations = trip.Destinations
					.OrderBy(d => d.Order)
					.Select(d => new GETDestination
					{
						Id = d.Id,
						Name = d.Name,
						Order = d.Order, 
						TripId = d.TripId,
					}),

				TravelBookings = trip.TravelBookings.Select(tb => new GETTravelBooking
				{
					Id = tb.Id,
					TripId = tb.TripId,
					Arrival = tb.Arrival,
					Carrier = tb.Carrier,
					Departure = tb.Departure,
					Reference = tb.Reference,
					Note = tb.Note,
					Destination = tb.Destination,
					Origin = tb.Origin,
					Type = (int)tb.Type
				}),

				AccommodationBookings = trip.AccommodationBookings.Select(ab => new GETAccommodationBooking
				{
					Id = ab.Id,
					TripId = ab.TripId,
					From = ab.From,
					To = ab.To,
					Location = ab.Location,
					Name = ab.Name,
					Note = ab.Note,
					Reference = ab.Reference,
					Type = (int)ab.Type
				})
			};
		}


	}

}
