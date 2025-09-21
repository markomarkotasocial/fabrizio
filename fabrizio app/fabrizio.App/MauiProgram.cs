using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using fabrizio.App.Services; 

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
			builder.Services.AddSingleton<TripsViewModel>();

			// 🟢 Pages
			builder.Services.AddSingleton<TripsPage>();

			return builder.Build();
		}
	}
}
