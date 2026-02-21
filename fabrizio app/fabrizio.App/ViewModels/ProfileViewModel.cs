using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Pages.Auth;
using fabrizio.App.ViewModels;
using fabrizio.Shared.DTO;

namespace fabrizio.App.Services
{
	public partial class ProfileViewModel : BaseViewModel
	{
		private readonly IProfileService _profileService;
		private readonly IAuthService _authService;


		[ObservableProperty] private bool isRefreshing;
		[ObservableProperty] private string errorMessage;
		[ObservableProperty] private AccountDto? account;

		[ObservableProperty] private string name;
		[ObservableProperty] private bool isEditingName;

		[ObservableProperty] private string preferredLanguage;
		[ObservableProperty] private string preferredCurrency;
		[ObservableProperty] private string timeZone;


		public AsyncRelayCommand LogoutCommand { get; }
		public AsyncRelayCommand DeleteAccountCommand { get; }
		public AsyncRelayCommand LoadCommand { get; }

		public AsyncRelayCommand SaveAccountCommand { get; }


		public bool IsEmpty => !IsBusy && !IsRefreshing && Account == null;


		public ProfileViewModel(IProfileService profileService, AuthService authService)
		{
			_authService = authService;
			_profileService = profileService;

			LoadCommand = new AsyncRelayCommand(LoadInitialAsync);
			LogoutCommand = new AsyncRelayCommand(LogoutAsync);
			DeleteAccountCommand = new AsyncRelayCommand(DeleteAccountAsync);
			SaveAccountCommand = new AsyncRelayCommand(SaveAccountAsync);
		}



		private async Task SaveAccountAsync()
		{
			if (IsBusy || Account == null) return;

			try
			{
				IsBusy = true;

				var request = new UpdateAccountProfileRequest
				{
					Name = Name,
					PreferredLanguage = Account.PreferredLanguage,
					PreferredCurrency = Account.PreferredCurrency,
					TimeZone = Account.TimeZone
				};

				//var result = await _profileService.UpdateAccount(request);

				//if (!result.IsSuccess)
				//{
				//	// rollback za sva polja koja editiraš
				//	Name = Account.Name;
				//	return;
				//}

				//// optimistic sync
				//Account.Name = Name;
			}
			finally
			{
				IsBusy = false;
				//IsEditingName = false;
			}
		}


		public async Task LoadOnEnterAsync()
		{
			if (IsBusy) return;

			try
			{
				IsBusy = true;
				await LoadOverviewCoreAsync();
			}
			catch (UnauthorizedException)
			{
				await _authService.LogoutAsync();
			}
			finally
			{
				IsBusy = false;
				OnPropertyChanged(nameof(IsEmpty));
			}
		}

		private async Task LoadInitialAsync()
		{
			if (IsBusy) return;

			try
			{
				IsBusy = true;
				await LoadOverviewCoreAsync();
			}
			catch (UnauthorizedException)
			{
				await _authService.LogoutAsync();
			}
			finally
			{
				MainThread.BeginInvokeOnMainThread(() =>
				{
					IsBusy = false;
					OnPropertyChanged(nameof(IsEmpty));
				});
			}
		}

		public async Task LoadOverviewCoreAsync()
		{
			var result = await _profileService.GetAccount();

			if (!result.IsSuccess)
			{
				Account = null;
				OnPropertyChanged(nameof(IsEmpty));
				return;
			}

			if (result.Value != null)
			{
				Account = result.Value;

				Name = Account.Name;
				IsEditingName = false;

				PreferredCurrency = Account.PreferredCurrency;
				PreferredLanguage = Account.PreferredLanguage;
				TimeZone = Account.TimeZone;

				OnPropertyChanged(nameof(IsEmpty));
			}
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

		private async Task DeleteAccountAsync()
		{
			
		}

	}

}
