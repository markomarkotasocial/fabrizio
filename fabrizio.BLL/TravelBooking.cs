using fabrizio.DAL.Entities;
using fabrizio.Shared.DTO;
using fabrizio.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.BLL
{
	public partial class TripService : ITripService
	{

		public async Task<Result<TravelBookingDto>> CreateTravelBooking(int accountid, Guid tripid, CreateTravelBookingRequest dto)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			ArgumentNullException.ThrowIfNull(dto, nameof(dto));
			if (tripid.Equals(Guid.Empty)) throw new ArgumentException("Trip id is not correct.", nameof(tripid));

			if (!Enum.IsDefined(typeof(TravelBookingTypes), dto.Type))
			{
				return Result<TravelBookingDto>.Fail(new BusinessError("travelbooking_type_invalid", "Invalid travel booking type.", 400));
			}

			if (string.IsNullOrWhiteSpace(dto.Origin))
			{
				return Result<TravelBookingDto>.Fail(new BusinessError("travelbooking_origin_required", "Origin must be provided.", 400));
			}

			if (string.IsNullOrWhiteSpace(dto.Destination))
			{
				return Result<TravelBookingDto>.Fail(new BusinessError("travelbooking_destination_required", "Destination must be provided.", 400));
			}

			if (dto.Departure != null && dto.Departure != null)
			{
				if (dto.Arrival < dto.Departure)
				{
					return Result<TravelBookingDto>.Fail(new BusinessError("travelbooking_dates_inconsistency", "Arrival cannot be earlier than departure.", 400));
				}
			}

			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null)
			{
				return Result<TravelBookingDto>.Fail(new BusinessError("trip_not_found", "There is no trip with specified ID.", 404));
			}

			if (trip.Status == TripStatus.Cancelled)
			{
				return Result<TravelBookingDto>.Fail(new BusinessError("trip_cancelled", "Cancelled trip is not editable.", 409));
			}

			if (trip.AccountId != accountid)
			{
				return Result<TravelBookingDto>.Fail(new BusinessError("forbidden", "You do not have access to this trip.", 403));
			}

			#endregion Validate

			var travelbooking = new TravelBooking
			{
				AccountId = accountid,
				TripId = tripid,
				Type = (TravelBookingTypes)dto.Type,
				Reference = dto.Reference,
				Carrier = dto.Carrier,
				Departure = dto.Departure,
				Arrival = dto.Arrival,
				Origin = dto.Origin,
				Destination = dto.Destination,
				Note = dto.Note
			};

			trip.TravelBookings.Add(travelbooking);
			trip.Recalculate();
			await _travelBookingRepository.SaveChangesAsync();
			return Result<TravelBookingDto>.Success(travelbooking.ToDto());
		}

		public async Task<Result<TravelBookingDto>> UpdateTravelBooking(int accountid, Guid tripid ,UpdateTravelBookingRequest dto)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			ArgumentNullException.ThrowIfNull(dto, nameof(dto));
			if (tripid.Equals(Guid.Empty)) throw new ArgumentException("Trip id is not correct.", nameof(tripid));

			if (!Enum.IsDefined(typeof(TravelBookingTypes), dto.Type))
			{
				return Result<TravelBookingDto>.Fail(new BusinessError("travelbooking_type_invalid", "Invalid travel booking type.", 400));
			}

			if (string.IsNullOrWhiteSpace(dto.Origin))
			{
				return Result<TravelBookingDto>.Fail(new BusinessError("travelbooking_origin_required", "Origin must be provided.", 400));
			}

			if (string.IsNullOrWhiteSpace(dto.Destination))
			{
				return Result<TravelBookingDto>.Fail(new BusinessError("travelbooking_destination_required", "Destination must be provided.", 400));
			}

			if (dto.Departure != null && dto.Departure != null)
			{
				if (dto.Arrival < dto.Departure)
				{
					return Result<TravelBookingDto>.Fail(new BusinessError("travelbooking_dates_inconsistency", "Arrival cannot be earlier than departure.", 400));
				}
			}

			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null)
			{
				return Result<TravelBookingDto>.Fail(new BusinessError("trip_not_found", "There is no trip with specified ID.", 404));
			}

			if (trip.Status == TripStatus.Cancelled)
			{
				return Result<TravelBookingDto>.Fail(new BusinessError("trip_cancelled", "Cancelled trip is not editable.", 409));
			}

			if (trip.AccountId != accountid)
			{
				return Result<TravelBookingDto>.Fail(new BusinessError("forbidden", "You do not have access to this trip.", 403));
			}

			TravelBooking? booking = trip.TravelBookings.FirstOrDefault(b => b.Id == dto.Id);
			if (booking == null)
			{
				return Result<TravelBookingDto>.Fail(new BusinessError("travelbooking_not_found", "There is no travel booking with specified ID.", 404));
			}

			#endregion Validate

			booking.Type = (TravelBookingTypes)dto.Type;
			booking.Reference = dto.Reference;
			booking.Carrier = dto.Carrier;
			booking.Departure = dto.Departure;
			booking.Arrival = dto.Arrival;
			booking.Origin = dto.Origin;
			booking.Destination = dto.Destination;
			booking.Note = dto.Note;

			trip.Recalculate();
			await _tripRepository.SaveChangesAsync();
			return Result<TravelBookingDto>.Success(booking.ToDto());
		}

		public async Task<Result> DeleteTravelBooking(int accountid, Guid tripid, Guid travelbookingid)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			if (tripid.Equals(Guid.Empty)) throw new ArgumentException("Trip ID is not correct.", nameof(tripid));
			if (travelbookingid.Equals(Guid.Empty)) throw new ArgumentException("Travel booking ID is not correct.", nameof(travelbookingid));

			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null)
			{
				return Result.Fail(new BusinessError("trip_not_found", "There is no trip with specified ID.", 404));
			}

			if (trip.Status == TripStatus.Cancelled)
			{
				return Result<TravelBooking>.Fail(new BusinessError("trip_cancelled", "Cancelled trip is not editable.", 409));
			}

			if (trip.AccountId != accountid)
			{
				return Result.Fail(new BusinessError("forbidden", "You do not have access to this trip.", 403));
			}

			TravelBooking? booking = trip.TravelBookings.FirstOrDefault(b => b.Id == travelbookingid);
			if (booking == null)
			{
				return Result.Fail(new BusinessError("travelbooking_not_found", "There is no travel booking with specified ID.", 404));
			}

			#endregion Validate

			trip.TravelBookings.Remove(booking);
			trip.Recalculate();
			await _tripRepository.SaveChangesAsync();
			return Result.Success();
		}

	}
}
