using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Maui.Storage;

namespace fabrizio.App
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
			MainPage = new NavigationPage(new LoginPage());
		}
	}
}
