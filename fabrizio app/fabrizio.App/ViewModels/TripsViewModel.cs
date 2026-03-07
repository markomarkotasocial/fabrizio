using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Pages.Flows;
using fabrizio.App.ViewModels;
using fabrizio.Shared.DTO;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace fabrizio.App.Services
{
	public partial class TripsViewModel : BaseViewModel
	{
		private readonly ITripService _tripService;
		private readonly IAuthService _authService;


		public ObservableCollection<TripListItemDto> Trips { get; } = new();

		[ObservableProperty] string selectedFilter;
		[ObservableProperty] private bool isRefreshing;


		public AsyncRelayCommand RefreshCommand { get; }
		public AsyncRelayCommand LoadCommand { get; }

		public AsyncRelayCommand AddTripCommand { get; }
		public AsyncRelayCommand<TripListItemDto> DeleteTripCommand { get; }
		public AsyncRelayCommand<TripListItemDto> OpenTripCommand { get; }
		public IRelayCommand<string> SetFilterCommand { get; }


		public bool IsEmpty => !IsBusy && !IsRefreshing && Trips.Count == 0;

		private bool _isInitialized;

		public List<string> Filters { get; } = new() { "All", "Upcoming", "Past" };


		public TripsViewModel(ITripService tripService, IAuthService authService)
		{
			_tripService = tripService;
			_authService = authService;

			SelectedFilter = "All";

			LoadCommand = new AsyncRelayCommand(LoadInitialAsync);
			RefreshCommand = new AsyncRelayCommand(RefreshAsync, () => !IsRefreshing);
			AddTripCommand = new AsyncRelayCommand(OnAddTripAsync);			
			OpenTripCommand = new AsyncRelayCommand<TripListItemDto>(OpenTripDetailAsync);
			DeleteTripCommand = new AsyncRelayCommand<TripListItemDto>(DeleteTripAsync);
			SetFilterCommand = new RelayCommand<string>(SetFilter);

			// if something happen to Trips collection (add, delete, clear) => recalculate IsEmpty
			Trips.CollectionChanged += (_, __) => {	OnPropertyChanged(nameof(IsEmpty));	};
			this.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(IsRefreshing))	RefreshCommand.NotifyCanExecuteChanged(); };
		}


		private void SetFilter(string filter)
		{
			if (SelectedFilter == filter)
				return;

			SelectedFilter = filter;
		}
		partial void OnSelectedFilterChanged(string value)
		{
			_ = ApplyFilter();
		}
		private async Task ApplyFilter()
		{
			DateTime? startDate = null;
			DateTime? endDate = null;

			if (SelectedFilter == "Upcoming")
			{
				startDate = DateTime.Today;
			}
			else if (SelectedFilter == "Past")
			{
				endDate = DateTime.Today;
			}

			await LoadTripsCoreAsync(startDate, endDate);
		}




		public async Task EnsureLoadedAsync()
		{
			if (_isInitialized)	return;
			_isInitialized = true;
			await LoadInitialAsync();
		}

		public async Task LoadInitialAsync()
		{
			if (IsBusy) return;

			try
			{
				IsBusy = true;
				await LoadTripsCoreAsync(null, null);
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
				await LoadTripsCoreAsync(null, null);
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

		private async Task LoadTripsCoreAsync(DateTime? startDate = null, DateTime? endDate = null)
		{
			Trips.Clear();

			var result = await _tripService.GetTrips(startDate, endDate);

			if (!result.IsSuccess)
			{
				return;
			}

			foreach (var t in result.Value!)
			{
				Trips.Add(t);
			}
		}






		private Task OnAddTripAsync()
		{
			return Shell.Current.GoToAsync("trip-form");
		}


		// double tap protection
		private bool _isOpeningDetail;
		private async Task OpenTripDetailAsync(TripListItemDto trip)
		{
			if (trip == null || _isOpeningDetail) return;
			try
			{
				_isOpeningDetail = true;
				await Shell.Current.GoToAsync("trip-detail", new Dictionary<string, object>
				{
					["tripId"] = trip.Id
				});
			}
			finally
			{
				_isOpeningDetail = false;
			}
		}

		private async Task DeleteTripAsync(TripListItemDto trip)
		{
			await _tripService.DeleteTrip(trip.Id);
		}

	}
}
