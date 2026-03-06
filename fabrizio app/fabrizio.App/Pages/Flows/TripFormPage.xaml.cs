using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using fabrizio.App.Services;

namespace fabrizio.App.Pages.Flows
{
	[QueryProperty(nameof(TripId), "tripId")]
	public partial class TripFormPage : ContentPage
	{
		private readonly TripFormViewModel _viewModel;

		public Guid TripId
		{
			set
			{
				if (value != Guid.Empty)
				{
					_viewModel.LoadTrip(value);
				}
			}
		}

		public TripFormPage(TripFormViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = _viewModel = viewModel;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();			
		}

	}
}
