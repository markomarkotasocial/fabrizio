using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace fabrizio.App
{
	public partial class LoginPage : ContentPage
	{

		private readonly HttpClient _httpClient;

		public LoginPage()
		{
			InitializeComponent();
			_httpClient = new HttpClient { BaseAddress = new Uri("https://fabrizio-ftdpcwhsh5enhscn.westeurope-01.azurewebsites.net/") };
		}


		private async void OnLoginClicked(object sender, EventArgs e)
		{
			MessageLabel.Text = string.Empty;

			var dto = new
			{
				email = EmailEntry.Text?.Trim(),
				password = PasswordEntry.Text
			};

			try
			{
				var response = await _httpClient.PostAsJsonAsync("api/accounts/login", dto);
				if (!response.IsSuccessStatusCode)
				{
					MessageLabel.Text = $"Login failed: {response.StatusCode}";
					return;
				}

				// Expect server to return { "token": "..." } or { "Token": "..." }
				var json = await response.Content.ReadAsStringAsync();
				using var doc = JsonDocument.Parse(json);

				if (doc.RootElement.TryGetProperty("token", out var tokenProp) ||
					doc.RootElement.TryGetProperty("Token", out tokenProp))
				{
					string token = tokenProp.GetString() ?? string.Empty;

					// store token securely
					await SecureStorage.SetAsync("jwt_token", token);

					// 🔑 switch to AppShell instead of pushing a page
					Application.Current.MainPage = new AppShell();

					// optionally navigate to HomePage explicitly
					await Shell.Current.GoToAsync("//Pages/HomePage");
				}
				else
				{
					MessageLabel.Text = "Login response missing token.";
				}
			}
			catch (Exception ex)
			{
				MessageLabel.Text = "Error: " + ex.Message;
			}
		}
	}
}
