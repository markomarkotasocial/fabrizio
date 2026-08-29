using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Resources.Lookups;
using fabrizio.App.Services.Abstractions;
using fabrizio.App.Services;
using fabrizio.Shared.DTO;
using System.Collections.ObjectModel;

namespace fabrizio.App.ViewModels
{

	[QueryProperty(nameof(StartDate), "startDate")]
	[QueryProperty(nameof(EndDate), "endDate")]
	public partial class DateRangeViewModel : BaseViewModel
	{

		[ObservableProperty]
		DateTime startDate = DateTime.Today;

		[ObservableProperty]
		DateTime endDate = DateTime.Today.AddDays(1);



		public DateRangeViewModel()
		{

		}



		[RelayCommand]
		async Task Apply()
		{
			var parameters = new Dictionary<string, object>
			{
				{ "selectedStartDate", StartDate },
				{ "selectedEndDate", EndDate }
			};

			await Shell.Current.GoToAsync("..", parameters);
		}

		[RelayCommand]
		async Task Cancel()
		{
			await Shell.Current.GoToAsync("..");
		}





	}
}
