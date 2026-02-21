using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fabrizio.App.Pages.Auth;
using fabrizio.App.ViewModels;
using fabrizio.Shared.DTO;

namespace fabrizio.App.Services
{
	public partial class HomeViewModel : BaseViewModel
	{
		private readonly ITripService _tripService;
		private readonly IAuthService _authService;



		[ObservableProperty] private bool isRefreshing;

		[ObservableProperty] private TripDto? current;

		[ObservableProperty] private TripDto? next;



		public AsyncRelayCommand RefreshCommand { get; }
		public AsyncRelayCommand LoadCommand { get; }


		public bool ShowCurrentSplash => Current != null;
		public bool ShowNextFollower => Current != null && Next != null;
		public bool ShowNextSplash => Current == null && Next != null;
	



		public bool IsEmpty => !IsBusy && !IsRefreshing && Current == null && Next == null;
		public bool HasAnyTrip => Current != null || Next != null;


		private bool _isInitialized;

		public HomeViewModel(ITripService tripService, AuthService authService)
		{
			_tripService = tripService;
			_authService = authService;

			LoadCommand = new AsyncRelayCommand(LoadInitialAsync);
			RefreshCommand = new AsyncRelayCommand(RefreshAsync); //, () => !IsRefreshing);

			//this.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(IsRefreshing)) RefreshCommand.NotifyCanExecuteChanged(); };
		}



		partial void OnCurrentChanged(TripDto? value)
		{
			OnPropertyChanged(nameof(HasAnyTrip));
		}

		partial void OnNextChanged(TripDto? value)
		{
			OnPropertyChanged(nameof(HasAnyTrip));
		}



		public async Task LoadOnEnterAsync()
		{
			if (IsBusy) return;

			try
			{
				IsBusy = true;
				await LoadOverviewCoreAsync();
			}
			catch (UnauthorizedException)
			{
				await _authService.LogoutAsync();
			}
			finally
			{
				IsBusy = false;
				OnPropertyChanged(nameof(IsEmpty));
			}
		}

		private async Task LoadInitialAsync()
		{
			if (IsBusy) return;

			try
			{
				IsBusy = true;
				await LoadOverviewCoreAsync();
			}
			catch (UnauthorizedException)
			{
				await _authService.LogoutAsync();
			}
			finally
			{
				MainThread.BeginInvokeOnMainThread(() =>
				{
					IsBusy = false;
					OnPropertyChanged(nameof(IsEmpty));
				});
			}
		}

		public async Task RefreshAsync()
		{
			try
			{
				await LoadOverviewCoreAsync();
			}
			catch (UnauthorizedException)
			{
				await _authService.LogoutAsync();
			}
			finally
			{
				IsRefreshing = false;
				OnPropertyChanged(nameof(IsEmpty));

			}
		}

		public async Task LoadOverviewCoreAsync()
		{
			var result = await _tripService.GetTripsOverview();

			if (!result.IsSuccess)
			{
				Current = null;
				Next = null;

				OnPropertyChanged(nameof(ShowCurrentSplash));
				OnPropertyChanged(nameof(ShowNextFollower));
				OnPropertyChanged(nameof(ShowNextSplash));
				OnPropertyChanged(nameof(IsEmpty));

				return;
			}

			if (result.Value?.Current != null)
			{
				Current = result.Value.Current;

				OnPropertyChanged(nameof(ShowCurrentSplash));
				OnPropertyChanged(nameof(ShowNextFollower));
				OnPropertyChanged(nameof(ShowNextSplash));
				OnPropertyChanged(nameof(IsEmpty));
			}

			if (result.Value?.Next != null)
			{
				Next = result.Value.Next;

				OnPropertyChanged(nameof(ShowCurrentSplash));
				OnPropertyChanged(nameof(ShowNextFollower));
				OnPropertyChanged(nameof(ShowNextSplash));
				OnPropertyChanged(nameof(IsEmpty));
			}
		}






		//partial void OnCurrentChanged(GETTrip? value)
		//{
		//	OnPropertyChanged(nameof(HasCurrent));
		//	OnPropertyChanged(nameof(HasNext));
		//	OnPropertyChanged(nameof(IsEmpty));
		//}

		//partial void OnNextChanged(GETTrip? value)
		//{
		//	OnPropertyChanged(nameof(HasNext));
		//	OnPropertyChanged(nameof(IsEmpty));
		//}
	}


}
