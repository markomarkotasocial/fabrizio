using fabrizio.App.Services;
using fabrizio.App.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace fabrizio.App.Pages.Auth
{
	public partial class LoginPage : ContentPage
	{
		public LoginPage() : this(new LoginViewModel(App.AuthService))
		{
		}

		public LoginPage(LoginViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}

}
