using fabrizio.DAL.Entities;
using fabrizio.Shared.DTO;
using fabrizio.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.BLL
{
	public partial class TripService : ITripService
	{

		public async Task<Result<Destination>> CreateDestination(int accountid, Guid tripid, POSTDestination dto)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			ArgumentNullException.ThrowIfNull(dto, nameof(dto));
			if (tripid.Equals(Guid.Empty)) throw new ArgumentException("Trip id is not correct.", nameof(tripid));

			if (string.IsNullOrWhiteSpace(dto.Name))
			{
				return Result<Destination>.Fail(new BusinessError("destination_name_required", "Name must be provided.", 400));
			}

			var hasOverlap = await _destinationRepository.HasOverlappingDestination(accountid, tripid, dto.Name, null);
			if (hasOverlap)
			{
				return Result<Destination>.Fail(new BusinessError("destination_overlap", "Destination name overlap.",409));
			}

			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null)
			{
				return Result<Destination>.Fail(new BusinessError("trip_not_found", "There is no trip with specified ID.",404));
			}

			if (trip.Status == TripStatus.Cancelled)
			{
				return Result<Destination>.Fail(new BusinessError("trip_cancelled", "Cancelled trip is not editable.", 409));
			}

			if (trip.AccountId != accountid)
			{
				return Result<Destination>.Fail(new BusinessError("forbidden", "You do not have access to this trip.", 403));
			}

			#endregion Validate

			var nextOrder = trip.Destinations.Any()	? trip.Destinations.Max(d => d.Order) + 1 : 1;

			var destination = new Destination
			{
				AccountId = accountid,
				TripId = tripid,
				Name = dto.Name,
				Order = nextOrder
			};

			trip.Destinations.Add(destination);
			await _travelBookingRepository.SaveChangesAsync();
			return Result<Destination>.Success(destination);
		}

		public async Task<Result> UpdateDestination(int accountid, Guid tripid, PUTDestination dto)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			ArgumentNullException.ThrowIfNull(dto, nameof(dto));
			if (tripid.Equals(Guid.Empty)) throw new ArgumentException("Trip id is not correct.", nameof(tripid));

			if (string.IsNullOrWhiteSpace(dto.Name))
			{
				return Result.Fail(new BusinessError("destination_name_required", "Name must be provided.", 400));
			}

			var hasOverlap = await _destinationRepository.HasOverlappingDestination(accountid, tripid, dto.Name, dto.Id);
			if (hasOverlap)
			{
				return Result.Fail(new BusinessError("destination_overlap", "Destination name overlap.", 409));
			}
						
			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null)
			{
				return Result.Fail(new BusinessError("trip_not_found", "There is no trip with specified ID.", 404));
			}

			if (trip.AccountId != accountid)
			{
				return Result.Fail(new BusinessError("forbidden", "You do not have access to this trip.", 403));
			}

			if (trip.Status == TripStatus.Cancelled)
			{
				return Result.Fail(new BusinessError("trip_cancelled", "Cancelled trip is not editable.", 409));
			}

			Destination? destination = trip.Destinations.FirstOrDefault(b => b.Id == dto.Id);
			if (destination == null)
			{
				return Result.Fail(new BusinessError("destination_not_found", "There is no destination with specified ID.", 404));
			}

			#endregion Validate

			destination.Name = dto.Name;

			await _tripRepository.SaveChangesAsync();
			return Result.Success();
		}

		public async Task<Result> DeleteDestination(int accountid, Guid tripid, Guid destinationid)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			if(tripid.Equals(Guid.Empty)) throw new ArgumentException("Trip ID is not correct.", nameof(tripid));
			if(destinationid.Equals(Guid.Empty)) throw new ArgumentException("Destination ID is not correct.", nameof(destinationid));

			Trip? trip = await _tripRepository.GetById(tripid);
			if (trip == null)
			{
				return Result.Fail(new BusinessError("trip_not_found", "There is no trip with specified ID.", 404));
			}

			if (trip.AccountId != accountid)
			{
				return Result.Fail(new BusinessError("forbidden", "You do not have access to this trip.", 403));
			}

			Destination? destination = trip.Destinations.FirstOrDefault(b => b.Id == destinationid);
			if (destination == null)
			{
				return Result.Fail(new BusinessError("destination_not_found", "There is no destination with specified ID.", 404));
			}

			#endregion Validate

			trip.Destinations.Remove(destination);

			var ordered = trip.Destinations.OrderBy(d => d.Order).ToList();
			for (int i = 0; i < ordered.Count; i++)	ordered[i].Order = i + 1;

			await _tripRepository.SaveChangesAsync();
			return Result.Success();
		}

	}
}
