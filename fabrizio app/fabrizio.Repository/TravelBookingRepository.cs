using Microsoft.EntityFrameworkCore;

using fabrizio.DAL;
using fabrizio.DAL.Entities;


namespace fabrizio.Repository
{
	public interface ITravelBookingRepository
	{
		Task SaveChangesAsync();

		//IQueryable<TravelBooking> QueryAll(int accountid);
		//Task<TravelBooking?> GetById(Guid id);
		void Add(TravelBooking travelbooking);
		void Delete(TravelBooking travelbooking);
	}


	public class TravelBookingRepository : ITravelBookingRepository
	{

		private readonly AppDbContext _context;

		public TravelBookingRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}



		//public IQueryable<Trip> QueryAll(int accountid)
		//{
		//	return _context.Trips.Where(x => x.AccountId == accountid).AsNoTracking();
		//}

		//public async Task<Trip?> GetById(Guid id)
		//{
		//	return await _context.Trips.FindAsync(id);
		//}

		public void Add(TravelBooking travelbooking)
		{
			_context.TravelBookings.Add(travelbooking);
		}



		public void Delete(TravelBooking travelbooking)
		{
			_context.TravelBookings.Remove(travelbooking);
		}


	}
}
