using System;

using Microsoft.Extensions.DependencyInjection;

using fabrizio.App.Pages.Auth;

namespace fabrizio.App.Services
{
	/// <summary>
	/// Swaps the application root between the tabbed shell and the login page.
	/// The target page is resolved from the DI container (real constructors), on
	/// the main thread.
	/// </summary>
	public interface INavigationService
	{
		/// <summary>Make the tabbed <see cref="AppShell"/> the application root (startup / after login).</summary>
		void GoToApp();

		/// <summary>Make the login page the application root (logout / 401).</summary>
		void GoToLogin();
	}

	internal sealed class NavigationService : INavigationService
	{
		private readonly IServiceProvider _services;

		public NavigationService(IServiceProvider services) => _services = services;

		public void GoToApp() => SetRoot<AppShell>();

		public void GoToLogin() => SetRoot<LoginPage>();

		private void SetRoot<TPage>() where TPage : Page =>
			MainThread.BeginInvokeOnMainThread(() =>
			{
				if (Application.Current is { } app)
					app.MainPage = _services.GetRequiredService<TPage>();
			});
	}
}
