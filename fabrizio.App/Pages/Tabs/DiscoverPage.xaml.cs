using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Maui.Storage;

namespace fabrizio.App.Pages.Tabs
{
	public partial class DiscoverPage : ContentPage
	{
		private readonly HttpClient _httpClient;

		public DiscoverPage()
		{
			InitializeComponent();
			_httpClient = new HttpClient { BaseAddress = new Uri("https://fabrizio-ftdpcwhsh5enhscn.westeurope-01.azurewebsites.net/") };
		}



		//private async void OnFetchTripsClicked(object sender, EventArgs e)
		//{
		//	try
		//	{
		//		var token = await SecureStorage.GetAsync("jwt_token");
		//		if (string.IsNullOrEmpty(token))
		//		{
		//			//ResultLabel.Text = "No token found. Please login.";
		//			return;
		//		}

		//		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		//		var response = await _httpClient.GetAsync("api/trips");
		//		if (!response.IsSuccessStatusCode)
		//		{
		//			//ResultLabel.Text = $"Error: {response.StatusCode}";
		//			return;
		//		}

		//		var json = await response.Content.ReadAsStringAsync();
		//		//ResultLabel.Text = json;
		//	}
		//	catch (Exception ex)
		//	{
		//		//ResultLabel.Text = "Error: " + ex.Message;
		//	}
		//}
	}
}
