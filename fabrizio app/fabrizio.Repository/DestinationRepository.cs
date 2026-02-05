using Microsoft.EntityFrameworkCore;

using fabrizio.DAL;
using fabrizio.DAL.Entities;


namespace fabrizio.Repository
{
	public interface IDestinationRepository
	{
		Task SaveChangesAsync();


		void Add(Destination destination);
		void Delete(Destination destination);
	}


	public class DestinationRepository : IDestinationRepository
	{

		private readonly AppDbContext _context;

		public DestinationRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}



		public void Add(Destination destination)
		{
			_context.Destinations.Add(destination);
		}

		public void Delete(Destination destination)
		{
			_context.Destinations.Remove(destination);
		}


	}
}
