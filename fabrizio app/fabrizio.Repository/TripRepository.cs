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
			return _context.Trips.Where(x => x.AccountId == accountid).AsNoTracking();
		}

		public async Task<Trip?> GetById(Guid id)
		{
			return await _context.Trips.FindAsync(id);
		}

		public void Add(Trip trip)
		{
			_context.Trips.Add(trip);
		}



		public void Delete(Trip trip)
		{
			_context.Trips.Remove(trip);
		}


	}
}
