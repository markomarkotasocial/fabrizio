using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using fabrizio.App.Services;
using fabrizio.App.ViewModels;

namespace fabrizio.App.Pages.Flows
{
	//[QueryProperty(nameof(TripId), "tripId")]
	public partial class TripDetailPage : ContentPage
	{
		private readonly TripDetailViewModel _viewModel;

		public TripDetailPage(TripDetailViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = _viewModel = viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			if (BindingContext is TripDetailViewModel vm && vm.TripId != Guid.Empty)
			{
				await vm.Load(vm.TripId);
			}
		}

	}
}
