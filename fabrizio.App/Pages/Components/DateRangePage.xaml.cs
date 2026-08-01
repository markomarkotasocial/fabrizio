using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using fabrizio.App.Services;
using fabrizio.App.ViewModels;

namespace fabrizio.App.Pages.Components
{
	public partial class DateRangePage : ContentPage
	{
		private readonly DateRangeViewModel _viewModel;

		public DateRangePage(DateRangeViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = _viewModel = viewModel;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();			
		}



	}
}
