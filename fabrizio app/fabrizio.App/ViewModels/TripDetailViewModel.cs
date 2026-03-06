using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using fabrizio.App.ViewModels;
using fabrizio.Shared.DTO;

namespace fabrizio.App.Services
{
	[QueryProperty(nameof(TripId), "tripId")]
	public partial class TripDetailViewModel : BaseViewModel
	{
		private readonly ITripService _tripService;


		[ObservableProperty] private Guid tripId;


		[ObservableProperty] string name;

		[ObservableProperty] DateTime? startDate;

		[ObservableProperty] DateTime? endDate;

		[ObservableProperty] string notes;
		[ObservableProperty] int status;

		[ObservableProperty] ObservableCollection<string> destinations;
		[ObservableProperty] ObservableCollection<AccommodationBookingDto> accommodations;
		[ObservableProperty] ObservableCollection<TravelBookingDto> travels;



		public TripDetailViewModel(ITripService tripService)
		{
			_tripService = tripService;
		}




		partial void OnTripIdChanged(Guid value)
		{
			if (value != Guid.Empty)
			{
				LoadTrip(value);
			}
		}




		public async Task LoadTrip(Guid id)
		{
			if (IsBusy)	return;

			try
			{
				IsBusy = true;

				var result = await _tripService.GetTrip(id);
				if (!result.IsSuccess || result.Value == null) return;

				var trip = result.Value;

				TripId = trip.Id;
				Name = trip.Name;
				Notes = trip.Notes;
				StartDate = trip.StartDate;
				EndDate = trip.EndDate;
				Status = trip.Status;

				Destinations = new ObservableCollection<string>(trip.Destinations.Select(x => x.Name));
				Accommodations = new ObservableCollection<AccommodationBookingDto>(trip.AccommodationBookings);
				Travels = new ObservableCollection<TravelBookingDto>(trip.TravelBookings);
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}
