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


		public ObservableCollection<GETTrip> Trips { get; } = new();

		
		[ObservableProperty] private GETTrip selectedTrip;
		[ObservableProperty] private bool isRefreshing;



		public AsyncRelayCommand RefreshCommand { get; }
		public AsyncRelayCommand LoadCommand { get; }

		public AsyncRelayCommand AddTripCommand { get; }
		public AsyncRelayCommand<GETTrip> DeleteTripCommand { get; }
		public AsyncRelayCommand<GETTrip> OpenTripCommand { get; }


		public bool IsEmpty => !IsBusy && !IsRefreshing && Trips.Count == 0;


		public TripsViewModel(ITripService tripService, AuthService authService)
		{
			_tripService = tripService;
			_authService = authService;

			LoadCommand = new AsyncRelayCommand(LoadInitialAsync);
			RefreshCommand = new AsyncRelayCommand(RefreshAsync);

			AddTripCommand = new AsyncRelayCommand(OnAddTripAsync);
			DeleteTripCommand = new AsyncRelayCommand<GETTrip>(DeleteTripAsync);
			OpenTripCommand = new AsyncRelayCommand<GETTrip>(OpenTripAsync);

			// if something happen to Trips collection (add, delete, clear) => recalculate IsEmpty
			Trips.CollectionChanged += (_, __) => {	OnPropertyChanged(nameof(IsEmpty));	};
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
				IsBusy = false;
				OnPropertyChanged(nameof(IsEmpty));
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
				// TODO: UI handling (toast, dialog, log...)
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

		private Task OpenTripAsync(GETTrip trip)
		{
			if (trip == null) return Task.CompletedTask;
			return Shell.Current.GoToAsync($"trip-detail?tripId={trip.Id}");
		}

		private async Task DeleteTripAsync(GETTrip trip)
		{
			await _tripService.DeleteTrip(trip.Id);
		}

	}
}
