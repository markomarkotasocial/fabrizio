---
name: maui-agent
role: frontend
scope: "fabrizio.App"
description: "Agent specialized in .NET MAUI UI and ViewModel work. Uses CommunityToolkit.Mvvm patterns, typed HttpClient and TokenHandler for API calls."
---

Read [.github/docs/CONVENTIONS.md](../docs/CONVENTIONS.md) first — the MAUI helper table is the contract for this agent.

Agent rules
-----------
- Focus only on `fabrizio.App` and its DI registrations. Do not change backend projects without delegation to api-agent.
- Locate `BaseViewModel` (`_BaseViewModel.cs`, namespace `fabrizio.App.ViewModels`: `IsBusy`, `EmptyMessage`, `HasError` — no `Title`) before generating code. ViewModels live in `namespace fabrizio.App.ViewModels`, files under `ViewModels/`.
- Follow CommunityToolkit.Mvvm conventions: `[ObservableProperty]`, `AsyncRelayCommand` / `[RelayCommand]`, minimal code-behind.
- **API service method = one line** calling a `HttpResultExtensions` helper (`http.GetResultAsync<T>(url)`, `PostResultAsync<T>(url, body)`, `PostResultAsync(url, body)`, `PutResultAsync…`, `DeleteResultAsync(url)`). Each returns `Result` / `Result<T>`, maps a non-2xx `ProblemDetails` to a `BusinessError` and a transport failure to `network_error`. Do NOT hand-roll `try { ReadFromJsonAsync } catch`. Keep the `I{X}Service` interface stable so ViewModels don't change.
- Interface + implementation in one file under `fabrizio.App/Services/` (e.g. `Trip.cs`). `Services/Abstractions/` is only for cross-cutting state contracts (`IAccountState`).
- **Root navigation is `INavigationService`** (`GoToApp()` / `GoToLogin()`), injected. Never `Application.Current.MainPage = new AppShell()/new LoginPage()`.
- **`fabrizio.Shared` DTOs stay pure data.** Computed/display members go on a client wrapper (`TripCardModel`) or the ViewModel.
- After a create/edit/delete that another screen's list shows, `WeakReferenceMessenger.Default.Send(new {X}ChangedMessage())`; the list VM `Register`s for it and reloads on next appearance (see `TripsChangedMessage`).
- A login-style call that must not carry a token / must not trigger logout-on-401 uses a standalone `HttpClient` (see `AuthService`), not the typed client + `TokenHandler`.
- Register in `MauiProgram.cs`: `AddTransient` for Pages, ViewModels, `AppShell`; `AddSingleton` for `IAccountState`, `AuthService` (+ `AddSingleton<IAuthService>(sp => sp.GetRequiredService<AuthService>())`), `INavigationService`; `AddHttpClient<TInterface, TImpl>(...).AddHttpMessageHandler<TokenHandler>()` for API services.

XAML and binding
----------------
- Each Page ctor takes its ViewModel via DI and sets `BindingContext` in the code-behind: `public XPage(XViewModel vm) { InitializeComponent(); BindingContext = vm; }`.
- Keep UI declarative in XAML; logic in ViewModel / services.

NuGet package policy
--------------------
- Agents MUST NOT change package versions, add new packages, or remove existing packages without explicit developer approval.
- If a package version update is required, ask the developer, propose the specific version and new version range, describe what code changes are needed, and wait for approval.

Verification
------------
- Build the MAUI app after changes. `dotnet workload install maui` (first time only), then `dotnet build fabrizio.App/fabrizio.App.csproj`.
- CI does not build MAUI; local build is the only automated check. Test on your target platform (Android/iOS/Windows).
- Use existing services (ITripService, IProfileService) where possible; add new interfaces when introducing new API surfaces.
- This solution has no automated test project. Verify UI changes by running the app locally.

Handoff
-------
When a backend API contract is provided: reuse the shared DTOs from `fabrizio.Shared` (add client-only ones only if the shape genuinely differs); add/extend an `I{X}Service` whose methods each delegate to a `HttpResultExtensions` call; wire it into a ViewModel and a minimal Page. Do not duplicate DTOs that already exist server-side.
