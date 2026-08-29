using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Pages.Auth;
using fabrizio.App.Services;
using fabrizio.Shared.DTO;

namespace fabrizio.App.ViewModels
{
	public partial class HomeViewModel : BaseViewModel
	{
		private readonly ITripService _tripService;



		[ObservableProperty] private bool isRefreshing;

		[ObservableProperty] private TripDto? current;

		[ObservableProperty] private TripDto? next;




		public bool ShowCurrentSplash => Current != null;
		public bool ShowNextFollower => Current != null && Next != null;
		public bool ShowNextSplash => Current == null && Next != null;	
		public bool IsEmpty => !IsBusy && !IsRefreshing && Current == null && Next == null;
		public bool HasAnyTrip => Current != null || Next != null;
		public string CurrentDateRangeText => Current == null ? string.Empty: $"{Current.StartDate:dd MMM} — {Current.EndDate:dd MMM}";

		public string NextTripCountdownText
		{
			get
			{
				if (Next?.StartDate == null) return string.Empty;

				var today = DateTime.Today;
				var days = (Next.StartDate.Value.Date - today).Days;

				if (days > 0) return $"{days}"; 
				else return string.Empty;
			}
		}


		public HomeViewModel(ITripService tripService)
		{
			_tripService = tripService;
		}



		[RelayCommand]
		public async Task Refresh()
		{
			try
			{
				await LoadOverviewCoreAsync();
			}
			finally
			{
				IsRefreshing = false;
				OnPropertyChanged(nameof(IsEmpty));

			}
		}


		[RelayCommand]
		private async Task Load()
		{
			if (IsBusy) return;

			try
			{
				IsBusy = true;
				await LoadOverviewCoreAsync();
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







		partial void OnCurrentChanged(TripDto? value)
		{
			OnPropertyChanged(nameof(HasAnyTrip));
			OnPropertyChanged(nameof(ShowCurrentSplash));
			OnPropertyChanged(nameof(ShowNextFollower));
			OnPropertyChanged(nameof(ShowNextSplash));
			OnPropertyChanged(nameof(IsEmpty));
			OnPropertyChanged(nameof(CurrentDateRangeText));
		}
		partial void OnNextChanged(TripDto? value)
		{
			OnPropertyChanged(nameof(HasAnyTrip));
			OnPropertyChanged(nameof(ShowNextFollower));
			OnPropertyChanged(nameof(ShowNextSplash));
			OnPropertyChanged(nameof(IsEmpty));
			OnPropertyChanged(nameof(NextTripCountdownText));
		}


		public async Task LoadOnEnterAsync()
		{
			if (IsBusy) return;

			try
			{
				IsBusy = true;
				await LoadOverviewCoreAsync();
			}
			finally
			{
				IsBusy = false;
				OnPropertyChanged(nameof(IsEmpty));
			}
		}

		

		public async Task LoadOverviewCoreAsync()
		{
			var result = await _tripService.GetTripsOverview();

			if (!result.IsSuccess)
			{
				Current = null;
				Next = null;
				return;
			}

			if (result.Value?.Current != null)
			{
				Current = result.Value.Current;
			}

			if (result.Value?.Next != null)
			{
				Next = result.Value.Next;

			}
		}



	}


}
