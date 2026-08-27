using fabrizio.DAL;
using fabrizio.DAL.Entities;
using fabrizio.Repository;
using fabrizio.Shared.Contracts;
using fabrizio.Shared.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Net.NetworkInformation;
using System.Numerics;
using static Azure.Core.HttpHeader;


namespace fabrizio.BLL
{
	public interface ITripService
	{
		Task<Result<GETTripOverview>> GetTripOverview(int accountid, DateTime? date = null);
		Task<Result<TripDto>> GetTripById(int accountid, Guid id);
		Task<Result<PagedResult<TripListItemDto>>> GetAllTrips(int accountid, int skip = 0, int take = 100, string? name = null, TripFilter filter = TripFilter.CurrentAndUpcoming);
		Task<Result<TripDto>> CreateTrip(int accountid, CreateTripRequest dto);
		Task<Result<TripDto>> UpdateTrip(int accountid, Guid id, UpdateTripRequest dto);
		Task<Result> DeleteTrip(int accountid, Guid id);


		Task<Result<TravelBookingDto>> CreateTravelBooking(int accountid, Guid tripid, CreateTravelBookingRequest dto);
		Task<Result<TravelBookingDto>> UpdateTravelBooking(int accountid, Guid id, UpdateTravelBookingRequest dto);
		Task<Result> DeleteTravelBooking(int accountid, Guid tripid, Guid travelbookingid);

		Task<Result<AccommodationBookingDto>> CreateAccommodationBooking(int accountid, Guid tripid, CreateAccommodationBookingRequest dto);
		Task<Result<AccommodationBookingDto>> UpdateAccommodationBooking(int accountid, Guid tripid, UpdateAccommodationBookingRequest dto);
		Task<Result> DeleteAccommodationBooking(int accountid, Guid tripid, Guid accommodationbookingid);

		Task<Result<DestinationDto>> CreateDestination(int accountid, Guid tripid, CreateDestinationRequest dto);
		Task<Result<DestinationDto>> UpdateDestination(int accountid, Guid tripid, UpdateDestinationRequest dto);
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



		public async Task<Result<TripDto>> GetTripById(int accountid, Guid id)
		{
			#region Validate

			if (id.Equals(Guid.Empty)) throw new ArgumentException("Id is not correct.", nameof(id));
			Trip? trip = await _tripRepository.GetById(id);
			if (trip == null)
			{
				return Result<TripDto>.Fail(new BusinessError("trip_not_found", "There is no trip with specified ID.", 404));
			}

			if (trip.AccountId != accountid)
			{
				return Result<TripDto>.Fail(new BusinessError("forbidden", "You do not have access to this trip.", 403));
			}

			#endregion Validate

			return Result<TripDto>.Success(trip.ToDto());
		}

		public async Task<Result<PagedResult<TripListItemDto>>> GetAllTrips(int accountid, int skip = 0, int take = 100, string? name = null, TripFilter filter = TripFilter.CurrentAndUpcoming)
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

			var today = DateTime.UtcNow.Date;

			switch (filter)
			{
				case TripFilter.Past:
					query = query.Where(t => t.EndDate != null && t.EndDate < today);
					break;

				case TripFilter.CurrentAndUpcoming:
					query = query.Where(t => t.EndDate == null || t.EndDate >= today);
					break;

				case TripFilter.Wishlist:
					query = query.Where(t => t.StartDate == null && t.EndDate == null);
					break;

				case TripFilter.All:
				default:
					// no date filtering
					break;
			}

			#endregion Filters

			query = query.OrderBy(t =>
			t.StartDate != null && t.StartDate <= today && (t.EndDate == null || t.EndDate >= today) ? 0 :
			t.StartDate != null && t.StartDate > today ? 1 : 2)
				.ThenBy(t => t.StartDate)
				.ThenBy(t => t.EndDate)
				.ThenBy(t => t.Name);

			// Paging
			var totalCount = await query.CountAsync();
			var items = await query.Skip(skip).Take(take).ToListAsync();

			// Map to DTOs
			var dtoItems = items.Select(trip => trip.ToListItemDto());

			return Result<PagedResult<TripListItemDto>>.Success(new PagedResult<TripListItemDto>
			{
				TotalCount = totalCount,
				Items = dtoItems
			});
		}

		public async Task<Result<TripDto>> CreateTrip(int accountid, CreateTripRequest dto)
		{
			#region Validate

			if(accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			ArgumentNullException.ThrowIfNull(dto, nameof(dto));

			if (string.IsNullOrWhiteSpace(dto.Name))
			{
				return Result<TripDto>.Fail(new BusinessError("trip_name_required", "Name must be provided.", 400));
			}

			if (dto.EndDate != null)
			{
				if (dto.EndDate < dto.StartDate)
				{
					return Result<TripDto>.Fail(new BusinessError("trip_dates_inconsistency", "End date cannot be earlier than start date.", 400));
				}
			}

			var hasOverlap = await _tripRepository.HasOverlappingTrip(accountid, dto.StartDate, dto.EndDate, excludeTripId: null);
			if (hasOverlap)
			{
				return Result<TripDto>.Fail(new BusinessError("trip_overlap", "Trip dates overlap.", 409));
			}

			#endregion Validate

			var trip = new Trip
			{
				AccountId = accountid,
				Name = dto.Name,
				Notes = dto.Notes ?? string.Empty,
				StartDate = dto.StartDate, 
				EndDate = dto.EndDate
			};

			trip.Recalculate();

			_tripRepository.Add(trip);
			await _tripRepository.SaveChangesAsync();
			return Result<TripDto>.Success(new TripDto
			{
				Id = trip.Id, 
				Name = trip.Name, 
				Notes = trip.Notes ?? string.Empty, 
				StartDate = trip.StartDate, 
				EndDate = trip.EndDate, 
				Status = (int)trip.Status, 
				AccommodationBookings = Enumerable.Empty<AccommodationBookingDto>(), 
				TravelBookings = Enumerable.Empty<TravelBookingDto>(), 
				Destinations = Enumerable.Empty<DestinationDto>()
			});
		}

		public async Task<Result<TripDto>> UpdateTrip(int accountid, Guid id, UpdateTripRequest dto)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			ArgumentNullException.ThrowIfNull(dto, nameof(dto));

			if (string.IsNullOrWhiteSpace(dto.Name))
			{
				return Result<TripDto>.Fail(new BusinessError("trip_name_required", "Name must be provided.", 400));
			}

			var trip = await _tripRepository.GetById(id);
			if (trip == null)
			{
				return Result<TripDto>.Fail(new BusinessError("trip_not_found", "There is no trip with specified ID.", 404));
			}

			if (trip.AccountId != accountid)
			{
				return Result<TripDto>.Fail(new BusinessError("forbidden", "You do not have access to this trip.", 403));
			}

			if (trip.Status == TripStatus.Cancelled)
			{
				return Result<TripDto>.Fail(new BusinessError("trip_cancelled", "Cancelled trip can not be updated.", 403));
			}		

			var dateValidation = ValidateTripDates(trip, dto.StartDate, dto.EndDate);
			if (!dateValidation.IsSuccess)
			{
				return Result<TripDto>.Fail(dateValidation.Error!);
			}

			var hasOverlap = await _tripRepository.HasOverlappingTrip(accountid, dto.StartDate, dto.EndDate, excludeTripId: id);
			if (hasOverlap)
			{
				return Result<TripDto>.Fail(new BusinessError("trip_overlap", "Trip dates overlap.", 409));
			}

			#endregion Validate

			trip.Name = dto.Name;
			trip.Notes = dto.Notes ?? string.Empty;
			trip.StartDate = dto.StartDate;
			trip.EndDate = dto.EndDate;

			trip.Recalculate();

			await _tripRepository.SaveChangesAsync();
			return Result<TripDto>.Success(new TripDto
			{
				Id = trip.Id,
				Name = trip.Name,
				Notes = trip.Notes ?? string.Empty,
				StartDate = trip.StartDate,
				EndDate = trip.EndDate,
				Status = (int)trip.Status,
				AccommodationBookings = Enumerable.Empty<AccommodationBookingDto>(),
				TravelBookings = Enumerable.Empty<TravelBookingDto>(),
				Destinations = Enumerable.Empty<DestinationDto>()
			});
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
				Previous = previous?.ToDto(),
				Current = current?.ToDto(),
				Next = next?.ToDto()
			});
		}






		private Result ValidateTripDates(Trip trip, DateTime? startDate, DateTime? endDate)
		{
			if (startDate.HasValue && endDate.HasValue && endDate < startDate)
			{
				return Result<TripDto>.Fail(new BusinessError("trip_dates_inconsistency", "End date cannot be earlier than start date.", 400));
			}

			bool hasBookings = trip.AccommodationBookings.Any() || trip.TravelBookings.Any();

			if (hasBookings)
			{
				if (!startDate.HasValue)
				{
					return Result<TripDto>.Fail(new BusinessError("trip_start_required", "Start date cannot be removed when bookings exist.", 400));
				}

				if (!endDate.HasValue)
				{
					return Result<TripDto>.Fail(new BusinessError("trip_end_required", "End date cannot be removed when bookings exist.", 400));
				}

				var bookingStarts =
					trip.AccommodationBookings.Where(x => x.From.HasValue).Select(x => x.From!.Value)
					.Concat(trip.TravelBookings.Where(x => x.Departure.HasValue).Select(x => x.Departure!.Value))
					.ToList();

				if (bookingStarts.Any())
				{
					var minBookingStart = bookingStarts.Min();

					if (startDate.Value > minBookingStart)
					{
						return Result<TripDto>.Fail(new BusinessError("trip_start_conflict", "Start date cannot be after the first booking.", 400));
					}
				}

				var bookingEnds =
					trip.AccommodationBookings.Where(x => x.To.HasValue).Select(x => x.To!.Value)
					.Concat(trip.TravelBookings.Where(x => x.Arrival.HasValue).Select(x => x.Arrival!.Value))
					.ToList();

				if (bookingEnds.Any())
				{
					var maxBookingEnd = bookingEnds.Max();

					if (endDate.Value < maxBookingEnd)
					{
						return Result<TripDto>.Fail(
							new BusinessError("trip_end_conflict", "End date cannot be before the last booking.", 400));
					}
				}
			}

			return Result.Success();
		}


	}

}
