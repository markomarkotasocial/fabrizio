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
		Task<Result<AccountDto>> GetAccount();
		Task<Result> UpdateAccount(UpdateAccountProfileRequest request);
	}


	public class ProfileService : IProfileService
	{
		private readonly HttpClient _http;
		
		public ProfileService(HttpClient httpClient)
		{
			_http = httpClient;
		}

		public async Task<Result<AccountDto>> GetAccount()
		{
			try
			{
				var result = await _http.GetFromJsonAsync<Result<AccountDto>>("api/accounts/info");

				if (result == null)
				{
					return Result<AccountDto>.Fail(new BusinessError("network_error", "Unable to reach server.", 0));
				}

				if (!result.IsSuccess)
				{
					return Result<AccountDto>.Fail(result.Error!);
				}

				return Result<AccountDto>.Success(result.Value!);
			}
			catch (Exception)
			{
				return Result<AccountDto>.Fail(new BusinessError("network_error", "Unable to reach server.", 0));
			}
		}


		public async Task<Result> UpdateAccount(UpdateAccountProfileRequest request)
		{
			try
			{
				var response = await _http.PutAsJsonAsync("api/accounts/info", request);
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

	}
}
