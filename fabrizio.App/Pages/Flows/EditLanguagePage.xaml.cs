using fabrizio.App.Services;
using fabrizio.App.ViewModels;

namespace fabrizio.App.Pages.Flows
{

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

	}
}
