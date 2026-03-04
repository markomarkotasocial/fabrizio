using fabrizio.App.Pages.Auth;
using fabrizio.App.Pages.Flows;
using fabrizio.App.Pages.Tabs;

namespace fabrizio.App
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();

			// Flow / detail routes (NOT in TabBar)
			Routing.RegisterRoute(nameof(EditLanguagePage), typeof(EditLanguagePage));
			Routing.RegisterRoute(nameof(EditCurrencyPage), typeof(EditCurrencyPage));
			Routing.RegisterRoute("add-trip", typeof(AddTripPage));
			Routing.RegisterRoute("trip-detail", typeof(TripDetailPage));

			Routing.RegisterRoute("login", typeof(LoginPage));
			Routing.RegisterRoute("register", typeof(RegisterPage));
			Routing.RegisterRoute("forgot-password", typeof(ForgotPasswordPage));
		}
	}
}
