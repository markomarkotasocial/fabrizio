using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using fabrizio.App.Services;

namespace fabrizio.App
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
			await _viewModel.LoadCommand.ExecuteAsync(null);
		}

	}
}
