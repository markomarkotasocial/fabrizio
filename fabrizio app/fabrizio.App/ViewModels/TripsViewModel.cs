using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using fabrizio.DTO;
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


		public TripsViewModel(ITripService tripService, AuthService authService)
		{
			_tripService = tripService;
			_authService = authService;

			LoadCommand = new AsyncRelayCommand(LoadTripsAsync);
			AddTripCommand = new AsyncRelayCommand(OnAddTripAsync);
			DeleteTripCommand = new AsyncRelayCommand<GETTrip>(DeleteTripAsync);
			OpenTripCommand = new AsyncRelayCommand<GETTrip>(OpenTripAsync);
		}



		private bool _isLoading = false;
		private async Task LoadTripsAsync()
		{
			if (_isLoading) return;
			_isLoading = true;

			try
			{
				IsRefreshing = true;
				Trips.Clear();

				var list = await _tripService.GetTrips();
				foreach (var t in list)
					Trips.Add(t);
			}
			catch (UnauthorizedException)
			{
				await _authService.LogoutAsync();
			}
			finally
			{
				IsRefreshing = false;
				_isLoading = false;
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
