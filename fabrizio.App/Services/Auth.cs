using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using fabrizio.Shared.DTO;

namespace fabrizio.App.Services
{
	public interface IAuthService
	{
		Task LoginAsync(string email, string password);
		Task LogoutAsync();
		Task<bool> IsAuthenticatedAsync();
	}




	public class AuthService : IAuthService
	{
		// Standalone client: login must not go through the TokenHandler pipeline
		// (no token yet, and a 401 here means bad credentials, not an expired session).
		private readonly HttpClient _httpClient = new HttpClient
		{
			BaseAddress = new Uri("https://fabrizio-ftdpcwhsh5enhscn.westeurope-01.azurewebsites.net/")
		};

		private readonly INavigationService _navigation;

		private bool _isLoggingOut;

		public AuthService(INavigationService navigation)
		{
			_navigation = navigation;
		}

		public async Task LogoutAsync()
		{
			if (_isLoggingOut) return;
			_isLoggingOut = true;

			try
			{
				SecureStorage.Remove("jwt_token");
				_navigation.GoToLogin();
			}
			finally
			{
				_isLoggingOut = false;
			}
		}

		public async Task LoginAsync(string email, string password)
		{
			var response = await _httpClient.PostAsJsonAsync("api/accounts/login", new { email, password });

			if (!response.IsSuccessStatusCode) throw new Exception("Invalid credentials");

			var body = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
			if (string.IsNullOrWhiteSpace(body?.Token)) throw new Exception("Login response missing token");

			await SecureStorage.SetAsync("jwt_token", body.Token);
			_navigation.GoToApp();
		}

		public async Task<bool> IsAuthenticatedAsync()
		{
			var token = await SecureStorage.GetAsync("jwt_token");
			return !string.IsNullOrWhiteSpace(token);
		}
	}
}
