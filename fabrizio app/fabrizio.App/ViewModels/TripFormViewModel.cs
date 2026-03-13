using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Pages.Components;
using fabrizio.App.ViewModels;
using fabrizio.Shared.Contracts;
using fabrizio.Shared.DTO;
using System.Collections.ObjectModel;

namespace fabrizio.App.Services
{
	[QueryProperty(nameof(TripId), "tripId")]
	[QueryProperty(nameof(SelectedStartDate), "selectedStartDate")]
	[QueryProperty(nameof(SelectedEndDate), "selectedEndDate")]
	public partial class TripFormViewModel : BaseViewModel
	{
		private readonly ITripService _tripService;


		[ObservableProperty] private Guid? tripId;

		[ObservableProperty] string name;

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(DateRangeDisplay))] DateTime? startDate;

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(DateRangeDisplay))] DateTime? endDate;

		[ObservableProperty] string notes;

		[ObservableProperty] ObservableCollection<string> destinations;
		[ObservableProperty] ObservableCollection<AccommodationBookingDto> accommodations;
		[ObservableProperty] ObservableCollection<TravelBookingDto> travels;



		public bool IsNewTrip => TripId == null;
		public string Title => TripId == null ? "New Trip" : "Edit Trip";
		public string DateRangeDisplay => StartDate.HasValue && EndDate.HasValue ? $"{StartDate:dd MMM yyyy} → {EndDate:dd MMM yyyy}" : "Select travel dates";


		public DateTime SelectedStartDate {	set => StartDate = value; }
		public DateTime SelectedEndDate	{ set => EndDate = value; }


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






		[RelayCommand]
		async Task SelectDates()
		{
			var query = new List<string>();

			if (StartDate.HasValue)
				query.Add($"startDate={StartDate.Value:O}");

			if (EndDate.HasValue)
				query.Add($"endDate={EndDate.Value:O}");

			var queryString = string.Join("&", query);

			var route = string.IsNullOrEmpty(queryString)
				? nameof(DateRangePage)
				: $"{nameof(DateRangePage)}?{queryString}";

			await Shell.Current.GoToAsync(route);
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
