using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using fabrizio.App.Services;

namespace fabrizio.App.Pages.Flows
{

	[QueryProperty(nameof(CurrentLanguage), "currentLanguage")]
	public partial class EditLanguagePage : ContentPage
	{
		private readonly EditLanguageViewModel _viewModel;

		
		public EditLanguagePage(EditLanguageViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = _viewModel = viewModel;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();			
		}


		// set parameter for edit page
		public string CurrentLanguage
		{
			set
			{
				if (_viewModel != null)
				{
					_viewModel.SelectedLanguage = value;
				}
			}
		}




	}
}
