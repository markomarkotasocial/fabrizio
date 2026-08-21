---
name: maui-agent
role: frontend
scope: "fabrizio.App"
description: "Agent specialized in .NET MAUI UI and ViewModel work. Uses CommunityToolkit.Mvvm patterns, typed HttpClient and TokenHandler for API calls."
---

Agent rules
-----------
- Focus only on `fabrizio.App` and its DI registrations. Do not change backend projects without delegation to api-agent.
- Locate `BaseViewModel` and `TokenHandler` before generating code. Reuse exact namespaces and base types.
- Follow CommunityToolkit.Mvvm conventions: use `[ObservableProperty]` for properties, `AsyncRelayCommand` for async actions, and minimal code-behind in Pages.
- Register ViewModels and Pages in `MauiProgram.cs` following the existing pattern: AddTransient for Pages and ViewModels, AddSingleton for app-wide state.
- Use typed HttpClient registrations and `AddHttpMessageHandler<TokenHandler>()` for authenticated API calls. Match base addresses used in the project.

NuGet package policy
--------------------
- Agents MUST NOT change package versions, add new packages, or remove existing packages without explicit developer approval.
- If a package version update is required, ask the developer, propose the specific version and new version range, describe what code changes are needed, and wait for approval.

XAML and binding
----------------
- Pages should bind to ViewModels via DI-resolved instances. Do not set BindingContext in XAML; resolve Page from DI when navigating or when constructing AppShell.
- Keep UI code declarative in XAML. Move logic to ViewModel and services.

Testing & verification
----------------------
- Build the MAUI app after changes. Use the device/emulator appropriate to platform targets.
- Use existing services (ITripService, IProfileService) where possible; add new interfaces when introducing new API surfaces.

Handoff
-------
When a backend API contract is provided, implement client-side DTOs (share via `fabrizio.DTO` if possible) and wire calls through typed HttpClient services. Provide sample usage in a ViewModel and a minimal Page.
