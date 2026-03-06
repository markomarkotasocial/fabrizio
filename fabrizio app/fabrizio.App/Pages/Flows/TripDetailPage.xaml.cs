using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using fabrizio.App.Services;

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


		//public Guid TripId
		//{
		//	set
		//	{
		//		if (value != Guid.Empty)
		//		{
		//			_viewModel.LoadTrip(value);
		//		}
		//	}
		//}


		//protected override void OnAppearing()
		//{
		//	base.OnAppearing();			
		//}

	}
}
