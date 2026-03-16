using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.ViewModels;
using fabrizio.Shared.DTO;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security;

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



		public string DateRangeText => StartDate.HasValue && EndDate.HasValue ? $"{StartDate:dd MMM} — {EndDate:dd MMM}" : string.Empty;
		public string SummaryText
		{
			get
			{
				var parts = new List<string>();
				if (StartDate.HasValue && EndDate.HasValue)
				{
					int days = (EndDate.Value - StartDate.Value).Days;
					parts.Add($"{days} days");
				}
				if (Destinations?.Count > 0)
				{
					parts.Add($"{Destinations.Count} destinations");
				}
				return string.Join(" • ", parts);
			}
		}



		public AsyncRelayCommand AddAccomodation { get; }
		public AsyncRelayCommand AddTravel { get; }


		public string Title => "Trip Details";


		public TripDetailViewModel(ITripService tripService)
		{
			_tripService = tripService;

			AddAccomodation = new AsyncRelayCommand(AddAccomodationAsync);
			AddTravel = new AsyncRelayCommand(AddTravelAsync);
		}


		partial void OnTripIdChanged(Guid value)
		{
			if (value != Guid.Empty)
			{
				Load(value);
			}
		}


		partial void OnStartDateChanged(DateTime? value)
		{
			OnPropertyChanged(nameof(DateRangeText));
			OnPropertyChanged(nameof(SummaryText));
		}

		partial void OnEndDateChanged(DateTime? value)
		{
			OnPropertyChanged(nameof(DateRangeText));
			OnPropertyChanged(nameof(SummaryText));
		}
		partial void OnDestinationsChanged(ObservableCollection<string> value)
		{
			if (value != null)
			{
				value.CollectionChanged += (s, e) =>
				{
					OnPropertyChanged(nameof(SummaryText));
				};
			}
			OnPropertyChanged(nameof(SummaryText));
		}






		[RelayCommand]
		private async Task AddAccomodationAsync()
		{
			var action = await Shell.Current.DisplayActionSheet(
				"Add accomodation",
				"Cancel",
				null,
				"Enter manually",
				"Scan document");

			if (action == "Enter manually")
			{
				// kasnije:
				// await Shell.Current.GoToAsync("accommodationbooking-form");
			}

			if (action == "Scan document")
			{
				// kasnije:
				// await Shell.Current.GoToAsync("accommodationbooking-ai");
			}
		}

		[RelayCommand]
		private async Task AddTravelAsync()
		{
			var action = await Shell.Current.DisplayActionSheet(
				"Add transport",
				"Cancel",
				null,
				"Enter manually",
				"Scan ticket");

			if (action == "Enter manually")
			{
				// kasnije:
				// await Shell.Current.GoToAsync("travelbooking-form");
			}

			if (action == "Scan ticket")
			{
				// kasnije:
				// await Shell.Current.GoToAsync("travelbooking-ai");
			}
		}


		[RelayCommand]
		public async Task Cancel()
		{
			await Shell.Current.GoToAsync("..");
		}



		private bool _isOpeningDetail; // double tap protection

		[RelayCommand]
		public async Task Edit()
		{
			if (_isOpeningDetail) return;
			try
			{
				_isOpeningDetail = true;

				await Shell.Current.GoToAsync("trip-form", new Dictionary<string, object>
				{
					["tripId"] = TripId
				});

			}
			finally
			{
				_isOpeningDetail = false;
			}
		}

		[RelayCommand]
		private async Task DeleteTrip()
		{
			bool confirm = await Shell.Current.DisplayAlert(
				"Delete trip",
				"Are you sure you want to delete this trip?",
				"Delete",
				"Cancel");

			if (!confirm) return;

			//var result = await _tripService.DeleteTrip();

			//if (!result.IsSuccess)
			//{
			//	await Shell.Current.DisplayAlert("Error", result.Error?.Message ?? "Error deleting trip", "OK");
			//	return;
			//}

			await Shell.Current.GoToAsync("..");
		}



		[RelayCommand]
		public async Task Load(Guid id)
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
