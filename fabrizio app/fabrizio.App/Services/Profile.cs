using fabrizio.Shared.Contracts;
using fabrizio.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.App.Services
{
	public interface IProfileService
	{
		Task<Result<GETAccount>> GetAccount();
	}


	public class ProfileService : IProfileService
	{
		private readonly HttpClient _http;
		
		public ProfileService(HttpClient httpClient)
		{
			_http = httpClient;
		}

		public async Task<Result<GETAccount>> GetAccount()
		{
			var result = await _http.GetFromJsonAsync<Result<GETAccount>>($"api/accounts/info");

			if (result == null)
			{
				return Result<GETAccount>.Fail(new BusinessError("network_error", "Unable to reach server.", 0));
			}

			if (!result.IsSuccess)
			{
				return Result<GETAccount>.Fail(result.Error!);
			}

			return Result<GETAccount>.Success(result.Value!);
		}


		public async Task<Result> UpdateAccount(UpdateAccountProfileRequest request)
		{
			var response = await _http.PutAsJsonAsync("api/accounts/info", request);

			if (!response.IsSuccessStatusCode)
			{
				var errorResult = await response.Content.ReadFromJsonAsync<Result>();
				return Result.Fail(errorResult?.Error ?? new BusinessError("unknown_error", "An unknown error occurred.", (int)response.StatusCode));
			}

			return Result.Success();
		}

	}
}
