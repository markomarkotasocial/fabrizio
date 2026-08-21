Repository Copilot Instructions
=============================

Purpose
-------
Rules and best practices for automated code agents working in this repository.

Global rules
------------
- Scope: only modify files in this repository. For cross-project changes, update ProjectReference and adjust DI registrations.
- Platform: this solution targets .NET 8. Use .NET 8 features and SDK-style projects.
- UI: this repository uses .NET MAUI (CommunityToolkit.Maui + CommunityToolkit.Mvvm). Do NOT reference Xamarin.Forms.
- ORM: backend uses Entity Framework Core with SQL Server. Follow existing repository and DbContext patterns.
- Naming: interfaces start with `I` (IAccountService, ITripRepository). Repositories end with `Repository`, services with `Service`, view models with `ViewModel`.
- DI: in API use AddScoped for repositories and business services. In MAUI follow existing lifetimes: AddSingleton for app/global state, AddTransient for Pages/ViewModels, typed AddHttpClient for API services.
- ViewModels: use CommunityToolkit.Mvvm patterns: `[ObservableProperty]`, `AsyncRelayCommand`, inherit from repository's `BaseViewModel`.

Safety checks before edits
-------------------------
1. Locate and open `AppDbContext` and `BaseViewModel` in the workspace. Use exact namespaces from the files before referencing them in generated code.
2. Run a local build (dotnet build or Visual Studio) after changes. Fix compile errors introduced by edits.

Entity Framework migrations
---------------------------
- Agents MAY create migration files when model changes exist (using `dotnet ef migrations add <Name> -p fabrizio.DAL -s fabrizio.API`).
- Agents MUST NOT apply migrations to the database. Migration application is exclusively the developer's responsibility.
- Always document the migration command in the PR description so the developer can review and apply it.

NuGet package management
------------------------
- Agents MUST NOT change package versions independently.
- Agents MUST NOT add new packages without explicit developer approval.
- Agents MUST NOT remove existing packages without explicit developer approval.
- If a package update is needed: ask the developer, propose the specific version, describe required code changes, and wait for approval before proceeding.

Git & PR
--------
- Branch naming: `feature/agent/<short-desc>` or `fix/agent/<short-desc>`.
- Commit message template: `<scope>: <short summary>

  - Body: one-line description of why change was made
  - Footer: references (issue/PR)`
- Create a draft PR targeting `master` for agent-created branches.

Use skills
----------
This repository exposes reusable skills under `.github/skills/`. Prefer calling those skill documents when performing common tasks (add CRUD endpoint, add MAUI page, wire an authenticated HttpClient or DTO mapping).

Architecture & project boundaries
----------------------------------
This repository follows a strict **layered architecture** where dependency direction ALWAYS flows downward. All agents MUST respect these boundaries when adding features or fixing bugs.

See: **[Layered Architecture & Project Boundaries](.github/docs/ARCHITECTURE.md)** for detailed rules, file organization patterns, and the bottom-up workflow for adding features.

Key rules:
- No circular dependencies. No upward references.
- Controllers are thin. All domain logic belongs in BLL services.
- Repositories expose only interfaces, never DbContext.
- DTOs in fabrizio.DTO only. No business logic there.
- Always start from DAL and work upward: DAL → Repository → BLL → API/App.
