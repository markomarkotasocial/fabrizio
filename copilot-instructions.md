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
1. Locate and open `_AppDbContext.cs` (namespace `fabrizio.DAL.Entities`) and `_BaseViewModel.cs` (namespace `fabrizio.App.ViewModels`) in the workspace. Use exact namespaces from the files.
2. Build the relevant project after changes:
   - Backend changes: `dotnet build fabrizio.API/fabrizio.API.csproj -c Release` (mirrors CI; do not build entire solution).
   - MAUI changes: `dotnet workload install maui` (first time only), then `dotnet build fabrizio.App/fabrizio.App.csproj` (CI does not build MAUI).
   - Fix any compile errors introduced by edits.

Error handling & Result pattern
--------------------------------
- BLL services return `Result` / `Result<T>` for expected business errors (not exceptions).
- Build results with `Result.Success()` / `Result<T>.Success(value)` and `Result.Fail(new BusinessError(code, message, httpStatusCode))` — `BusinessError` is a record in `fabrizio.Shared.Contracts`.
- Argument errors still `throw` (e.g., `ArgumentNullException.ThrowIfNull()`).
- Controllers call `result.ToProblem()` to convert a failed `Result` into an `IActionResult` (a `ProblemDetails` payload).
- MAUI clients deserialize `Result<T>` or `ApiProblem` from API responses and rewrap in local `Result<T>` for UI.
- See [Error Handling & the Result Pattern](.github/docs/ARCHITECTURE.md#error-handling--the-result-pattern) for details.

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
- **Branch creation:** Agents MUST NOT create branches independently. Only create a branch when explicitly instructed by the developer. Always confirm the task scope and approach with the developer before branching.
- Branch naming (when instructed): `feature/agent/<short-desc>` or `fix/agent/<short-desc>`.
- Commit message template: `<scope>: <short summary>

  - Body: one-line description of why change was made
  - Footer: references (issue/PR)`
- Create a draft PR targeting `master` for agent-created branches.
- After completing work, notify the developer with a summary of changes and PR link for review.

Use skills
----------
This repository exposes reusable skills under `.github/skills/`. Prefer calling those skill documents when performing common tasks (add CRUD endpoint, add MAUI page, wire an authenticated HttpClient or DTO mapping).

Architecture & project boundaries
----------------------------------
This repository follows a strict **layered architecture** where dependency direction ALWAYS flows downward. All agents MUST respect these boundaries when adding features or fixing bugs.

See: **[Layered Architecture & Project Boundaries](.github/docs/ARCHITECTURE.md)** for detailed rules, file organization patterns, Error handling & Result pattern, known deviations, and the bottom-up workflow.

Key file locations:
- Backend entities: `fabrizio.DAL/` (project root; namespace `fabrizio.DAL.Entities`; infrastructure with `_` prefix; no `Entities/` folder).
- Repositories: `fabrizio.Repository/` (one file per aggregate; namespace `fabrizio.Repository`).
- BLL services: `fabrizio.BLL/` (project root; one file per aggregate; namespace `fabrizio.BLL`; no `Services/` folder).
- DTOs: `fabrizio.DTO/DTO/` (namespace `fabrizio.Shared.DTO`); Contracts: `fabrizio.DTO/Contracts/` (namespace `fabrizio.Shared.Contracts`).
- Controllers: `fabrizio.API/Controllers/` (PLURAL; namespace `fabrizio.API.Controllers`).
- MAUI Pages: `fabrizio.App/Pages/<Area>/`; ViewModels: `fabrizio.App/ViewModels/`; Services (interface + impl same file): `fabrizio.App/Services/`.

Key rules:
- No circular or upward dependencies.
- Controllers thin; domain logic in BLL services.
- Repositories expose interfaces only; BLL may use `IQueryable<T>` from `QueryAll()` for query composition (known trade-off).
- Result pattern: BLL returns `Result<T>` for business errors; controllers call `result.ToProblem()`.
- Always bottom-up: DAL → Repository → BLL → API/App.
