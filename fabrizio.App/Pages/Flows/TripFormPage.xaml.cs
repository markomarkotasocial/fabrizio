using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using fabrizio.App.Services;
using fabrizio.App.ViewModels;

namespace fabrizio.App.Pages.Flows
{
	public partial class TripFormPage : ContentPage
	{
		private readonly TripFormViewModel _viewModel;

		public TripFormPage(TripFormViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = _viewModel = viewModel;

			//_viewModel.RequestFocus = (item) =>
			//{
			//	MainThread.BeginInvokeOnMainThread(() =>
			//	{
			//		var entry = FindEntryForItem(item);
			//		entry?.Focus();
			//	});
			//};
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();			
		}



		private async void OnDestinationUnfocused(object sender, FocusEventArgs e)
		{
			if (sender is not Entry entry) return;
			if (entry.BindingContext is not DestinationItemViewModel item) return;

			var vm = (TripFormViewModel)BindingContext;

			// Read straight off the Entry: the TwoWay binding may not have pushed
			// the latest text into item.Name yet by the time Unfocused fires.
			var name = entry.Text?.Trim() ?? string.Empty;
			item.Name = name;

			if (string.IsNullOrWhiteSpace(name))
			{
				if (item.IsNew)
					vm.Destinations.Remove(item);
				return;
			}

			if (item.IsNew)
				await vm.CreateDestinationAsync(item);
			else
				await vm.UpdateDestinationAsync(item);
		}

		private void OnEntryLoaded(object sender, EventArgs e)
		{
			if (sender is Entry entry && entry.BindingContext is DestinationItemViewModel item && item.IsNew)
			{
				entry.Focus();
			}
		}





	}
}
