using fabrizio.App.ViewModels;

namespace fabrizio.App.Pages.Auth
{
	public partial class LoginPage : ContentPage
	{
		public LoginPage(LoginViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}
}
