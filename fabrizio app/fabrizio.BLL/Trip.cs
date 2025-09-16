using Microsoft.EntityFrameworkCore;

using fabrizio.DAL;
using fabrizio.DAL.Entities;
using fabrizio.DTO;
using fabrizio.Repository;


namespace fabrizio.BLL
{
	public interface ITripService
	{
		Task<DTO.GETTrip> GetTripById(Guid id);
		Task<PagedResult<DTO.GETTrip>> GetAllTrips(int accountid, int skip = 0, int take = 100, string? name = null, string? destination = null, DateTime? startdate = null, DateTime? enddate = null);
		Task<Trip> CreateTrip(int accountid, POSTTrip dto);
		Task UpdateTrip(int accountid, Guid id, PUTTrip dto);
		Task DeleteTrip(int accountid, Guid id);


		Task<TravelBooking> CreateTravelBooking(int accountid, Guid tripid, POSTTravelBooking dto);


		Task<TravelBooking> CreateAccommodationBooking(int accountid, Guid tripid, POSTAccommodationBooking dto);
	}


	public partial class TripService : ITripService
	{
		private readonly ITripRepository _tripRepository;
		private readonly ITravelBookingRepository _travelBookingRepository;
		private readonly AppDbContext _context;

		public TripService(ITripRepository repository, ITravelBookingRepository travelBookingRepository, AppDbContext context)
		{
			_tripRepository = repository;
			_travelBookingRepository = travelBookingRepository;
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
				Destination = trip.Destination,
				StartDate = trip.StartDate,
				EndDate = trip.EndDate,
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
				}).ToList(),
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
				}).ToList(),
			};
		}

		public async Task<PagedResult<DTO.GETTrip>> GetAllTrips(int accountid, int skip = 0, int take = 100, string? name = null, string? destination = null, DateTime? startdate = null,  DateTime? enddate = null)
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

			if (!string.IsNullOrWhiteSpace(destination))
			{
				var trimmedDestination = destination.Trim();
				if (trimmedDestination.Length > 0)
					query = query.Where(t => t.Destination.Contains(trimmedDestination));
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
				Destination = trip.Destination,
				StartDate = trip.StartDate,
				EndDate = trip.EndDate
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

			if (dto.StartDate != null && dto.EndDate != null)
			{
				if (dto.EndDate < dto.StartDate)
					throw new ArgumentException("End date cannot be earlier than start date.", nameof(dto.EndDate));
			}


			#endregion Validate

			var trip = new Trip
			{
				AccountId = accountid,
				Name = dto.Name, 
				Destination = dto.Destination, 
				StartDate = dto.StartDate, 
				EndDate = dto.EndDate, 
				Status= TripStatus.Planned
			};

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

			if (dto.StartDate != null && dto.EndDate != null)
			{
				if (dto.EndDate < dto.StartDate)
					throw new ArgumentException("End date cannot be earlier than start date.", nameof(dto.EndDate));
			}

			var trip = await _tripRepository.GetById(id);
			if (trip == null)
				throw new KeyNotFoundException("There is no trip with the specified ID.");

			#endregion Validate

			trip.Name = dto.Name;
			trip.StartDate = dto.StartDate;
			trip.EndDate = dto.EndDate;

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


	}

}
