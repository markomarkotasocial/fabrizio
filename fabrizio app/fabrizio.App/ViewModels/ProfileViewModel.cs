using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Pages.Auth;
using fabrizio.DTO;

namespace fabrizio.App.Services
{
	public class ProfileViewModel
	{
		private readonly AuthService _authService;

		public AsyncRelayCommand LogoutCommand { get; }

		public ProfileViewModel(AuthService authService)
		{
			_authService = authService;
			LogoutCommand = new AsyncRelayCommand(LogoutAsync);
		}

		private async Task LogoutAsync()
		{
			await _authService.LogoutAsync();
		}
	}

}
