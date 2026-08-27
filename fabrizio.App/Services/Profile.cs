using fabrizio.Shared.Contracts;
using fabrizio.Shared.DTO;
using System.Net.Http;
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

		public Task<Result<AccountDto>> GetAccount()
			=> _http.GetResultAsync<AccountDto>("api/accounts/info");

		public Task<Result> UpdateAccount(UpdateAccountProfileRequest request)
			=> _http.PutResultAsync("api/accounts/info", request);
	}
}
