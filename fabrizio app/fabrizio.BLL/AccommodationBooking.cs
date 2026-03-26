using fabrizio.DAL.Entities;
using fabrizio.DAL.Migrations;
using fabrizio.Shared.DTO;
using fabrizio.Shared.Contracts;
using fabrizio.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.BLL
{
	public partial class TripService : ITripService
	{

		public async Task<Result<AccommodationBookingDto>> CreateAccommodationBooking(int accountid, Guid tripid, CreateAccommodationBookingRequest dto)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			ArgumentNullException.ThrowIfNull(dto, nameof(dto));
			if (tripid.Equals(Guid.Empty)) throw new ArgumentException("Trip id is not correct.", nameof(tripid));

			if (string.IsNullOrWhiteSpace(dto.Location))
			{
				return Result<AccommodationBookingDto>.Fail(new BusinessError("accomodationbooking_location_required", "Location must be provided.", 400));
			}

			if (!Enum.IsDefined(typeof(AccommodationBookingTypes), dto.Type))
			{
				return Result<AccommodationBookingDto>.Fail(new BusinessError("accomodationbooking_type_invalid", "Invalid accommodation booking type.", 400));
			}

			if (dto.From == null || dto.To == null)
			{
				return Result<AccommodationBookingDto>.Fail(new BusinessError("accomodationbooking_dated_required", "Accommodation booking must have both from and to dates.", 400));
			}

			if (dto.From > dto.To)
			{
				return Result<AccommodationBookingDto>.Fail(new BusinessError("accomodationbooking_dates_inconsistency", "End date cannot be earlier than start date.", 400));
			}
			
			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null)
			{
				return Result<AccommodationBookingDto>.Fail(new BusinessError("trip_not_found", "There is no trip with specified ID.", 404));
			}

			if (trip.Status == TripStatus.Cancelled)
			{
				return Result<AccommodationBookingDto>.Fail(new BusinessError("trip_cancelled", "Cancelled trip is not editable.", 409));
			}

			if (trip.AccountId != accountid)
			{
				return Result<AccommodationBookingDto>.Fail(new BusinessError("forbidden", "You do not have access to this trip.", 403));
			}

			#endregion Validate

			var accommodationbooking = new AccommodationBooking
			{
				AccountId = accountid,
				TripId = tripid,
				Type = (AccommodationBookingTypes)dto.Type,
				Reference = dto.Reference, 
				From = dto.From,
				To = dto.To, 
				Location = dto.Location,
				Name = string.IsNullOrWhiteSpace(dto.Name) ? dto.Location : dto.Name,
				Note = dto.Note
			};

			trip.AccommodationBookings.Add(accommodationbooking);
			trip.Recalculate();
			await _accommodationBookingRepository.SaveChangesAsync();
			return Result<AccommodationBookingDto>.Success(new AccommodationBookingDto 
			{
				Id = accommodationbooking.Id,
				Type = (int)accommodationbooking.Type,
				Reference = accommodationbooking.Reference,
				From = accommodationbooking.From,
				To = accommodationbooking.To,
				Location = accommodationbooking.Location,
				Name = accommodationbooking.Name,
				Note = accommodationbooking.Note
			});
		}

		public async Task<Result<AccommodationBookingDto>> UpdateAccommodationBooking(int accountid, Guid tripid, UpdateAccommodationBookingRequest dto)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			ArgumentNullException.ThrowIfNull(dto, nameof(dto));
			if (tripid.Equals(Guid.Empty)) throw new ArgumentException("Trip id is not correct.", nameof(tripid));

			if (string.IsNullOrWhiteSpace(dto.Location))
			{
				return Result<AccommodationBookingDto>.Fail(new BusinessError("accomodationbooking_location_required", "Location must be provided.", 400));
			}

			if (string.IsNullOrWhiteSpace(dto.Name))
			{
				return Result<AccommodationBookingDto>.Fail(new BusinessError("accomodationbooking_name_required", "Name must be provided.", 400));
			}

			if (!Enum.IsDefined(typeof(AccommodationBookingTypes), dto.Type))
			{
				return Result<AccommodationBookingDto>.Fail(new BusinessError("accomodationbooking_type_invalid", "Invalid accommodation booking type.", 400));
			}

			if (dto.From == null || dto.To == null)
			{
				return Result<AccommodationBookingDto>.Fail(new BusinessError("accomodationbooking_dated_required", "Accommodation booking must have both from and to dates.", 400));
			}

			if (dto.From > dto.To)
			{
				return Result<AccommodationBookingDto>.Fail(new BusinessError("accomodationbooking_dates_inconsistency", "End date cannot be earlier than start date.", 400));
			}

			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null)
			{
				return Result<AccommodationBookingDto>.Fail(new BusinessError("trip_not_found", "There is no trip with specified ID.", 404));
			}

			if (trip.Status == TripStatus.Cancelled)
			{
				return Result<AccommodationBookingDto>.Fail(new BusinessError("trip_cancelled", "Cancelled trip is not editable.", 409));
			}

			if (trip.AccountId != accountid)
			{
				return Result<AccommodationBookingDto>.Fail(new BusinessError("forbidden", "You do not have access to this trip.", 403));
			}

			AccommodationBooking? booking = trip.AccommodationBookings.FirstOrDefault(b => b.Id == dto.Id);
			if (booking == null)
			{
				return Result<AccommodationBookingDto>.Fail(new BusinessError("accomodationbooking_not_found", "There is no accommodation booking with specified ID.", 404));
			}

			#endregion Validate

			booking.Type = (AccommodationBookingTypes)dto.Type;
			booking.Location = dto.Location;
			booking.Name = dto.Name;
			booking.Reference = dto.Reference;
			booking.From = dto.From;
			booking.To = dto.To;
			booking.Note = dto.Note;

			trip.Recalculate();
			await _tripRepository.SaveChangesAsync();
			return Result<AccommodationBookingDto>.Success(new AccommodationBookingDto
			{
				Id = booking.Id,
				Name = booking.Name, 
				From = booking.From, 
				To = booking.To, 
				Location = booking.Location, 
				Note = booking.Note, 
				Reference = booking.Reference,  
				Type = (int)booking.Type, 
				TripId = booking.TripId,
			});
		}

		public async Task<Result> DeleteAccommodationBooking(int accountid, Guid tripid, Guid accommodationbookingid)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			if (tripid.Equals(Guid.Empty)) throw new ArgumentException("Trip ID is not correct.", nameof(tripid));
			if (accommodationbookingid.Equals(Guid.Empty)) throw new ArgumentException("Accommodation booking ID is not correct.", nameof(accommodationbookingid));

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

			AccommodationBooking? booking = trip.AccommodationBookings.FirstOrDefault(b => b.Id == accommodationbookingid);
			if (booking == null)
			{
				return Result.Fail(new BusinessError("accomodationbooking_not_found", "There is no accommodation booking with specified ID.", 404));
			}

			#endregion Validate

			trip.AccommodationBookings.Remove(booking);
			trip.Recalculate();
			await _tripRepository.SaveChangesAsync();
			return Result.Success();
		}
	}
}
