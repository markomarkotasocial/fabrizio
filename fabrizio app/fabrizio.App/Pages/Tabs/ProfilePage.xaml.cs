using fabrizio.App.Pages.Auth;
using fabrizio.App.Services;
using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace fabrizio.App.Pages.Tabs
{
	public partial class ProfilePage : ContentPage
	{
		public ProfilePage()
		{
			InitializeComponent();
		}

		private async void OnLogoutClicked(object sender, EventArgs e)
		{
			await App.AuthService.LogoutAsync();
		}
	}
}
