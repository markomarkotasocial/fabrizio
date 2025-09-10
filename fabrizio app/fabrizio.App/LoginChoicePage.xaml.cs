using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;

namespace fabrizio.App
{
	public partial class LoginChoicePage : ContentPage
	{
		public LoginChoicePage()
		{
			InitializeComponent();
		}

		private async void OnEmailLoginClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new LoginPage());
		}
	}
}
