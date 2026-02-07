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

		public async Task<Destination> CreateDestination(int accountid, Guid tripid, POSTDestination dto)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));

			ArgumentNullException.ThrowIfNull(dto, nameof(dto));
		
			if (string.IsNullOrWhiteSpace(dto.Name))
				throw new ArgumentException("Name must be provided.", nameof(dto.Name));

			var hasOverlap = await _destinationRepository.HasOverlappingDestination(accountid, tripid, dto.Name, null);
			if (hasOverlap) throw new ArgumentException("Destination name overlap.");

			if (tripid.Equals(Guid.Empty)) throw new ArgumentException("Trip id is not correct.", nameof(tripid));
			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null) throw new ArgumentException("There is no trip with specified ID!");
			if (trip.Status == TripStatus.Cancelled) throw new InvalidOperationException("Cancelled trip is not editable.");

			#endregion Validate

			var nextOrder = trip.Destinations.Any()	? trip.Destinations.Max(d => d.Order) + 1: 1;

			var destination = new Destination
			{
				AccountId = accountid,
				TripId = tripid,
				Name = dto.Name,
				Order = nextOrder
			};

			trip.Destinations.Add(destination);
			await _travelBookingRepository.SaveChangesAsync();
			return destination;
		}

		public async Task UpdateDestination(int accountid, Guid tripid, PUTDestination dto)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));

			if (string.IsNullOrWhiteSpace(dto.Name))
				throw new ArgumentException("Name must be provided.", nameof(dto.Name));

			ArgumentNullException.ThrowIfNull(dto, nameof(dto));

			var hasOverlap = await _destinationRepository.HasOverlappingDestination(accountid, tripid, dto.Name, dto.Id);
			if (hasOverlap) throw new ArgumentException("Destination name overlap.");

			if (tripid.Equals(Guid.Empty)) throw new ArgumentException("Trip id is not correct.", nameof(tripid));
			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null) throw new KeyNotFoundException("There is no trip with specified ID!");
			if (trip.Status == TripStatus.Cancelled) throw new ArgumentException("Cancelled trip is not editable.");

			Destination? destination = trip.Destinations.FirstOrDefault(b => b.Id == dto.Id);
			if (destination == null) throw new KeyNotFoundException("There is no destination with specified ID!");

			#endregion Validate

			destination.Name = dto.Name;
			await _tripRepository.SaveChangesAsync();
		}

		public async Task DeleteDestination(int accountid, Guid tripid, Guid destinationid)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));

			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null) throw new KeyNotFoundException("There is no trip with specified ID!");
			if (trip.Status == TripStatus.Cancelled) throw new InvalidOperationException("Cancelled trip is not editable.");

			Destination? destination = trip.Destinations.FirstOrDefault(b => b.Id == destinationid);
			if (destination == null) throw new KeyNotFoundException("There is no destination with specified ID!");

			#endregion Validate

			trip.Destinations.Remove(destination);
			await _tripRepository.SaveChangesAsync();
		}

	}
}
