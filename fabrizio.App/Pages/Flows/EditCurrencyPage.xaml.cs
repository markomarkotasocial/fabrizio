using fabrizio.App.Services;
using fabrizio.App.ViewModels;

namespace fabrizio.App.Pages.Flows
{

	public partial class EditCurrencyPage : ContentPage
	{
		private readonly EditCurrencyViewModel _viewModel;

		
		public EditCurrencyPage(EditCurrencyViewModel viewModel)
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
