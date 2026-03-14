using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Pages.Auth;
using fabrizio.App.ViewModels;
using fabrizio.Shared.DTO;

namespace fabrizio.App.Services
{
	public partial class HomeViewModel : BaseViewModel
	{
		private readonly ITripService _tripService;
		private readonly IAuthService _authService;



		[ObservableProperty] private bool isRefreshing;

		[ObservableProperty] private TripDto? current;

		[ObservableProperty] private TripDto? next;



		public AsyncRelayCommand RefreshCommand { get; }
		public AsyncRelayCommand LoadCommand { get; }

		

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


		private bool _isInitialized;

		public HomeViewModel(ITripService tripService, AuthService authService)
		{
			_tripService = tripService;
			_authService = authService;

			LoadCommand = new AsyncRelayCommand(LoadInitialAsync);
			RefreshCommand = new AsyncRelayCommand(RefreshAsync);

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

		public async Task RefreshAsync()
		{
			try
			{
				await LoadOverviewCoreAsync();
			}
			catch (UnauthorizedException)
			{
				await _authService.LogoutAsync();
			}
			finally
			{
				IsRefreshing = false;
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
