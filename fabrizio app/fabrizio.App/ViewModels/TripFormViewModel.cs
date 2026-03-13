using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.ViewModels;
using fabrizio.Shared.DTO;
using fabrizio.Shared.Contracts;
using System.Collections.ObjectModel;

namespace fabrizio.App.Services
{
	[QueryProperty(nameof(TripId), "tripId")]
	public partial class TripFormViewModel : BaseViewModel
	{
		private readonly ITripService _tripService;


		[ObservableProperty] private Guid? tripId;

		[ObservableProperty] string name;
		[ObservableProperty] DateTime? startDate;
		[ObservableProperty] DateTime? endDate;
		[ObservableProperty] string notes;

		[ObservableProperty] ObservableCollection<string> destinations;
		[ObservableProperty] ObservableCollection<AccommodationBookingDto> accommodations;
		[ObservableProperty] ObservableCollection<TravelBookingDto> travels;



		public bool IsNewTrip => TripId == null;
		public string Title => TripId == null ? "New Trip" : "Edit Trip";


		public TripFormViewModel(ITripService tripService)
		{
			_tripService = tripService;

		}



		partial void OnTripIdChanged(Guid? value)
		{
			OnPropertyChanged(nameof(Title));

			if (value.HasValue)
			{
				_ = LoadTrip(value.Value);
			}
			else
			{
				InitNewTrip();
			}
		}
		
		partial void OnStartDateChanged(DateTime? value)
		{
			if (value.HasValue && EndDate < value)
				EndDate = value.Value.AddDays(1);
		}






		[RelayCommand]
		public async Task Save()
		{
			if (IsBusy) return;
			IsBusy = true;

			try
			{
				Result result;

				if (TripId == null)
				{
					result = await _tripService.AddTrip(new CreateTripRequest
					{
						Name = Name,
						StartDate = StartDate,
						EndDate = EndDate,
						Notes = Notes ?? string.Empty
					});
				}
				else
				{
					result = await _tripService.UpdateTrip(new UpdateTripRequest
					{
						Id = (Guid)TripId,
						Name = Name,
						StartDate = StartDate,
						EndDate = EndDate,
						Notes = Notes ?? string.Empty
					});
				}

				if (!result.IsSuccess)
				{
					await Shell.Current.DisplayAlert("Error", result.Error?.Message ?? "Unknown error occurred.", "OK");
					return;
				}

				await Shell.Current.GoToAsync("..");
			}
			finally
			{
				IsBusy = false;
			}
		}


		[RelayCommand]
		public async Task Cancel()
		{
			await Shell.Current.GoToAsync("..");
		}










		public async Task LoadTrip(Guid id)
		{
			if (IsBusy) return;

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
				//Status = trip.Status;

				Destinations = new ObservableCollection<string>(trip.Destinations.Select(x => x.Name));
				Accommodations = new ObservableCollection<AccommodationBookingDto>(trip.AccommodationBookings);
				Travels = new ObservableCollection<TravelBookingDto>(trip.TravelBookings);
			}
			finally
			{
				IsBusy = false;
			}
		}

		private void InitNewTrip()
		{
			Name = string.Empty;
			StartDate = DateTime.Today;
			EndDate = DateTime.Today.AddDays(1);
			Notes = string.Empty;
			Destinations = new ObservableCollection<string>();
			Accommodations = new ObservableCollection<AccommodationBookingDto>();
			Travels = new ObservableCollection<TravelBookingDto>();
		}



		
		

		
	}
}
