using Microsoft.EntityFrameworkCore;

using fabrizio.DAL;
using fabrizio.DAL.Entities;


namespace fabrizio.Repository
{
	public interface ITripRepository
	{
		Task SaveChangesAsync();

		IQueryable<Trip> QueryAll(int accountid);
		Task<Trip?> GetById(Guid id);
		void Add(Trip trip);
		Task<bool> HasOverlappingTrip(int accountId, DateTime start, DateTime? end, Guid? excludeTripId = null);
		void Delete(Trip trip);
	}


	public class TripRepository : ITripRepository
	{

		private readonly AppDbContext _context;

		public TripRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}



		public IQueryable<Trip> QueryAll(int accountid)
		{
			return _context.Trips.Where(x => x.AccountId == accountid).Include(t => t.AccommodationBookings).Include(t => t.TravelBookings).AsNoTracking();
		}

		public async Task<Trip?> GetById(Guid id)
		{
			return await _context.Trips.Include(t => t.AccommodationBookings).Include(t => t.TravelBookings).FirstOrDefaultAsync(t => t.Id == id); 
		}

		public void Add(Trip trip)
		{
			_context.Trips.Add(trip);
		}

		public async Task<bool> HasOverlappingTrip(int accountId, DateTime start,DateTime? end, Guid? excludeTripId = null)
		{
			var normalizedEnd = end ?? DateTime.Today;

			return await _context.Trips.AnyAsync(t =>
				t.AccountId == accountId
				&& t.Status != TripStatus.Cancelled
				&& (excludeTripId == null || t.Id != excludeTripId)
				&& t.StartDate.HasValue
				&& t.StartDate.Value <= normalizedEnd
				&& (t.EndDate ?? DateTime.Today) >= start
			);
		}


		public void Delete(Trip trip)
		{
			_context.Trips.Remove(trip);
		}


	}
}
