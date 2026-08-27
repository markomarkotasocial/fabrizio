using fabrizio.Shared.Contracts;
using fabrizio.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace fabrizio.App.Services
{
	public interface ITripService
	{
		Task<Result<IEnumerable<TripListItemDto>>> GetTrips(TripFilter filter, int skip, int take);
		Task<Result<TripDto>> GetTrip(Guid id);
		Task<Result<GETTripOverview>> GetTripsOverview();

		Task<Result> AddTrip(CreateTripRequest trip);
		Task<Result> UpdateTrip(UpdateTripRequest trip);
		Task<Result> DeleteTrip(Guid id);

		Task<Result<DestinationDto>> AddDestination(Guid tripId, CreateDestinationRequest request);
		Task<Result<DestinationDto>> UpdateDestination(Guid tripId, UpdateDestinationRequest request);
		Task<Result> DeleteDestination(Guid tripId, Guid destinationId);
	}



	public class TripService : ITripService
	{
		private readonly HttpClient _http;

		public TripService(HttpClient httpClient)
		{
			_http = httpClient;
		}

		public async Task<Result<IEnumerable<TripListItemDto>>> GetTrips(TripFilter filter, int skip, int take)
		{
			var url = $"api/trips?filter={filter}&skip={skip}&take={take}";

			var result = await _http.GetResultAsync<PagedResult<TripListItemDto>>(url);

			return result.IsSuccess
				? Result<IEnumerable<TripListItemDto>>.Success(result.Value!.Items)
				: Result<IEnumerable<TripListItemDto>>.Fail(result.Error!);
		}

		public Task<Result<TripDto>> GetTrip(Guid id)
			=> _http.GetResultAsync<TripDto>($"api/trips/{id}");

		public Task<Result<GETTripOverview>> GetTripsOverview()
			=> _http.GetResultAsync<GETTripOverview>("api/trips/overview");

		public Task<Result> AddTrip(CreateTripRequest trip)
			=> _http.PostResultAsync("api/trips", trip);

		public Task<Result> UpdateTrip(UpdateTripRequest trip)
			=> _http.PutResultAsync($"api/trips/{trip.Id}", trip);

		public Task<Result> DeleteTrip(Guid id)
			=> _http.DeleteResultAsync($"api/trips/{id}");

		public Task<Result<DestinationDto>> AddDestination(Guid tripId, CreateDestinationRequest request)
			=> _http.PostResultAsync<DestinationDto>($"api/trips/{tripId}/destination", request);

		public Task<Result<DestinationDto>> UpdateDestination(Guid tripId, UpdateDestinationRequest request)
			=> _http.PutResultAsync<DestinationDto>($"api/trips/{tripId}/destination", request);

		public Task<Result> DeleteDestination(Guid tripId, Guid destinationId)
			=> _http.DeleteResultAsync($"api/trips/{tripId}/destination/{destinationId}");
	}

}
