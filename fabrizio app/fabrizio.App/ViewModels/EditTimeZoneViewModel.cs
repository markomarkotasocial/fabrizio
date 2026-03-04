using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Resources.Lookups;
using fabrizio.App.Services.Abstractions;
using fabrizio.App.ViewModels;
using fabrizio.Shared.DTO;
using System.Collections.ObjectModel;

namespace fabrizio.App.Services
{
	public partial class EditTimeZoneViewModel : BaseViewModel
	{
		private readonly IAccountState _accountState;
		private readonly IProfileService _profileService;


		[ObservableProperty] private string selectedTimeZone;


		public ObservableCollection<TimeZoneOption> TimeZones { get; } = new(TimeZoneData.All);


		public EditTimeZoneViewModel(IAccountState accountState, IProfileService profileService)
		{
			_profileService = profileService;
			_accountState = accountState;

			SelectedTimeZone = _accountState.Account?.TimeZone;

			ReorderTimeZones();
			MarkSelectedTimeZone();
		}




		partial void OnSelectedTimeZoneChanged(string value)
		{
			MarkSelectedTimeZone();
		}
		private void MarkSelectedTimeZone()
		{
			foreach (var tz in TimeZones) 
				tz.IsSelected = tz.Id == SelectedTimeZone;
		}





		[RelayCommand]
		private async Task SelectTimeZone(TimeZoneOption timezone)
		{
			if (IsBusy || timezone == null) return;

			var acc = _accountState.Account;
			if (acc == null) return;

			try
			{
				IsBusy = true;

				var request = new UpdateAccountProfileRequest
				{
					Name = acc.Name,
					PreferredLanguage = acc.PreferredLanguage,
					PreferredCurrency = acc.PreferredCurrency,
					TimeZone = timezone.Id
				};

				var result = await _profileService.UpdateAccount(request);
				if (!result.IsSuccess) return;

				// ✅ single source of truth
				SelectedTimeZone = timezone.Id;

				// ✅ update global state
				acc.PreferredLanguage = timezone.Id;

				await Shell.Current.GoToAsync("..");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private void ReorderTimeZones()
		{
			if (string.IsNullOrEmpty(SelectedTimeZone)) return;

			var selected = TimeZoneData.All.FirstOrDefault(x => x.Id == SelectedTimeZone);

			if (selected == null) return;

			var rest = TimeZoneData.All.Where(x => x.Id != SelectedTimeZone);

			TimeZones.Clear();
			TimeZones.Add(selected);

			foreach (var language in rest) TimeZones.Add(language);
		}

	}
}
