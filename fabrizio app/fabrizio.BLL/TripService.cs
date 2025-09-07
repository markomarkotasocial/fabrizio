using fabrizio.DAL;
using fabrizio.DTO;
using fabrizio.Repository;
using Microsoft.EntityFrameworkCore;


namespace fabrizio.BLL
{
	public interface ITripService
	{
		Task<PagedResult<DTO.GETTrip>> GetAllAsync(int accountid, int skip = 0, int take = 100, string? name = null, string? destination = null, DateTime? startdate = null, DateTime? enddate = null);
		//Task<DTO.Trip?> GetByIdAsync(int id);
		//Task AddAsync(DTO.Trip trip);
		//Task UpdateAsync(DTO.Trip trip);
		//Task DeleteAsync(int id);
	}

	public class TripService : ITripService
	{
		private readonly ITripRepository _repository;
		private readonly AppDbContext _context;

		public TripService(ITripRepository repository, AppDbContext context)
		{
			_repository = repository;
			_context = context;	
		}

		public async Task<PagedResult<DTO.GETTrip>> GetAllAsync(int accountid, int skip = 0, int take = 100, string? name = null, string? destination = null, DateTime? startdate = null,  DateTime? enddate = null)
		{
			#region Validate

			if (accountid < 0)
				throw new ArgumentException("Account ID must be a non-negative integer.", nameof(accountid));

			if (take <= 0)
				throw new ArgumentException("Take must be greater than zero.", nameof(take));
			
			#endregion Validate

			var query = _repository.QueryByAccount(accountid);

			#region Filters

			if (!string.IsNullOrWhiteSpace(name))
			{
				var trimmedName = name.Trim();
				if (trimmedName.Length > 0)
					query = query.Where(t => t.Name.Contains(trimmedName));
			}

			if (!string.IsNullOrWhiteSpace(destination))
			{
				var trimmedDestination = destination.Trim();
				if (trimmedDestination.Length > 0)
					query = query.Where(t => t.Destination.Contains(trimmedDestination));
			}

			if (startdate.HasValue)
				query = query.Where(t => t.StartDate >= startdate.Value);

			if (enddate.HasValue)
				query = query.Where(t => t.EndDate <= enddate.Value);

			#endregion Filters

			// Paging
			var totalCount = await query.CountAsync();
			var items = await query.Skip(skip).Take(take).ToListAsync();

			// Map to DTOs
			var dtoItems = items.Select(trip => new DTO.GETTrip
			{
				Id = trip.Id,
				Status = (int)trip.Status,
				Name = trip.Name,
				Destination = trip.Destination,
				StartDate = trip.StartDate,
				EndDate = trip.EndDate
			});

			return new PagedResult<DTO.GETTrip>
			{
				TotalCount = totalCount,
				Items = dtoItems
			};
		}


		//public async Task<DTO.Trip?> GetByIdAsync(int id)
		//{
		//	return await _repository.GetByIdAsync(id);
		//}

		//public async Task AddAsync(DTO.Trip trip)
		//{
		//	// Example business rule: trip must have end date after start date
		//	if (trip.EndDate <= trip.StartDate)
		//		throw new ArgumentException("End date must be after start date.");

		//	await _repository.AddAsync(trip);
		//}

		//public async Task UpdateAsync(DTO.Trip trip)
		//{
		//	if (trip.EndDate <= trip.StartDate)
		//		throw new ArgumentException("End date must be after start date.");

		//	await _repository.UpdateAsync(trip);
		//}

		//public async Task DeleteAsync(int id)
		//{
		//	await _repository.DeleteAsync(id);
		//}
	}

}
