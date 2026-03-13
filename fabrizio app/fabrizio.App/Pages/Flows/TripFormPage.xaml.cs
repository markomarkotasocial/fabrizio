using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using fabrizio.App.Services;

namespace fabrizio.App.Pages.Flows
{
	public partial class TripFormPage : ContentPage
	{
		private readonly TripFormViewModel _viewModel;

		public TripFormPage(TripFormViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = _viewModel = viewModel;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();			
		}



		private async void OnStartDateSelected(object sender, DateChangedEventArgs e)
		{
			EndDatePicker.MinimumDate = e.NewDate;

			if (EndDatePicker.Date < e.NewDate)
				EndDatePicker.Date = e.NewDate;

			// mali delay da se zatvori prvi picker
			await Task.Delay(150);

			EndDatePicker.Focus();
		}

	}
}
