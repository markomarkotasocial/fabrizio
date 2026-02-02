using fabrizio.App.Pages.Auth;
using fabrizio.App.Pages.Flows;

namespace fabrizio.App
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();

			// Flow / detail routes (NOT in TabBar)
			Routing.RegisterRoute("add-trip", typeof(AddTripPage));
			Routing.RegisterRoute("trip-detail", typeof(TripDetailPage));

			Routing.RegisterRoute("login", typeof(LoginPage));
			Routing.RegisterRoute("register", typeof(RegisterPage));
			Routing.RegisterRoute("forgot-password", typeof(ForgotPasswordPage));
		}
	}
}
