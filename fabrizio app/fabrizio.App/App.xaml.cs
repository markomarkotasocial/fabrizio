using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Maui.Storage;

using fabrizio.App.Pages.Auth;

namespace fabrizio.App
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new ContentPage
			{
				Content = new ActivityIndicator
				{
					IsRunning = true,
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.Center
				}
			};

			InitializeAsync();
		}

		private async void InitializeAsync()
		{
			try
			{
				var token = await SecureStorage.GetAsync("jwt_token");

				if (!string.IsNullOrWhiteSpace(token))
				{
					// user je već logiran
					MainPage = new AppShell();
				}
				else
				{
					// nema tokena → login
					MainPage = new LoginPage();
				}
			}
			catch
			{
				// SecureStorage može baciti exception (npr. prvi run)
				MainPage = new LoginPage();
			}
		}


	}
}
