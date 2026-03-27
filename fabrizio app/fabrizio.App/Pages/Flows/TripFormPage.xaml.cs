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



		private async void OnDestinationUnfocused(object sender, FocusEventArgs e)
		{
			if (sender is not Entry entry) return;
			if (entry.BindingContext is not DestinationItemViewModel item) return;

			if (string.IsNullOrWhiteSpace(item.Name))
				return;

			var vm = (TripFormViewModel)BindingContext;

			if (item.IsNew)
				await vm.CreateDestinationAsync(item);
			else
				await vm.UpdateDestinationAsync(item);
		}



	}
}
