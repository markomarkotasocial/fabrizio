Skill: add-maui-page
=====================

Purpose
-------
Steps to add a .NET MAUI Page + ViewModel and register them with DI and navigation in this repository.

Steps
-----
1. Create the XAML Page under `fabrizio.App/Pages/<Area>/<Name>Page.xaml` and code-behind `.xaml.cs`.
2. Create `NameViewModel` in `fabrizio.App/ViewModels`, inherit from `BaseViewModel`.
   - Use CommunityToolkit.Mvvm attributes:
	 - [ObservableProperty] on backing fields
	 - AsyncRelayCommand for async actions
3. Register Page and ViewModel in `MauiProgram.cs` using AddTransient<NamePage>() and AddTransient<NameViewModel>(). Follow existing registration patterns.
4. If the page consumes an API, add or use a typed HttpClient service (IExampleService) and inject it into the ViewModel.
5. Bind XAML to ViewModel properties and commands. Prefer no code-behind logic beyond InitializeComponent and small UI wiring.
6. Test page navigation by resolving Page from DI and navigating via AppShell or navigation service used in the app.

Example ViewModel template
-------------------------
public partial class NameViewModel : BaseViewModel
{
	[ObservableProperty] private string title;
	public AsyncRelayCommand LoadCommand { get; }

	public NameViewModel(IExampleService exampleService)
	{
		LoadCommand = new AsyncRelayCommand(LoadAsync);
	}

	private async Task LoadAsync() { /* ... */ }
}

Notes
-----
- Use AddTransient for Pages/ViewModels unless state must be preserved.
- Locate and reuse `BaseViewModel` implementation to keep consistent IsBusy/Title patterns.
