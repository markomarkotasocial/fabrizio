using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.ViewModels;
using fabrizio.Shared.DTO;

namespace fabrizio.App.Services
{
	public partial class EditLanguageViewModel : BaseViewModel
	{
		private readonly ITripService _tripService;


		[ObservableProperty] private Guid tripId;




		public AsyncRelayCommand SaveCommand { get; }
		public AsyncRelayCommand CancelCommand { get; }


		public EditLanguageViewModel(ITripService tripService)
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

	}
}
