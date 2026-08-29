using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Pages.Auth;
using fabrizio.App.Services;
using fabrizio.Shared.DTO;

namespace fabrizio.App.ViewModels
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
