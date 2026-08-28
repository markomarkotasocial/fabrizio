using fabrizio.DAL;
using fabrizio.DAL.Entities;


namespace fabrizio.Repository
{
	public interface ITravelBookingRepository : IRepository<TravelBooking>
	{
	}


	public class TravelBookingRepository : RepositoryBase<TravelBooking>, ITravelBookingRepository
	{
		public TravelBookingRepository(AppDbContext context) : base(context) { }
	}
}
