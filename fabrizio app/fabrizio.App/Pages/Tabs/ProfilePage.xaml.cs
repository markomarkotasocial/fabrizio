using fabrizio.App.Pages.Auth;
using fabrizio.App.Services;
using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace fabrizio.App.Pages.Tabs
{
	public partial class ProfilePage : ContentPage
	{
		private readonly ProfileViewModel _viewModel;

		public ProfilePage(ProfileViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = _viewModel = viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (BindingContext is ProfileViewModel vm)
			{
				await vm.LoadOnEnterAsync();
			}
		}



		private void OnNameTapped(object sender, EventArgs e)
		{
			if (BindingContext is ProfileViewModel vm)
				vm.IsEditingName = true;
		}

		private async void OnNameCompleted(object sender, EventArgs e)
		{
			await TriggerSaveAsync();
		}

		private async void OnNameUnfocused(object sender, FocusEventArgs e)
		{
			await TriggerSaveAsync();
		}

		private async Task TriggerSaveAsync()
		{
			if (BindingContext is ProfileViewModel vm)
			{
				await vm.SaveAccountCommand.ExecuteAsync(null);
			}
		}

	}
}
