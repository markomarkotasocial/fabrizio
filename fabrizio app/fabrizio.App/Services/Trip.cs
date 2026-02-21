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
		Task<Result<IEnumerable<TripDto>>> GetTrips();
		Task<Result<TripDto>> GetTrip(Guid id);
		Task<Result<GETTripOverview>> GetTripsOverview();


		Task AddTrip(CreateTripRequest trip);
		Task<Result> UpdateTrip(UpdateTripRequest trip);
		Task DeleteTrip(Guid id);
	}




	public class TripService : ITripService
	{
		private readonly HttpClient _http;

		public TripService(HttpClient httpClient)
		{
			_http = httpClient;
		}

		public async Task<Result<IEnumerable<TripDto>>> GetTrips()
		{
			try
			{
				var result = await _http.GetFromJsonAsync<Result<PagedResult<TripDto>>>("api/trips");

				if (result == null)
				{
					return Result<IEnumerable<TripDto>>.Fail(new BusinessError("network_error", "Unable to reach server.", 0));
				}

				if (!result.IsSuccess)
				{
					return Result<IEnumerable<TripDto>>.Fail(result.Error!);
				}

				return Result<IEnumerable<TripDto>>.Success(result.Value!.Items);
			}
			catch (Exception)
			{
				return Result<IEnumerable<TripDto>>.Fail(new BusinessError("network_error", "Unable to reach server.", 0));
			}
		}

		public async Task<Result<TripDto>> GetTrip(Guid id)
		{
			try
			{
				var result = await _http.GetFromJsonAsync<Result<TripDto>>($"api/trips/{id}");

				if (result == null)
				{
					return Result<TripDto>.Fail(new BusinessError("network_error", "Unable to reach server.", 0));
				}

				if (!result.IsSuccess)
				{
					return Result<TripDto>.Fail(result.Error!);
				}

				return Result<TripDto>.Success(result.Value!);
			}
			catch (Exception)
			{
				return Result<TripDto>.Fail(new BusinessError("network_error", "Unable to reach server.", 0));
			}
		}

		public async Task<Result<GETTripOverview>> GetTripsOverview()
		{
			try
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
			catch (Exception)
			{
				return Result<GETTripOverview>.Fail(new BusinessError("network_error", "Unable to reach server.", 0));
			}
			
		}







		public async Task AddTrip(CreateTripRequest trip)
		{
			await _http.PostAsJsonAsync("api/trips", trip);
		}

		public async Task<Result> UpdateTrip(UpdateTripRequest request)
		{
			try
			{
				var response = await _http.PutAsJsonAsync($"api/trips/{request.Id}", request);
				if (!response.IsSuccessStatusCode)
				{
					var errorResult = await response.Content.ReadFromJsonAsync<Result>();
					return Result.Fail(errorResult?.Error ?? new BusinessError("unknown_error", "An unknown error occurred.", (int)response.StatusCode));
				}

				return Result.Success();
			}
			catch (Exception)
			{
				return Result.Fail(new BusinessError("network_error", "Unable to reach server.", 0));
			}
		}

		public async Task DeleteTrip(Guid id)
		{
			await _http.DeleteAsync($"api/trips/{id}");
		}
	}

}
