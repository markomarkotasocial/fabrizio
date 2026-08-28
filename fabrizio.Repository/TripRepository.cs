using Microsoft.EntityFrameworkCore;

using fabrizio.DAL;
using fabrizio.DAL.Entities;


namespace fabrizio.Repository
{
	public interface ITripRepository : IRepository<Trip>
	{
		IQueryable<Trip> QueryAll(int accountid);
		Task<Trip?> GetById(Guid id);
		Task<bool> HasOverlappingTrip(int accountId, DateTime? start, DateTime? end, Guid? excludeTripId = null);
	}


	public class TripRepository : RepositoryBase<Trip>, ITripRepository
	{
		public TripRepository(AppDbContext context) : base(context) { }

		public IQueryable<Trip> QueryAll(int accountid)
		{
			return Context.Trips.Where(x => x.AccountId == accountid).Include(t => t.AccommodationBookings).Include(t => t.TravelBookings).Include(t => t.Destinations).AsNoTracking();
		}

		public async Task<Trip?> GetById(Guid id)
		{
			return await Context.Trips.Include(t => t.AccommodationBookings).Include(t => t.TravelBookings).Include(t => t.Destinations).FirstOrDefaultAsync(t => t.Id == id);
		}

		public async Task<bool> HasOverlappingTrip(int accountId, DateTime? start, DateTime? end, Guid? excludeTripId = null)
		{
			var normalizedEnd = end ?? DateTime.Today;

			return await Context.Trips.AnyAsync(t =>
				t.AccountId == accountId
				&& t.Status != TripStatus.Cancelled
				&& (excludeTripId == null || t.Id != excludeTripId)
				&& t.StartDate.HasValue
				&& t.StartDate.Value <= normalizedEnd
				&& (t.EndDate ?? DateTime.Today) >= start
			);
		}
	}
}
