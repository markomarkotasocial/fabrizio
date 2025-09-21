using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using fabrizio.DTO;

namespace fabrizio.App.Services
{
	public partial class TripsViewModel : ObservableObject
	{
		private readonly ITripService _tripService;

		public ObservableCollection<GETTrip> Trips { get; } = new();

		[ObservableProperty]
		private GETTrip selectedTrip;

		public AsyncRelayCommand LoadCommand { get; }
		public AsyncRelayCommand AddTripCommand { get; }
		public AsyncRelayCommand<GETTrip> OpenTripCommand { get; }

		public TripsViewModel(ITripService tripService)
		{
			_tripService = tripService;

			LoadCommand = new AsyncRelayCommand(LoadTripsAsync);
			AddTripCommand = new AsyncRelayCommand(OnAddTripAsync);
			OpenTripCommand = new AsyncRelayCommand<GETTrip>(OpenTripAsync);
		}

		private async Task LoadTripsAsync()
		{
			var list = await _tripService.GetTrips();
			Trips.Clear();
			foreach (var t in list)
				Trips.Add(t);
		}

		private Task OnAddTripAsync()
			=> Shell.Current.GoToAsync("AddPage"); // make sure route matches

		private async Task OpenTripAsync(GETTrip trip)
		{
			if (trip == null) return;
			await Shell.Current.GoToAsync($"TripDetailPage?tripId={trip.Id}");
		}

		partial void OnSelectedTripChanged(GETTrip value)
		{
			if (value == null) return;

			_ = OpenTripCommand.ExecuteAsync(value);
			selectedTrip = null; // clear highlight
		}
	}
}
