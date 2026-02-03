using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Pages.Auth;
using fabrizio.DTO;

namespace fabrizio.App.Services
{
	public partial class LoginViewModel : ObservableObject
	{
		private readonly IAuthService _authService;


		[ObservableProperty] private string email;
		[ObservableProperty] private string password;
		[ObservableProperty] private string errorMessage;
		[ObservableProperty] private bool isBusy;


		public AsyncRelayCommand LoginCommand { get; }

		public LoginViewModel(AuthService authService)
		{
			_authService = authService;
			LoginCommand = new AsyncRelayCommand(LoginAsync, () => !IsBusy);
		}

		private async Task LoginAsync()
		{
			if (IsBusy) return;
			IsBusy = true;

			try
			{
				await _authService.LoginAsync(Email, Password);
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
			finally
			{
				IsBusy = false;
			}

		}
	}
}
