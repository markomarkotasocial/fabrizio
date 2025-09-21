using fabrizio.DAL.Entities;
using fabrizio.DTO;
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

		public async Task<AccommodationBooking> CreateAccommodationBooking(int accountid, Guid tripid, POSTAccommodationBooking dto)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));

			ArgumentNullException.ThrowIfNull(dto, nameof(dto));

			if (string.IsNullOrWhiteSpace(dto.Location))
				throw new ArgumentException("Location must be provided.", nameof(dto.Location));

			if (!Enum.IsDefined(typeof(AccommodationBookingTypes), dto.Type))
				throw new ArgumentException($"Invalid accommodation booking type: {dto.Type}");

			if (dto.From == null || dto.To == null)
				throw new ArgumentException("Accommodation booking must have both From and To dates.");

			if (dto.From > dto.To)
				throw new ArgumentException("From date cannot be after To date.");

			if (tripid.Equals(Guid.Empty)) throw new ArgumentException("Trip id is not correct.", nameof(tripid));
			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null) throw new KeyNotFoundException("There is no trip with specified ID!");
			if (trip.Status == TripStatus.Cancelled) throw new InvalidOperationException("Cancelled trip is not editable.");

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
			return accommodationbooking;
		}


		public async Task UpdateAccommodationBooking(int accountid, Guid tripid, PUTAccommodationBooking dto)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));

			ArgumentNullException.ThrowIfNull(dto, nameof(dto));

			if (string.IsNullOrWhiteSpace(dto.Location))
				throw new ArgumentException("Location must be provided.", nameof(dto.Location));

			if (string.IsNullOrWhiteSpace(dto.Name))
				throw new ArgumentException("Name must be provided.", nameof(dto.Name));

			if (!Enum.IsDefined(typeof(AccommodationBookingTypes), dto.Type))
				throw new ArgumentException($"Invalid accommodation booking type: {dto.Type}");

			if (dto.From == null || dto.To == null)
				throw new ArgumentException("Accommodation booking must have both From and To dates.");

			if (dto.From > dto.To)
				throw new ArgumentException("From date cannot be after To date.");

			if (tripid.Equals(Guid.Empty)) throw new ArgumentException("Trip id is not correct.", nameof(tripid));
			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null) throw new KeyNotFoundException("There is no trip with specified ID!");
			if (trip.Status == TripStatus.Cancelled) throw new InvalidOperationException("Cancelled trip is not editable.");

			AccommodationBooking? booking = trip.AccommodationBookings.FirstOrDefault(b => b.Id == dto.Id);
			if (booking == null) throw new KeyNotFoundException("There is no accommodation booking with specified ID!");

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
		}


		public async Task DeleteAccommodationBooking(int accountid, Guid tripid, Guid accommodationbookingid)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));

			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null) throw new KeyNotFoundException("There is no trip with specified ID!");
			if (trip.Status == TripStatus.Cancelled) throw new InvalidOperationException("Cancelled trip is not editable.");

			AccommodationBooking? booking = trip.AccommodationBookings.FirstOrDefault(b => b.Id == accommodationbookingid);
			if (booking == null) throw new KeyNotFoundException("There is no accommodation booking with specified ID!");

			#endregion Validate

			trip.AccommodationBookings.Remove(booking);
			trip.Recalculate();
			await _tripRepository.SaveChangesAsync();
		}
	}
}
