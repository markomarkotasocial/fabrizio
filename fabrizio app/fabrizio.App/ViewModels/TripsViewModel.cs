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


		[ObservableProperty] private bool isRefreshing;


		public AsyncRelayCommand RefreshCommand { get; }

		public bool IsEmpty => !IsBusy && !IsRefreshing && Trips.Count == 0;

		private bool _isInitialized;
		private bool _isInitializing = true;
		private bool _isLoadingTrips;


		public TripsViewModel(ITripService tripService, IAuthService authService)
		{
			_tripService = tripService;
			_authService = authService;

			RefreshCommand = new AsyncRelayCommand(RefreshAsync, () => !IsRefreshing);

			// if something happen to Trips collection (add, delete, clear) => recalculate IsEmpty
			Trips.CollectionChanged += (_, __) => {	OnPropertyChanged(nameof(IsEmpty));	};
			this.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(IsRefreshing))	RefreshCommand.NotifyCanExecuteChanged(); };

			_isInitializing = false;
		}




		[RelayCommand]
		private void SetFilter(FilterChip chip)
		{
			if (chip.IsSelected) return;
			foreach (var f in Filters) f.IsSelected = f == chip;
			_ = ApplyFilter();
		}


		[RelayCommand]
		private Task AddTrip()
		{
			return Shell.Current.GoToAsync("trip-form");
		}
				
		private bool _isOpeningDetail; // double tap protection

		[RelayCommand]
		private async Task OpenTrip(TripListItemDto trip)
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

		[RelayCommand]
		public async Task Load()
		{
			if (IsBusy) return;
			try
			{
				IsBusy = true;
				await ApplyFilter();

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

		[RelayCommand]
		private async Task DeleteTrip(TripListItemDto trip)
		{
			await _tripService.DeleteTrip(trip.Id);
		}









		private async Task ApplyFilter()
		{
			var chip = Filters.FirstOrDefault(x => x.IsSelected);

			TripFilter filter = chip?.Name switch
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
			await Load();
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
			if (_isLoadingTrips) return;
			_isLoadingTrips = true;

			try
			{
				Trips.Clear();
				var result = await _tripService.GetTrips(filter);
				if (!result.IsSuccess) return;
				foreach (var t in result.Value!) Trips.Add(t);
			}
			finally
			{
				_isLoadingTrips = false;
			}
		}






	




	}
}
