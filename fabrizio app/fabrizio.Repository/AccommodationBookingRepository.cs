using Microsoft.EntityFrameworkCore;

using fabrizio.DAL;
using fabrizio.DAL.Entities;


namespace fabrizio.Repository
{
	public interface IAccommodationBookingRepository
	{
		Task SaveChangesAsync();

		//IQueryable<TravelBooking> QueryAll(int accountid);
		//Task<TravelBooking?> GetById(Guid id);
		void Add(AccommodationBooking accommodationbooking);
		void Delete(AccommodationBooking accommodationbooking);
	}


	public class AccommodationBookingRepository : IAccommodationBookingRepository
	{

		private readonly AppDbContext _context;

		public AccommodationBookingRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}




		public void Add(AccommodationBooking accommodationbooking)
		{
			_context.AccommodationBookings.Add(accommodationbooking);
		}


		public void Delete(AccommodationBooking accommodationbooking)
		{
			_context.AccommodationBookings.Remove(accommodationbooking);
		}


	}
}
