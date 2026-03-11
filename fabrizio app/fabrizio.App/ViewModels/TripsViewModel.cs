using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Pages.Flows;
using fabrizio.App.ViewModels;
using fabrizio.Shared.DTO;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace fabrizio.App.Services
{
	public partial class FilterChip : ObservableObject
	{
		public string Name { get; set; }

		[ObservableProperty]
		private bool isSelected;
	}

	public partial class TripsViewModel : BaseViewModel
	{
		private readonly ITripService _tripService;
		private readonly IAuthService _authService;


		public ObservableCollection<TripListItemDto> Trips { get; } = new();
		public ObservableCollection<FilterChip> Filters { get; } = new()
		{
			new FilterChip { Name = "Upcoming", IsSelected = true },
			new FilterChip { Name = "Past" }, 
			new FilterChip { Name = "All" }
		};



		[ObservableProperty] string selectedFilter;
		[ObservableProperty] private bool isRefreshing;


		public AsyncRelayCommand RefreshCommand { get; }
		public AsyncRelayCommand LoadCommand { get; }

		public AsyncRelayCommand AddTripCommand { get; }
		public AsyncRelayCommand<TripListItemDto> DeleteTripCommand { get; }
		public AsyncRelayCommand<TripListItemDto> OpenTripCommand { get; }
		public IRelayCommand<FilterChip> SetFilterCommand { get; }


		public bool IsEmpty => !IsBusy && !IsRefreshing && Trips.Count == 0;

		private bool _isInitialized;



		public TripsViewModel(ITripService tripService, IAuthService authService)
		{
			_tripService = tripService;
			_authService = authService;

			LoadCommand = new AsyncRelayCommand(LoadInitialAsync);
			RefreshCommand = new AsyncRelayCommand(RefreshAsync, () => !IsRefreshing);
			AddTripCommand = new AsyncRelayCommand(OnAddTripAsync);			
			OpenTripCommand = new AsyncRelayCommand<TripListItemDto>(OpenTripDetailAsync);
			DeleteTripCommand = new AsyncRelayCommand<TripListItemDto>(DeleteTripAsync);
			SetFilterCommand = new RelayCommand<FilterChip>(SetFilter);

			// if something happen to Trips collection (add, delete, clear) => recalculate IsEmpty
			Trips.CollectionChanged += (_, __) => {	OnPropertyChanged(nameof(IsEmpty));	};
			this.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(IsRefreshing))	RefreshCommand.NotifyCanExecuteChanged(); };

			SelectedFilter = "Upcoming";
		}




		private void SetFilter(FilterChip chip)
		{
			if (chip.IsSelected) return;

			foreach (var f in Filters)
				f.IsSelected = false;

			chip.IsSelected = true;
			SelectedFilter = chip.Name;
		}

		partial void OnSelectedFilterChanged(string value)
		{
			_ = ApplyFilter();
		}
		private async Task ApplyFilter()
		{
			TripFilter filter = SelectedFilter switch
			{
				"Upcoming" => TripFilter.CurrentAndUpcoming,
				"Past" => TripFilter.Past,
				"All" => TripFilter.All,
				_ => TripFilter.CurrentAndUpcoming
			};

			await LoadTripsCoreAsync(filter);
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
				await LoadTripsCoreAsync();
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
				await ApplyFilter();
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

		private async Task LoadTripsCoreAsync(TripFilter filter = TripFilter.CurrentAndUpcoming)
		{
			Trips.Clear();

			var result = await _tripService.GetTrips(filter);

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
