using Microsoft.EntityFrameworkCore;

using fabrizio.DAL;
using fabrizio.DAL.Entities;


namespace fabrizio.Repository
{
	public interface IDestinationRepository : IRepository<Destination>
	{
		Task<bool> HasOverlappingDestination(int accountId, Guid tripId, string destinationName, Guid? excludeDestinationId = null);
	}


	public class DestinationRepository : RepositoryBase<Destination>, IDestinationRepository
	{
		public DestinationRepository(AppDbContext context) : base(context) { }

		public async Task<bool> HasOverlappingDestination(int accountId, Guid tripId, string destinationName, Guid? excludeDestinationId = null)
		{
			return await Context.Destinations.AnyAsync(t =>
				t.AccountId == accountId
				&& t.TripId == tripId
				&& t.Name == destinationName
				&& (excludeDestinationId == null || t.Id != excludeDestinationId)
			);
		}
	}
}
