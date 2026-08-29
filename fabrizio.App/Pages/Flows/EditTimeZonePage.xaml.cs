using fabrizio.App.Services;
using fabrizio.App.ViewModels;

namespace fabrizio.App.Pages.Flows
{

	public partial class EditTimeZonePage : ContentPage
	{
		private readonly EditTimeZoneViewModel _viewModel;

		
		public EditTimeZonePage(EditTimeZoneViewModel viewModel)
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
