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
		Task<Result<GETTrip>> GetTrip(Guid id);
		Task<Result<GETTripOverview>> GetTripsOverview();


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

		public async Task<Result<GETTrip>> GetTrip(Guid id)
		{
			var result = await _http.GetFromJsonAsync<Result<GETTrip>>($"api/trips/{id}");

			if (result == null)
			{
				return Result<GETTrip>.Fail(new BusinessError("network_error", "Unable to reach server.", 0));
			}

			if (!result.IsSuccess)
			{
				return Result<GETTrip>.Fail(result.Error!);
			}

			return Result<GETTrip>.Success(result.Value!);
		}

		public async Task<Result<GETTripOverview>> GetTripsOverview()
		{
			var result = await _http.GetFromJsonAsync<Result<GETTripOverview>>($"api/trips/overview");

			if (result == null)
			{
				return Result<GETTripOverview>.Fail(new BusinessError("network_error", "Unable to reach server.", 0));
			}

			if (!result.IsSuccess)
			{
				return Result<GETTripOverview>.Fail(result.Error!);
			}

			return Result<GETTripOverview>.Success(result.Value!);
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
