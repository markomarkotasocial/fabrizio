using fabrizio.DAL.Entities;
using fabrizio.Shared.Contracts;

namespace fabrizio.BLL
{
	public partial class TripService
	{
		/// <summary>
		/// Loads a trip and verifies it exists and belongs to <paramref name="accountId"/>.
		/// On failure returns a failed <see cref="Result{T}"/> carrying the right status:
		/// 400 (empty id), 404 (not found), 403 (not the owner).
		/// State checks such as "cancelled" are left to the caller.
		/// </summary>
		private async Task<Result<Trip>> LoadOwnedTripAsync(int accountId, Guid tripId)
		{
			if (tripId == Guid.Empty)
				return Result<Trip>.Fail(new BusinessError("trip_id_invalid", "Trip id is not correct.", 400));

			var trip = await _tripRepository.GetById(tripId);
			if (trip == null)
				return Result<Trip>.Fail(new BusinessError("trip_not_found", "There is no trip with specified ID.", 404));

			if (trip.AccountId != accountId)
				return Result<Trip>.Fail(new BusinessError("forbidden", "You do not have access to this trip.", 403));

			return Result<Trip>.Success(trip);
		}
	}
}
