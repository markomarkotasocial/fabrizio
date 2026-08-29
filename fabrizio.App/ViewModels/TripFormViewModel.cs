using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using fabrizio.App.Pages.Components;
using fabrizio.App.Services;
using fabrizio.Shared.Contracts;
using fabrizio.Shared.DTO;
using System.Collections.ObjectModel;

namespace fabrizio.App.ViewModels
{
	public partial class DestinationItemViewModel : ObservableObject
	{
		[ObservableProperty] private Guid? id;
		[ObservableProperty] private string name = string.Empty;
		[ObservableProperty] private int order;
		[ObservableProperty] private bool isNew;
	}




	[QueryProperty(nameof(TripId), "tripId")]
	[QueryProperty(nameof(SelectedStartDate), "selectedStartDate")]
	[QueryProperty(nameof(SelectedEndDate), "selectedEndDate")]
	public partial class TripFormViewModel : BaseViewModel
	{
		private readonly ITripService _tripService;


		[ObservableProperty] private Guid tripId;

		[ObservableProperty] string name;

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(DateRangeDisplay))] DateTime? startDate;

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(DateRangeDisplay))] DateTime? endDate;

		[ObservableProperty] string notes;

		[ObservableProperty] ObservableCollection<DestinationItemViewModel> destinations;
		[ObservableProperty] ObservableCollection<AccommodationBookingDto> accommodations;
		[ObservableProperty] ObservableCollection<TravelBookingDto> travels;



		public bool IsNewTrip => TripId == Guid.Empty;
		public bool ShowDestinations => !IsNewTrip;
		public string Title => TripId == Guid.Empty ? "New Trip" : "Edit Trip";
		public string DateRangeDisplay => StartDate.HasValue && EndDate.HasValue ? $"{StartDate:dd MMM yyyy} → {EndDate:dd MMM yyyy}" : "Select travel dates";


		public DateTime SelectedStartDate {	set => StartDate = value; }
		public DateTime SelectedEndDate	{ set => EndDate = value; }


		public TripFormViewModel(ITripService tripService)
		{
			_tripService = tripService;

			// Navigating in with tripId == Guid.Empty does not change the property
			// (it is already the default), so OnTripIdChanged never fires for a new
			// trip. Seed the "new trip" state here; OnTripIdChanged still loads an
			// existing trip when a real id arrives.
			InitNewTrip();
		}



		partial void OnTripIdChanged(Guid value)
		{
			OnPropertyChanged(nameof(Title));
			OnPropertyChanged(nameof(IsNewTrip));
			OnPropertyChanged(nameof(ShowDestinations));

			if (value != Guid.Empty)
				_ = LoadTrip(value);
			else
				InitNewTrip();
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

				if (TripId == Guid.Empty)
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
						Id = TripId,
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

				// Commit any destination rows that were typed but never unfocused.
				var pending = (Destinations ?? new ObservableCollection<DestinationItemViewModel>())
					.Where(d => d.IsNew && !string.IsNullOrWhiteSpace(d.Name)).ToList();
				foreach (var item in pending)
					await CreateDestinationAsync(item);

				WeakReferenceMessenger.Default.Send(new TripsChangedMessage());
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


		[RelayCommand]
		public async Task AddDestination()
		{
			Destinations.Add(new DestinationItemViewModel
			{
				Name = string.Empty,
				IsNew = true
			});
		}

		[RelayCommand]
		public async Task DeleteDestination(DestinationItemViewModel item)
		{
			if (item == null) return;

			// Handle unsaved (new) items locally
			if (item.IsNew || item.Id is not Guid id)
			{
				Destinations.Remove(item);
				return;
			}

			bool confirm = await Shell.Current.DisplayAlert("Delete destination", "Are you sure you want to delete this destination?", "Delete", "Cancel");
			if (!confirm) return;

			var result = await _tripService.DeleteDestination(TripId, id);

			if (!result.IsSuccess)
			{
				await Shell.Current.DisplayAlert("Error", result.Error?.Message ?? "Error deleting destination", "OK");
				return;
			}

			Destinations.Remove(item);
		}






		public async Task CreateDestinationAsync(DestinationItemViewModel item)
		{
			if (item == null || !item.IsNew || string.IsNullOrWhiteSpace(item.Name)) return;

			var result = await _tripService.AddDestination(TripId, new CreateDestinationRequest { Name = item.Name.Trim() });
			if (!result.IsSuccess)
			{
				await Shell.Current.DisplayAlert("Error", result.Error?.Message ?? "Could not save destination.", "OK");
				return;
			}

			item.Id = result.Value!.Id;
			item.Order = result.Value.Order;
			item.IsNew = false;
		}

		public async Task UpdateDestinationAsync(DestinationItemViewModel item)
		{
			if (item == null || item.IsNew || item.Id is not Guid id || string.IsNullOrWhiteSpace(item.Name)) return;

			var result = await _tripService.UpdateDestination(TripId, new UpdateDestinationRequest { Id = id, Name = item.Name.Trim() });
			if (!result.IsSuccess)
				await Shell.Current.DisplayAlert("Error", result.Error?.Message ?? "Could not update destination.", "OK");
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

				Name = trip.Name;
				Notes = trip.Notes;
				StartDate = trip.StartDate;
				EndDate = trip.EndDate;
				//Status = trip.Status;

				Destinations = new ObservableCollection<DestinationItemViewModel>(trip.Destinations.Select(x => new DestinationItemViewModel { Id = x.Id, IsNew = false, Name = x.Name, Order = x.Order }).OrderByDescending(x => x.Order).ToList());
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
			Destinations = new ObservableCollection<DestinationItemViewModel>();
			Accommodations = new ObservableCollection<AccommodationBookingDto>();
			Travels = new ObservableCollection<TravelBookingDto>();
		}



		
	}
}
