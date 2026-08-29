using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Resources.Lookups;
using fabrizio.App.Services.Abstractions;
using fabrizio.App.Services;
using fabrizio.Shared.DTO;
using System.Collections.ObjectModel;

namespace fabrizio.App.ViewModels
{
	public partial class EditCurrencyViewModel : BaseViewModel
	{
		private readonly IAccountState _accountState;
		private readonly IProfileService _profileService;


		[ObservableProperty] private string selectedCurrency;

		public ObservableCollection<CurrencyOption> Currencies { get; } = new(CurrencyData.All);


		public string Title => "Select your currency";


		public EditCurrencyViewModel(IAccountState accountState, IProfileService profileService)
		{
			_profileService = profileService;
			_accountState = accountState;

			SelectedCurrency = _accountState.Account?.PreferredCurrency;

			ReorderCurrencies();
			MarkSelectedCurrency();
		}




		partial void OnSelectedCurrencyChanged(string value)
		{
			MarkSelectedCurrency();
		}
		private void MarkSelectedCurrency()
		{
			foreach (var currency in Currencies)
				currency.IsSelected = currency.Code == SelectedCurrency;
		}





		[RelayCommand]
		private async Task SelectCurrency(CurrencyOption currency)
		{
			if (IsBusy || currency == null) return;

			var acc = _accountState.Account;
			if (acc == null) return;

			try
			{
				IsBusy = true;

				var request = new UpdateAccountProfileRequest
				{
					Name = acc.Name,
					PreferredLanguage = acc.PreferredLanguage,
					PreferredCurrency = currency.Code,
					TimeZone = acc.TimeZone
				};

				var result = await _profileService.UpdateAccount(request);
				if (!result.IsSuccess) return;

				// ✅ single source of truth
				SelectedCurrency = currency.Code;

				// ✅ update global state
				acc.PreferredCurrency = currency.Code;

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




		private void ReorderCurrencies()
		{
			if (string.IsNullOrEmpty(SelectedCurrency))	return;

			var selected = CurrencyData.All.FirstOrDefault(x => x.Code == SelectedCurrency);

			if (selected == null) return;

			var rest = CurrencyData.All.Where(x => x.Code != SelectedCurrency);

			Currencies.Clear();
			Currencies.Add(selected);

			foreach (var currency in rest) Currencies.Add(currency);
		}

	}
}
