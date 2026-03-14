using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Maui.Storage;

namespace fabrizio.App.Pages.Tabs
{
	public partial class HubPage : ContentPage
	{
		private readonly HttpClient _httpClient;

		public HubPage()
		{
			InitializeComponent();
			//_httpClient = new HttpClient { BaseAddress = new Uri("https://fabrizio-ftdpcwhsh5enhscn.westeurope-01.azurewebsites.net/") };
		}



	}
}
