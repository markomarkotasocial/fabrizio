using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Pages.Auth;
using fabrizio.Shared.DTO;

namespace fabrizio.App.Services
{
	public partial class DiscoverViewModel : ObservableObject
	{
		private readonly ITripService _tripService;



		public AsyncRelayCommand RefreshCommand { get; }


		public DiscoverViewModel(ITripService tripService)
		{
			_tripService = tripService;
			//RefreshCommand = new AsyncRelayCommand(RefreshHomeAsync);
		}

		
		
		//private async Task RefreshHomeAsync()
		//{			

		//}

	}

}
