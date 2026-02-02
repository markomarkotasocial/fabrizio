using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.DTO;

namespace fabrizio.App.Services
{
	public partial class AddTripViewModel : ObservableObject
	{
		private readonly ITripService _tripService;


		[ObservableProperty] private Guid tripId;




		public AsyncRelayCommand SaveCommand { get; }
		public AsyncRelayCommand CancelCommand { get; }


		public AddTripViewModel(ITripService tripService)
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
			// TODO: call _tripService.GetTrip(id) and map result to fields
		}
	}
}
