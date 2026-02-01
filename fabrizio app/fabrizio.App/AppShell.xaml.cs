namespace fabrizio.App
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();

			Routing.RegisterRoute("AddTripPage", typeof(AddTripPage));
		}
	}
}
