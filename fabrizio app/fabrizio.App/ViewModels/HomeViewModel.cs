using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Pages.Auth;
using fabrizio.DTO;

namespace fabrizio.App.Services
{
	public partial class HomeViewModel : ObservableObject
	{
		private readonly ITripService _tripService;




		public AsyncRelayCommand RefreshCommand { get; }


		public HomeViewModel(ITripService tripService)
		{
			_tripService = tripService;
			RefreshCommand = new AsyncRelayCommand(RefreshHomeAsync);
		}

		
		
		private async Task RefreshHomeAsync()
		{			


		}

	}

}
