using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

using fabrizio.Shared.DTO;
using fabrizio.Shared.Contracts;

namespace fabrizio.App.Services
{
	public interface ITripService
	{
		Task<Result<IEnumerable<GETTrip>>> GetTrips();
		Task<GETTrip> GetTrip(Guid id);
		Task AddTrip(POSTTrip trip);
		Task UpdateTrip(PUTTrip trip);
		Task DeleteTrip(Guid id);
	}




	public class TripService : ITripService
	{
		private readonly HttpClient _http;

		public TripService(HttpClient httpClient)
		{
			_http = httpClient;
		}

		public async Task<Result<IEnumerable<GETTrip>>> GetTrips()
		{
			var result = await _http.GetFromJsonAsync<Result<PagedResult<GETTrip>>>("api/trips");

			if (result == null)
			{
				return Result<IEnumerable<GETTrip>>.Fail(new BusinessError("network_error", "Unable to reach server.", 0));
			}

			if (!result.IsSuccess)
			{
				return Result<IEnumerable<GETTrip>>.Fail(result.Error!);
			}

			return Result<IEnumerable<GETTrip>>.Success(result.Value!.Items);
		}

		public async Task<GETTrip> GetTrip(Guid id)
		{
			var result = await _http.GetFromJsonAsync<GETTrip>($"api/trips/{id}");
			if (result == null)	throw new KeyNotFoundException($"Trip with ID {id} not found");
			return result;
		}

		public async Task AddTrip(POSTTrip trip)
		{
			await _http.PostAsJsonAsync("api/trips", trip);
		}

		public async Task UpdateTrip(PUTTrip trip)
		{
			await _http.PutAsJsonAsync($"api/trips/{trip.Id}", trip);
		}

		public async Task DeleteTrip(Guid id)
		{
			await _http.DeleteAsync($"api/trips/{id}");
		}
	}

}
