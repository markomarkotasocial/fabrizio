---
name: maui-agent
role: frontend
scope: "fabrizio.App"
description: "Agent specialized in .NET MAUI UI and ViewModel work. Uses CommunityToolkit.Mvvm patterns, typed HttpClient and TokenHandler for API calls."
---

Agent rules
-----------
- Focus only on `fabrizio.App` and its DI registrations. Do not change backend projects without delegation to api-agent.
- Locate `BaseViewModel` (file `_BaseViewModel.cs`, class without prefix, namespace `fabrizio.App.ViewModels`) and `TokenHandler` before generating code. BaseViewModel provides `IsBusy`, `EmptyMessage`, `HasError` (no `Title` property). Reuse exact namespaces and base types.
- Follow CommunityToolkit.Mvvm conventions: use `[ObservableProperty]` for properties, `AsyncRelayCommand` for async actions, and minimal code-behind in Pages.
- Register ViewModels and Pages in `MauiProgram.cs` following the existing pattern: `AddTransient` for Pages and ViewModels, `AddSingleton` for app-wide state (e.g., `IAccountState`, `AppShell`), `AddHttpClient<TInterface, TImpl>(...).AddHttpMessageHandler<TokenHandler>()` for API services.
- API services go in `fabrizio.App/Services/` with interface and implementation in the same file (e.g., `Example.cs` contains both `IExampleService` and `ExampleService`). Exception: cross-cutting state contracts like `IAccountState` live in `Services/Abstractions/`.
- MAUI services consume API responses via `Result<T>` or `PagedResult<T>` (from `fabrizio.Shared.Contracts`); on error, deserialize to `ApiProblem` and rewrap in `Result<T>` for UI (see existing `ITripService`, `IProfileService`).

XAML and binding
----------------
- Pages should bind to ViewModels via DI-resolved instances. Do not set BindingContext in XAML; resolve Page from DI when navigating or when constructing AppShell.
- Keep UI code declarative in XAML. Move logic to ViewModel and services.

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
When a backend API contract is provided, implement client-side DTOs (share via `fabrizio.DTO/DTO/` if possible; assembly is `fabrizio.Shared`) and wire calls through typed HttpClient services. Provide sample usage in a ViewModel and a minimal Page.
