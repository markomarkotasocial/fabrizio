using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using fabrizio.Shared.DTO;
using fabrizio.App.ViewModels;

namespace fabrizio.App.Services
{
	public partial class TripsViewModel : BaseViewModel
	{
		private readonly ITripService _tripService;
		private readonly IAuthService _authService;


		public ObservableCollection<TripListItemDto> Trips { get; } = new();

		
		[ObservableProperty] private TripListItemDto selectedTrip;
		[ObservableProperty] private bool isRefreshing;



		public AsyncRelayCommand RefreshCommand { get; }
		public AsyncRelayCommand LoadCommand { get; }

		public AsyncRelayCommand AddTripCommand { get; }
		public AsyncRelayCommand<TripListItemDto> DeleteTripCommand { get; }
		public AsyncRelayCommand<TripListItemDto> OpenTripCommand { get; }


		public bool IsEmpty => !IsBusy && !IsRefreshing && Trips.Count == 0;

		private bool _isInitialized;


		public TripsViewModel(ITripService tripService, AuthService authService)
		{
			_tripService = tripService;
			_authService = authService;

			LoadCommand = new AsyncRelayCommand(LoadInitialAsync);
			RefreshCommand = new AsyncRelayCommand(RefreshAsync, () => !IsRefreshing);

			AddTripCommand = new AsyncRelayCommand(OnAddTripAsync);
			DeleteTripCommand = new AsyncRelayCommand<TripListItemDto>(DeleteTripAsync);
			OpenTripCommand = new AsyncRelayCommand<TripListItemDto>(OpenTripAsync);

			// if something happen to Trips collection (add, delete, clear) => recalculate IsEmpty
			Trips.CollectionChanged += (_, __) => {	OnPropertyChanged(nameof(IsEmpty));	};

			this.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(IsRefreshing))	RefreshCommand.NotifyCanExecuteChanged(); };
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
				await LoadTripsCoreAsync();
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

		private async Task LoadTripsCoreAsync()
		{
			Trips.Clear();

			var result = await _tripService.GetTrips();

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
			return Shell.Current.GoToAsync("add-trip");
		}

		private Task OpenTripAsync(TripListItemDto trip)
		{
			if (trip == null) return Task.CompletedTask;
			return Shell.Current.GoToAsync($"trip-detail?tripId={trip.Id}");
		}

		private async Task DeleteTripAsync(TripListItemDto trip)
		{
			await _tripService.DeleteTrip(trip.Id);
		}

	}
}
