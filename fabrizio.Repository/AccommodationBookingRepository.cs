using fabrizio.DAL;
using fabrizio.DAL.Entities;


namespace fabrizio.Repository
{
	public interface IAccommodationBookingRepository : IRepository<AccommodationBooking>
	{
	}


	public class AccommodationBookingRepository : RepositoryBase<AccommodationBooking>, IAccommodationBookingRepository
	{
		public AccommodationBookingRepository(AppDbContext context) : base(context) { }
	}
}
