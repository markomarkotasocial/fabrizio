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

		public async Task<Result<DestinationDto>> CreateDestination(int accountid, Guid tripid, CreateDestinationRequest dto)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			ArgumentNullException.ThrowIfNull(dto, nameof(dto));

			if (string.IsNullOrWhiteSpace(dto.Name))
			{
				return Result<DestinationDto>.Fail(new BusinessError("destination_name_required", "Name must be provided.", 400));
			}

			var hasOverlap = await _destinationRepository.HasOverlappingDestination(accountid, tripid, dto.Name, null);
			if (hasOverlap)
			{
				return Result<DestinationDto>.Fail(new BusinessError("destination_overlap", "Destination name overlap.",409));
			}

			var loaded = await LoadOwnedTripAsync(accountid, tripid);
			if (!loaded.IsSuccess) return Result<DestinationDto>.Fail(loaded.Error!);
			var trip = loaded.Value!;

			if (trip.Status == TripStatus.Cancelled)
			{
				return Result<DestinationDto>.Fail(new BusinessError("trip_cancelled", "Cancelled trip is not editable.", 409));
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
			return Result<DestinationDto>.Success(destination.ToDto());
		}

		public async Task<Result<DestinationDto>> UpdateDestination(int accountid, Guid tripid, UpdateDestinationRequest dto)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			ArgumentNullException.ThrowIfNull(dto, nameof(dto));

			if (string.IsNullOrWhiteSpace(dto.Name))
			{
				return Result<DestinationDto>.Fail(new BusinessError("destination_name_required", "Name must be provided.", 400));
			}

			var hasOverlap = await _destinationRepository.HasOverlappingDestination(accountid, tripid, dto.Name, dto.Id);
			if (hasOverlap)
			{
				return Result<DestinationDto>.Fail(new BusinessError("destination_overlap", "Destination name overlap.", 409));
			}
						
			var loaded = await LoadOwnedTripAsync(accountid, tripid);
			if (!loaded.IsSuccess) return Result<DestinationDto>.Fail(loaded.Error!);
			var trip = loaded.Value!;

			if (trip.Status == TripStatus.Cancelled)
			{
				return Result<DestinationDto>.Fail(new BusinessError("trip_cancelled", "Cancelled trip is not editable.", 409));
			}

			Destination? destination = trip.Destinations.FirstOrDefault(b => b.Id == dto.Id);
			if (destination == null)
			{
				return Result<DestinationDto>.Fail(new BusinessError("destination_not_found", "There is no destination with specified ID.", 404));
			}

			#endregion Validate

			destination.Name = dto.Name;

			await _tripRepository.SaveChangesAsync();
			return Result<DestinationDto>.Success(destination.ToDto());
		}

		public async Task<Result> DeleteDestination(int accountid, Guid tripid, Guid destinationid)
		{
			#region Validate

			if (accountid < 0) throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));
			if(destinationid.Equals(Guid.Empty)) throw new ArgumentException("Destination ID is not correct.", nameof(destinationid));

			var loaded = await LoadOwnedTripAsync(accountid, tripid);
			if (!loaded.IsSuccess) return Result.Fail(loaded.Error!);
			var trip = loaded.Value!;

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
