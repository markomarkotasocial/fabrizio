using fabrizio.DAL.Entities;
using fabrizio.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.BLL
{
	public partial class TripService : ITripService
	{

		public async Task<TravelBooking> CreateTravelBooking(int accountid, Guid tripid, POSTTravelBooking dto)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));

			ArgumentNullException.ThrowIfNull(dto, nameof(dto));

			if (!Enum.IsDefined(typeof(TravelBookingTypes), dto.Type))
			{
				throw new ArgumentException($"Invalid travel booking type: {dto.Type}");
			}

			if (string.IsNullOrWhiteSpace(dto.Origin))
				throw new ArgumentException("Origin must be provided.", nameof(dto.Origin));

			if (string.IsNullOrWhiteSpace(dto.Destination))
				throw new ArgumentException("Destination must be provided.", nameof(dto.Destination));

			if (dto.Departure != null && dto.Departure != null)
			{
				if (dto.Arrival < dto.Departure)
					throw new ArgumentException("Arrival cannot be earlier than departure.", nameof(dto.Arrival));
			}

			if (!Enum.IsDefined(typeof(TravelBookingTypes), dto.Type))
				throw new ArgumentException($"Invalid travel booking type: {dto.Type}");

			if (tripid.Equals(Guid.Empty)) throw new ArgumentException("Trip id is not correct.", nameof(tripid));
			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null) throw new KeyNotFoundException("There is no trip with specified ID!");
			if (trip.Status == TripStatus.Cancelled) throw new InvalidOperationException("Cancelled trip is not editable.");

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
			return travelbooking;
		}

		public async Task UpdateTravelBooking(int accountid, Guid tripid ,PUTTravelBooking dto)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));

			ArgumentNullException.ThrowIfNull(dto, nameof(dto));

			if (string.IsNullOrWhiteSpace(dto.Origin))
				throw new ArgumentException("Origin must be provided.", nameof(dto.Origin));

			if (string.IsNullOrWhiteSpace(dto.Destination))
				throw new ArgumentException("Destination must be provided.", nameof(dto.Destination));

			if (dto.Departure != null && dto.Departure != null)
			{
				if (dto.Arrival < dto.Departure)
					throw new ArgumentException("Arrival cannot be earlier than departure.", nameof(dto.Arrival));
			}

			if (!Enum.IsDefined(typeof(TravelBookingTypes), dto.Type))
				throw new ArgumentException($"Invalid travel booking type: {dto.Type}");

			if (tripid.Equals(Guid.Empty)) throw new ArgumentException("Trip id is not correct.", nameof(tripid));
			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null) throw new KeyNotFoundException("There is no trip with specified ID!");
			if (trip.Status == TripStatus.Cancelled) throw new InvalidOperationException("Cancelled trip is not editable.");

			TravelBooking? booking = trip.TravelBookings.FirstOrDefault(b => b.Id == dto.Id);
			if (booking == null) throw new KeyNotFoundException("There is no travel booking with specified ID!");

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
		}

		public async Task DeleteTravelBooking(int accountid, Guid tripid, Guid travelbookingid)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));

			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null) throw new KeyNotFoundException("There is no trip with specified ID!");
			if (trip.Status == TripStatus.Cancelled) throw new InvalidOperationException("Cancelled trip is not editable.");

			TravelBooking? booking = trip.TravelBookings.FirstOrDefault(b => b.Id == travelbookingid);
			if (booking == null) throw new KeyNotFoundException("There is no travel booking with specified ID!");

			#endregion Validate

			trip.TravelBookings.Remove(booking);
			trip.Recalculate();
			await _tripRepository.SaveChangesAsync();
		}

	}
}
