using fabrizio.App.Services;
using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;


namespace fabrizio.App.Pages.Tabs
{
	public partial class HomePage : ContentPage
	{
		private readonly HomeViewModel _viewModel;

		public HomePage(HomeViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = _viewModel = viewModel;

		}


		//private async void OnPageLoaded(object sender, EventArgs e)
		//{
		//	if (BindingContext is HomeViewModel vm)
		//	{
		//		await vm.EnsureLoadedAsync();
		//	}
		//}



	}
}
