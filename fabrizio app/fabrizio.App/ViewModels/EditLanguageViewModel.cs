using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Resources.Lookups;
using fabrizio.App.ViewModels;
using fabrizio.Shared.DTO;
using System.Collections.ObjectModel;

namespace fabrizio.App.Services
{
	public partial class EditLanguageViewModel : BaseViewModel
	{
		private readonly IProfileService _profileService;



		[ObservableProperty] private string selectedLanguage;


		public ObservableCollection<LanguageOption> Languages { get; } = new(LanguageData.All);


		public EditLanguageViewModel(IProfileService profileService)
		{
			_profileService = profileService;

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


			foreach (var lang in Languages)
				lang.IsSelected = false;

			language.IsSelected = true;

			try
			{
				IsBusy = true;

				var request = new UpdateAccountProfileRequest
				{
					PreferredLanguage = language.Code
				};

				var result = await _profileService.UpdateAccount(request);
				if (!result.IsSuccess) return;

				// ✅ lokalni update (VAŽNO za checkmark)
				SelectedLanguage = language.Code;

				// ✅ refresh svih itema
				foreach (var lang in Languages)
					lang.IsSelected = lang.Code == SelectedLanguage;

				// ✅ povratak na ProfilePage
				await Shell.Current.GoToAsync("..");
			}
			finally
			{
				IsBusy = false;
			}
		}

	}
}
