using fabrizio.DAL;
using fabrizio.DAL.Entities;
using fabrizio.DTO;
using fabrizio.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Net.NetworkInformation;
using System.Numerics;


namespace fabrizio.BLL
{
	public interface ITripService
	{
		Task<DTO.GETTripOverview> GetTripOverview(int accountid, DateTime? date = null);
		Task<DTO.GETTrip> GetTripById(Guid id);
		Task<PagedResult<DTO.GETTrip>> GetAllTrips(int accountid, int skip = 0, int take = 100, string? name = null, DateTime? startdate = null, DateTime? enddate = null);
		Task<Trip> CreateTrip(int accountid, POSTTrip dto);
		Task UpdateTrip(int accountid, Guid id, PUTTrip dto);
		Task DeleteTrip(int accountid, Guid id);


		Task<TravelBooking> CreateTravelBooking(int accountid, Guid tripid, POSTTravelBooking dto);
		Task UpdateTravelBooking(int accountid, Guid id, PUTTravelBooking dto);
		Task DeleteTravelBooking(int accountid, Guid tripid, Guid travelbookingid);

		Task<AccommodationBooking> CreateAccommodationBooking(int accountid, Guid tripid, POSTAccommodationBooking dto);
		Task UpdateAccommodationBooking(int accountid, Guid tripid, PUTAccommodationBooking dto);
		Task DeleteAccommodationBooking(int accountid, Guid tripid, Guid accommodationbookingid);

		Task<Result<Destination>> CreateDestination(int accountid, Guid tripid, POSTDestination dto);
		Task<Result> UpdateDestination(int accountid, Guid tripid, PUTDestination dto);
		Task DeleteDestination(int accountid, Guid tripid, Guid destinationid);
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



		public async Task<DTO.GETTrip> GetTripById(Guid id)
		{
			#region Validate

			if (id.Equals(Guid.Empty)) throw new ArgumentException("Id is not correct.", nameof(id));
			Trip? trip = await _tripRepository.GetById(id);
			if (trip == null) throw new KeyNotFoundException("There is no trip with specified ID!");

			#endregion Validate

			return new DTO.GETTrip
			{
				Id = trip.Id,
				Status = (int)trip.Status,
				Name = trip.Name,
				Notes = trip.Notes ?? string.Empty,
				StartDate = trip.StartDate,
				EndDate = trip.EndDate,
				Destinations = trip.Destinations.Select(tb => new DTO.GETDestination 
				{
					Id = tb.Id,
					Name = tb.Name,
					Order = tb.Order,
					TripId = tb.TripId,
				}),
				TravelBookings = trip.TravelBookings.Select(tb => new DTO.GETTravelBooking
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
				AccommodationBookings = trip.AccommodationBookings.Select(ab => new DTO.GETAccommodationBooking
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
			};
		}

		public async Task<PagedResult<DTO.GETTrip>> GetAllTrips(int accountid, int skip = 0, int take = 100, string? name = null, DateTime? startdate = null,  DateTime? enddate = null)
		{
			#region Validate

			if (accountid < 0)
				throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));

			if (take <= 0)
				throw new ArgumentException("Take must be greater than zero.", nameof(take));
			
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
				query = query.Where(t => t.EndDate <= enddate.Value);

			#endregion Filters

			// Paging
			var totalCount = await query.CountAsync();
			var items = await query.Skip(skip).Take(take).ToListAsync();

			// Map to DTOs
			var dtoItems = items.Select(trip => new DTO.GETTrip
			{
				Id = trip.Id,
				Status = (int)trip.Status,
				Name = trip.Name,
				Notes = trip.Notes ?? string.Empty,
				StartDate = trip.StartDate,
				EndDate = trip.EndDate,
				Destinations = trip.Destinations.Select(tb => new DTO.GETDestination
				{
					Id = tb.Id,
					Name = tb.Name,
					Order = tb.Order,
					TripId = tb.TripId,
				}),
				TravelBookings = trip.TravelBookings.Select(tb => new DTO.GETTravelBooking
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
				AccommodationBookings = trip.AccommodationBookings.Select(ab => new DTO.GETAccommodationBooking
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
			});

			return new PagedResult<DTO.GETTrip>
			{
				TotalCount = totalCount,
				Items = dtoItems
			};
		}

		public async Task<Trip> CreateTrip(int accountid, POSTTrip dto)
		{
			#region Validate

			if(accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));

			ArgumentNullException.ThrowIfNull(dto, nameof(dto));

			if (string.IsNullOrWhiteSpace(dto.Name))
				throw new ArgumentException("Name must be provided.", nameof(dto.Name));

			if (dto.EndDate != null)
			{
				if (dto.EndDate < dto.StartDate)
					throw new ArgumentException("End date cannot be earlier than start date.", nameof(dto.EndDate));
			}

			var hasOverlap = await _tripRepository.HasOverlappingTrip(accountid, dto.StartDate, dto.EndDate, excludeTripId: null);
			if (hasOverlap) throw new ArgumentException("Trip dates overlap with an existing trip.");

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
			return trip;
		}

		public async Task UpdateTrip(int accountid, Guid id, PUTTrip dto)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));

			ArgumentNullException.ThrowIfNull(dto, nameof(dto));

			if (string.IsNullOrWhiteSpace(dto.Name))
				throw new ArgumentException("Name must be provided.", nameof(dto.Name));

			if (dto.EndDate != null)
			{
				if (dto.EndDate < dto.StartDate)
					throw new ArgumentException("End date cannot be earlier than start date.", nameof(dto.EndDate));
			}

			var trip = await _tripRepository.GetById(id);
			if (trip == null)
				throw new KeyNotFoundException("There is no trip with the specified ID.");

			var hasOverlap = await _tripRepository.HasOverlappingTrip(accountid, dto.StartDate, dto.EndDate, excludeTripId: id);
			if (hasOverlap) throw new ArgumentException("Trip dates overlap with an existing trip.");

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
		}

		public async Task DeleteTrip(int accountid, Guid id)
		{
			#region Validate

			if (id.Equals(Guid.Empty)) throw new ArgumentException("Id is not correct.", nameof(id));
			Trip? trip = await _tripRepository.GetById(id);
			if (trip == null) throw new KeyNotFoundException("There is no trip with specified ID!");

			#endregion Validate

			_tripRepository.Delete(trip);
			await _tripRepository.SaveChangesAsync();
		}




		public async Task<DTO.GETTripOverview> GetTripOverview(int accountid, DateTime? date = null)
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

			return new DTO.GETTripOverview
			{
				Previous = previous != null ? MapToGetTrip(previous) : null,
				Current = current != null ? MapToGetTrip(current) : null,
				Next = next != null ? MapToGetTrip(next) : null
			};
		}

		private DTO.GETTrip MapToGetTrip(Trip trip)
		{
			return new DTO.GETTrip
			{
				Id = trip.Id,
				Status = (int)trip.Status,
				Name = trip.Name,
				Notes = trip.Notes ?? string.Empty,
				StartDate = trip.StartDate,
				EndDate = trip.EndDate,

				Destinations = trip.Destinations
					.OrderBy(d => d.Order)
					.Select(d => new DTO.GETDestination
					{
						Id = d.Id,
						Name = d.Name,
						Order = d.Order, 
						TripId = d.TripId,
					}),

				TravelBookings = trip.TravelBookings.Select(tb => new DTO.GETTravelBooking
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

				AccommodationBookings = trip.AccommodationBookings.Select(ab => new DTO.GETAccommodationBooking
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
