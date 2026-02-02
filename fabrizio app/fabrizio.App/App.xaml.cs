using fabrizio.App.Pages.Auth;
using fabrizio.App.Services;
using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace fabrizio.App
{
	public partial class App : Application
	{
		public static AuthService AuthService { get; private set; }

		public App(AuthService authService)
		{
			InitializeComponent();
			AuthService = authService;
			MainPage = new AppShell();
		}
	}


}
