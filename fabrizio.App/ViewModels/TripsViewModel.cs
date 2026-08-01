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

		[ObservableProperty] private bool isSelected;
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
		public AsyncRelayCommand LoadMoreCommand { get; }

		public bool IsEmpty => !IsBusy && !IsRefreshing && Trips.Count == 0;

		private bool _isInitialized;
		private bool _isInitializing = true;
		private bool _isLoadingTrips;


		private int _skip = 0;
		private const int PageSize = 12;
		private bool _hasMoreItems = true;
		private bool _isLoadingMore;


		public TripsViewModel(ITripService tripService, IAuthService authService)
		{
			_tripService = tripService;
			_authService = authService;

			RefreshCommand = new AsyncRelayCommand(RefreshAsync);
			LoadMoreCommand = new AsyncRelayCommand(LoadMoreAsync, () => !_isLoadingMore && _hasMoreItems);

			// if something happen to Trips collection (add, delete, clear) => recalculate IsEmpty
			Trips.CollectionChanged += (_, __) => {	OnPropertyChanged(nameof(IsEmpty));	};
			this.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(IsRefreshing))	RefreshCommand.NotifyCanExecuteChanged(); };

			_isInitializing = false;
		}



		[RelayCommand]
		private Task AddTrip()
		{
			return Shell.Current.GoToAsync("trip-form", new Dictionary<string, object>
			{
				["tripId"] = Guid.Empty
			});
		}


		[RelayCommand]
		private void SetFilter(FilterChip chip)
		{
			if (chip.IsSelected) return;
			foreach (var f in Filters) f.IsSelected = f == chip;
			_ = ApplyFilter();
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
			}
		}

		private async Task LoadMoreAsync()
		{
			// Prevent duplicate loads during infinite scrolling, but allow reload when starting from the first page (skip == 0),
			// e.g. during pull-to-refresh or initial load. This acts as a safety net beyond the command's CanExecute logic.
			if ((_isLoadingMore && _skip != 0) || !_hasMoreItems) return;

			try
			{
				_isLoadingMore = true;

				var chip = Filters.FirstOrDefault(x => x.IsSelected);

				TripFilter filter = chip?.Name switch
				{
					"Upcoming" => TripFilter.CurrentAndUpcoming,
					"Past" => TripFilter.Past,
					"All" => TripFilter.All,
					_ => TripFilter.CurrentAndUpcoming
				};

				var result = await _tripService.GetTrips(filter, _skip, PageSize);
				if (!result.IsSuccess || result.Value == null) return;

				var items = result.Value.ToList(); // !

				foreach (var t in items)
				{
					// Prevent duplicates in case of backend paging inconsistencies
					// or data changes between requests (e.g. new items inserted).
					if (!Trips.Any(x => x.Id == t.Id)) 
						Trips.Add(t);
				}

				_skip += items.Count;

				if (items.Count < PageSize)
					_hasMoreItems = false;
			}
			finally
			{
				_isLoadingMore = false;

				// Re-evaluate CanExecute to update command (LoadMore) availability in UI
				LoadMoreCommand.NotifyCanExecuteChanged();
			}
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

			await ResetAndLoadAsync(filter);
		}

		private async Task ResetAndLoadAsync(TripFilter filter)
		{
			_skip = 0;
			_hasMoreItems = true;
			Trips.Clear();
			await LoadMoreAsync();
		}

		public async Task EnsureLoadedAsync()
		{
			if (_isInitialized)	return;
			_isInitialized = true;
			await Load();
		}




	}
}
