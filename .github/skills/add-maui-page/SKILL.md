Skill: add-maui-page
=====================

Purpose
-------
Steps to add a .NET MAUI Page + ViewModel and register them with DI and navigation in this repository.

Steps
-----
1. Create the XAML Page under `fabrizio.App/Pages/<Area>/<Name>Page.xaml` and code-behind `.xaml.cs`.
2. Create `NameViewModel` in `fabrizio.App/ViewModels`, inherit from `BaseViewModel` (located in `_BaseViewModel.cs`, namespace `fabrizio.App.ViewModels`).
   - Use CommunityToolkit.Mvvm attributes:
	 - `[ObservableProperty]` on backing fields
	 - `AsyncRelayCommand` for async actions
   - BaseViewModel provides: `IsBusy`, `EmptyMessage`, `HasError` (do NOT add `Title`).
3. Register Page and ViewModel in `MauiProgram.cs` using `AddTransient<NamePage>()` and `AddTransient<NameViewModel>()`. Follow existing registration patterns.
4. If the page consumes an API, add or use a typed HttpClient service (e.g., `IExampleService`) and inject it into the ViewModel.
5. Bind XAML to ViewModel properties and commands. Prefer no code-behind logic beyond `InitializeComponent` and small UI wiring.
6. Build the MAUI app: `dotnet workload install maui` (first time), then `dotnet build fabrizio.App/fabrizio.App.csproj` (CI does not build MAUI, so local build is the only automated check). Test navigation by resolving Page from DI and navigating via AppShell or navigation service.

Example ViewModel template
-------------------------
```csharp
public partial class NameViewModel : BaseViewModel
{
	[ObservableProperty] private string itemName;
	public AsyncRelayCommand LoadCommand { get; }

	public NameViewModel(IExampleService exampleService)
	{
		LoadCommand = new AsyncRelayCommand(LoadAsync);
	}

	private async Task LoadAsync() { /* ... */ }
}
```

Notes
-----
- Use `AddTransient` for Pages/ViewModels unless state must be preserved.
- BaseViewModel (in `_BaseViewModel.cs`) has `IsBusy`, `EmptyMessage`, `HasError`. Do not add `Title` property; these are the established patterns.
- This solution has no automated test project. Verify UI changes by running the app locally on your target platform (Android/iOS/Windows).
