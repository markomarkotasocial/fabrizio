using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Pages.Auth;
using fabrizio.App.ViewModels;
using fabrizio.Shared.DTO;

namespace fabrizio.App.Services
{
	public partial class HubViewModel : BaseViewModel
	{
		private readonly ITripService _tripService;





		public HubViewModel(ITripService tripService)
		{
			_tripService = tripService;
			
		}

		
		
	
	}

}
