using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Pages.Auth;
using fabrizio.DTO;

namespace fabrizio.App.Services
{
	public partial class ProfileViewModel : ObservableObject
	{
		private readonly IAuthService _authService;


		[ObservableProperty] private string errorMessage;
		[ObservableProperty] private bool isBusy;



		public AsyncRelayCommand LogoutCommand { get; }

		public ProfileViewModel(AuthService authService)
		{
			_authService = authService;
			LogoutCommand = new AsyncRelayCommand(LogoutAsync);
		}

		private async Task LogoutAsync()
		{
			//await _authService.LogoutAsync();

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
