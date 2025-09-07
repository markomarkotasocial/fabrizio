using Microsoft.EntityFrameworkCore;

using fabrizio.DAL;
using fabrizio.DAL.Entities;


namespace fabrizio.Repository
{
	public interface ITripRepository
	{
		IQueryable<Trip> QueryByAccount(int accountId);
		//Task<Trip?> GetByIdAsync(int id);
		//Task AddAsync(Trip trip);
		//Task UpdateAsync(Trip trip);
		Task DeleteAsync(int id);
	}


	public class TripRepository : ITripRepository
	{

		private readonly AppDbContext _context;

		public TripRepository(AppDbContext context)
		{
			_context = context;
		}

		public IQueryable<Trip> QueryByAccount(int accountId)
		{
			return _context.Trips.AsNoTracking().Where(t => t.AccountId == accountId);
		}


		//public async Task<Trip?> GetByIdAsync(int id)
		//{
		//	return await _context.Trips.FindAsync(id);
		//}

		//public async Task AddAsync(Trip trip)
		//{
		//	_context.Trips.Add(trip);
		//	await _context.SaveChangesAsync();
		//}

		//public async Task UpdateAsync(Trip trip)
		//{
		//	_context.Trips.Update(trip);
		//	await _context.SaveChangesAsync();
		//}

		public async Task DeleteAsync(int id)
		{
			var trip = await _context.Trips.FindAsync(id);
			if (trip != null)
			{
				_context.Trips.Remove(trip);
				await _context.SaveChangesAsync();
			}
		}


	}
}
