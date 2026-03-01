using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Pages.Auth;
using fabrizio.App.ViewModels;
using fabrizio.App.Resources.Lookups;
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
		public AsyncRelayCommand EditLanguageCommand { get; }


		public bool IsEmpty => !IsBusy && !IsRefreshing && Account == null;
		public bool IsNotEditingName => !IsEditingName;
		public bool HasAccount => Account != null;

		public string PreferredLanguageDisplay => LanguageData.All.FirstOrDefault(x => x.Code == PreferredLanguage)?.Name ?? PreferredLanguage ?? string.Empty;


		public ProfileViewModel(IProfileService profileService, AuthService authService)
		{
			_authService = authService;
			_profileService = profileService;

			LoadCommand = new AsyncRelayCommand(LoadInitialAsync);
			LogoutCommand = new AsyncRelayCommand(LogoutAsync);
			DeleteAccountCommand = new AsyncRelayCommand(DeleteAccountAsync);
			SaveAccountCommand = new AsyncRelayCommand(SaveAccountAsync);

			EditLanguageCommand = new AsyncRelayCommand(EditLanguageAsync);
		}



		partial void OnAccountChanged(AccountDto? value)
		{
			OnPropertyChanged(nameof(HasAccount));
			OnPropertyChanged(nameof(IsEmpty));
		}
		partial void OnIsEditingNameChanged(bool value)
		{
			OnPropertyChanged(nameof(IsNotEditingName));
		}		
		partial void OnPreferredLanguageChanged(string value)
		{
			OnPropertyChanged(nameof(PreferredLanguageDisplay));
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
					PreferredLanguage = PreferredLanguage,
					PreferredCurrency = PreferredCurrency,
					TimeZone = TimeZone
				};

				var result = await _profileService.UpdateAccount(request);

				if (!result.IsSuccess)
				{
					// rollback za sva polja koja editiraš
					Name = Account.Name;
					PreferredCurrency = Account.PreferredCurrency;
					PreferredLanguage = Account.PreferredLanguage;
					TimeZone = Account.TimeZone;
					return;
				}

				// optimistic sync
				Account.Name = Name;
				Account.PreferredLanguage = PreferredLanguage;
				Account.PreferredCurrency = PreferredCurrency;
				Account.TimeZone = TimeZone;
			}
			finally
			{
				IsBusy = false;
				IsEditingName = false;
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

		private Task EditLanguageAsync()
		{
			return Shell.Current.GoToAsync("edit-language");
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
