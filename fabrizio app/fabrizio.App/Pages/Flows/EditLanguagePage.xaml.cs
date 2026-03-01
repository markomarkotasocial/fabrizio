using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using fabrizio.App.Services;

namespace fabrizio.App.Pages.Flows
{
	public partial class EditLanguagePage : ContentPage
	{
		private readonly EditLanguageViewModel _viewModel;



		public EditLanguagePage()
		{
			InitializeComponent();
		}

		public EditLanguagePage(EditLanguageViewModel viewModel)
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
