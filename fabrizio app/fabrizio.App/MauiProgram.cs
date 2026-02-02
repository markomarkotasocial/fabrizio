using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using fabrizio.App.Services;
using fabrizio.App.Pages.Tabs;

namespace fabrizio.App
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

#if DEBUG
			builder.Logging.AddDebug();
#endif

			// 🟢 Services
			builder.Services.AddTransient<TokenHandler>();

			builder.Services.AddHttpClient<ITripService, TripService>(client =>
			{
				client.BaseAddress = new Uri("https://fabrizio-ftdpcwhsh5enhscn.westeurope-01.azurewebsites.net/");
				client.DefaultRequestHeaders.Add("Accept", "application/json");
			})
			.AddHttpMessageHandler<TokenHandler>();

			// 🟢 ViewModels
			builder.Services.AddTransient<TripsViewModel>();

			// 🟢 Pages
			builder.Services.AddTransient<TripsPage>();

			return builder.Build();
		}
	}
}
