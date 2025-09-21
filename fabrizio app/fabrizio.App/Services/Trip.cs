using fabrizio.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.App.Services
{
	public interface ITripService
	{
		Task<IEnumerable<GETTrip>> GetTrips();
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

		public async Task<IEnumerable<GETTrip>> GetTrips()
		{
			var result = await _http.GetFromJsonAsync<PagedResult<GETTrip>>("api/trips");
			return result?.Items ?? Enumerable.Empty<GETTrip>();
		}

		public async Task<GETTrip> GetTrip(Guid id)
		{
			throw new NotImplementedException();
			//return await _http.GetFromJsonAsync<TripDto>($"api/trips/{id}");
		}

		public async Task AddTrip(POSTTrip trip)
		{
			throw new NotImplementedException();
			//await _http.PostAsJsonAsync("api/trips", trip);
		}

		public async Task UpdateTrip(PUTTrip trip)
		{
			throw new NotImplementedException();
			//await _http.PutAsJsonAsync($"api/trips/{trip.Id}", trip);
		}

		public async Task DeleteTrip(Guid id)
		{
			throw new NotImplementedException();
			//await _http.DeleteAsync($"api/trips/{id}");
		}
	}

}
