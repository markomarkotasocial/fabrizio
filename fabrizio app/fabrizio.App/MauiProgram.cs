using fabrizio.App.Pages.Auth;
using fabrizio.App.Pages.Flows;
using fabrizio.App.Pages.Tabs;
using fabrizio.App.Services;
using fabrizio.App.Services.Abstractions;
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

			builder.Services.AddSingleton<IAccountState, AccountState>();

			builder.Services.AddSingleton<IAuthService, AuthService>();
			builder.Services.AddHttpClient<ITripService, TripService>(client =>
			{
				client.BaseAddress = new Uri("https://fabrizio-ftdpcwhsh5enhscn.westeurope-01.azurewebsites.net/");
				client.DefaultRequestHeaders.Add("Accept", "application/json");
			}).AddHttpMessageHandler<TokenHandler>();

			builder.Services.AddHttpClient<IProfileService, ProfileService>(client =>
			{
				client.BaseAddress = new Uri("https://fabrizio-ftdpcwhsh5enhscn.westeurope-01.azurewebsites.net/");
				client.DefaultRequestHeaders.Add("Accept", "application/json");
			}).AddHttpMessageHandler<TokenHandler>();


			// 🟢 ViewModels
			builder.Services.AddTransient<LoginViewModel>();
			builder.Services.AddTransient<TripsViewModel>();
			builder.Services.AddTransient<HomeViewModel>();
			builder.Services.AddTransient<ProfileViewModel>();

			builder.Services.AddTransient<EditLanguageViewModel>();
			builder.Services.AddTransient<EditCurrencyViewModel>();

			// 🟢 Pages
			builder.Services.AddTransient<LoginPage>();
			builder.Services.AddTransient<TripsPage>();
			builder.Services.AddTransient<HomePage>();
			builder.Services.AddTransient<ProfilePage>();

			builder.Services.AddTransient<EditLanguagePage>();
			builder.Services.AddTransient<EditCurrencyPage>();

			return builder.Build();
		}
	}
}
