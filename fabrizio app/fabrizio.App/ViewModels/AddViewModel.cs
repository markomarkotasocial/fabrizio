using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Pages.Auth;
using fabrizio.App.ViewModels;
using fabrizio.DTO;

namespace fabrizio.App.Services
{
	public partial class AddViewModel : BaseViewModel
	{
		private readonly ITripService _tripService;



		public AsyncRelayCommand RefreshCommand { get; }


		public AddViewModel(ITripService tripService)
		{
			_tripService = tripService;
			//RefreshCommand = new AsyncRelayCommand(RefreshHomeAsync);
		}

		
		
		//private async Task RefreshHomeAsync()
		//{			

		//}

	}

}
