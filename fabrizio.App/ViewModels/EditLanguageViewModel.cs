using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Resources.Lookups;
using fabrizio.App.Services.Abstractions;
using fabrizio.App.Services;
using fabrizio.Shared.DTO;
using System.Collections.ObjectModel;

namespace fabrizio.App.ViewModels
{
	public partial class EditLanguageViewModel : BaseViewModel
	{
		private readonly IAccountState _accountState;
		private readonly IProfileService _profileService;


		[ObservableProperty] private string selectedLanguage;

		public ObservableCollection<LanguageOption> Languages { get; } = new(LanguageData.All);


		public string Title => "Select your language";

		public EditLanguageViewModel(IAccountState accountState, IProfileService profileService)
		{
			_profileService = profileService;
			_accountState = accountState;

			SelectedLanguage = _accountState.Account?.PreferredLanguage;

			ReorderLanguages();
			MarkSelectedLanguage();
		}




		partial void OnSelectedLanguageChanged(string value)
		{
			MarkSelectedLanguage();
		}
		private void MarkSelectedLanguage()
		{
			foreach (var lang in Languages)
				lang.IsSelected = lang.Code == SelectedLanguage;
		}





		[RelayCommand]
		private async Task SelectLanguage(LanguageOption language)
		{
			if (IsBusy || language == null) return;

			var acc = _accountState.Account;
			if (acc == null) return;

			try
			{
				IsBusy = true;

				var request = new UpdateAccountProfileRequest
				{
					Name = acc.Name,
					PreferredLanguage = language.Code,
					PreferredCurrency = acc.PreferredCurrency,
					TimeZone = acc.TimeZone
				};

				var result = await _profileService.UpdateAccount(request);
				if (!result.IsSuccess) return;

				// ✅ single source of truth
				SelectedLanguage = language.Code;

				// ✅ update global state
				acc.PreferredLanguage = language.Code;

				await Shell.Current.GoToAsync("..");
			}
			finally
			{
				IsBusy = false;
			}
		}

		[RelayCommand]
		public async Task Cancel()
		{
			await Shell.Current.GoToAsync("..");
		}


		private void ReorderLanguages()
		{
			if (string.IsNullOrEmpty(SelectedLanguage)) return;

			var selected = LanguageData.All.FirstOrDefault(x => x.Code == SelectedLanguage);

			if (selected == null) return;

			var rest = LanguageData.All.Where(x => x.Code != SelectedLanguage);

			Languages.Clear();
			Languages.Add(selected);

			foreach (var language in rest) Languages.Add(language);
		}

	}
}
