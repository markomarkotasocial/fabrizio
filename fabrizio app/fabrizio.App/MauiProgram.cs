using fabrizio.App.Pages.Auth;
using fabrizio.App.Pages.Tabs;
using fabrizio.App.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
			builder.Services.AddSingleton<AppShell>();
			builder.Services.AddSingleton<AuthService>();
			builder.Services.AddTransient<TokenHandler>();

			builder.Services.AddSingleton<IAuthService, AuthService>();

			builder.Services.AddHttpClient<ITripService, TripService>(client =>
			{
				client.BaseAddress = new Uri("https://fabrizio-ftdpcwhsh5enhscn.westeurope-01.azurewebsites.net/");
				client.DefaultRequestHeaders.Add("Accept", "application/json");
			})
			.AddHttpMessageHandler<TokenHandler>();


			// 🟢 ViewModels
			builder.Services.AddTransient<LoginViewModel>();
			builder.Services.AddTransient<TripsViewModel>();

			// 🟢 Pages
			builder.Services.AddTransient<LoginPage>();
			builder.Services.AddTransient<TripsPage>();

			return builder.Build();
		}
	}
}
