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

			if (tripid.Equals(Guid.Empty)) throw new ArgumentException("Id is not correct.", nameof(tripid));

			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null) throw new KeyNotFoundException("There is no trip with specified ID!");
			if (trip.Status == TripStatus.Cancelled) throw new InvalidOperationException("Cannot add travel booking to a cancelled trip.");

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

			_travelBookingRepository.Add(travelbooking);
			await _travelBookingRepository.SaveChangesAsync();
			return travelbooking;
		}

	}
}
