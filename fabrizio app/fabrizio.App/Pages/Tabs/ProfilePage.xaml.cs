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
	}
}
