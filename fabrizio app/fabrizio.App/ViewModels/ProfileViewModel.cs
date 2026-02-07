using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Pages.Auth;
using fabrizio.App.ViewModels;
using fabrizio.Shared.DTO;

namespace fabrizio.App.Services
{
	public partial class ProfileViewModel : BaseViewModel
	{
		private readonly IAuthService _authService;


		[ObservableProperty] private string errorMessage;


		public AsyncRelayCommand LogoutCommand { get; }

		public ProfileViewModel(AuthService authService)
		{
			_authService = authService;
			LogoutCommand = new AsyncRelayCommand(LogoutAsync);
		}

		private async Task LogoutAsync()
		{
			if (IsBusy) return;
			IsBusy = true;

			try
			{
				await _authService.LogoutAsync();
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
