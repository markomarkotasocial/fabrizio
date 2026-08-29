Skill: add-maui-page
=====================

Purpose
-------
Add a .NET MAUI Page + ViewModel with DI and navigation. Assumes [.github/docs/CONVENTIONS.md](../../docs/CONVENTIONS.md).

Steps
-----
1. Create `fabrizio.App/Pages/<Area>/<Name>Page.xaml` + code-behind. The code-behind ctor injects the ViewModel and sets `BindingContext`:
   ```csharp
   public NamePage(NameViewModel vm) { InitializeComponent(); BindingContext = vm; }
   ```
2. Create `NameViewModel` in `fabrizio.App/ViewModels/` (namespace `fabrizio.App.ViewModels`), inherit `BaseViewModel` (`_BaseViewModel.cs`: `IsBusy`, `EmptyMessage`, `HasError` — no `Title`). Use `[ObservableProperty]` and `[RelayCommand]` / `AsyncRelayCommand`.
3. Register in `MauiProgram.cs`: `AddTransient<NamePage>()` and `AddTransient<NameViewModel>()`. Register a flow route in `AppShell.xaml.cs` (`Routing.RegisterRoute(...)`) if it is not a tab.
4. If the page calls the API, inject an `I{X}Service` whose methods delegate to `HttpResultExtensions` (see `add-authenticated-http-client`). If the page shows computed values over a DTO list, wrap items in a client model (like `TripCardModel`) rather than putting logic on the DTO.
5. If the page creates/edits/deletes data that another screen's list shows, `WeakReferenceMessenger.Default.Send(new {X}ChangedMessage())` on success before navigating away.
6. Bind XAML to ViewModel members. Code-behind stays at `InitializeComponent` + minimal UI wiring.
7. Build: `dotnet build fabrizio.App/fabrizio.App.csproj` (needs the `maui` workload; CI does not build MAUI). Test navigation on a device/emulator.

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
