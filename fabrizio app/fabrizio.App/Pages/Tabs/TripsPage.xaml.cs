using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using fabrizio.App.Services;

namespace fabrizio.App.Pages.Tabs
{
	public partial class TripsPage : ContentPage
	{
		private readonly TripsViewModel _viewModel;

		public TripsPage(TripsViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = _viewModel = viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			await _viewModel.EnsureLoadedAsync();
		}

		private async void OnChipTapped(object sender, TappedEventArgs e)
		{
			if (sender is not Frame frame) return;

			await frame.ScaleTo(0.92, 70);
			await frame.ScaleTo(1.0, 70);
		}


	}
}
