using fabrizio.App.Pages.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace fabrizio.App.Services
{
	public class AuthService
	{
		private readonly HttpClient _httpClient;

		public AuthService(HttpClient httpClient)
		{
			_httpClient = new HttpClient
			{
				BaseAddress = new Uri("https://fabrizio-ftdpcwhsh5enhscn.westeurope-01.azurewebsites.net/")
			};
		}



		private bool _isLoggingOut;

		public async Task LogoutAsync()
		{
			if (_isLoggingOut) return;
			_isLoggingOut = true;

			try
			{
				SecureStorage.Remove("jwt_token");

				MainThread.BeginInvokeOnMainThread(() =>
				{
					Application.Current!.MainPage = new LoginPage();
				});
			}
			finally
			{
				_isLoggingOut = false;
			}
		}


		public async Task LoginAsync(string email, string password)
		{
			var response = await _httpClient.PostAsJsonAsync("api/accounts/login", new
			{
				email,
				password
			});

			if (!response.IsSuccessStatusCode) throw new Exception("Invalid credentials");

			var json = await response.Content.ReadAsStringAsync();
			using var doc = JsonDocument.Parse(json);

			if (!doc.RootElement.TryGetProperty("token", out var tokenProp) && !doc.RootElement.TryGetProperty("Token", out tokenProp))
			{
				throw new Exception("Login response missing token");
			}

			var token = tokenProp.GetString();
			if (string.IsNullOrWhiteSpace(token)) throw new Exception("Empty token");

			await SecureStorage.SetAsync("jwt_token", token);
		}

	}


}
