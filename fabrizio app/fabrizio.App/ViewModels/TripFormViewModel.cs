using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.ViewModels;
using fabrizio.Shared.DTO;
using System.Collections.ObjectModel;

namespace fabrizio.App.Services
{
	public partial class TripFormViewModel : BaseViewModel
	{
		private readonly ITripService _tripService;


		[ObservableProperty] private Guid tripId;

		[ObservableProperty] string name;
		[ObservableProperty] DateTime? startDate;
		[ObservableProperty] DateTime? endDate;
		[ObservableProperty] string notes;

		[ObservableProperty] ObservableCollection<string> destinations;
		[ObservableProperty] ObservableCollection<AccommodationBookingDto> accommodations;
		[ObservableProperty] ObservableCollection<TravelBookingDto> travels;


		public AsyncRelayCommand SaveCommand { get; }
		public AsyncRelayCommand CancelCommand { get; }


		public TripFormViewModel(ITripService tripService)
		{
			_tripService = tripService;

			SaveCommand = new AsyncRelayCommand(SaveChangesAsync);
			CancelCommand = new AsyncRelayCommand(CancelChanges);
		}


		public async Task SaveChangesAsync()
		{
		}

		public async Task CancelChanges()
		{
		}

		public async Task LoadTrip(Guid id)
		{
			
		}
	}
}
