# Layered Architecture & Project Boundaries

This repository enforces a strict **layered architecture** where dependency direction ALWAYS flows downward. Violating these boundaries breaks the design and causes maintainability issues.

> **Before writing code, read [CONVENTIONS.md](CONVENTIONS.md)** — the canonical helpers (`AuthorizedControllerBase`, `ToActionResult`, `RepositoryBase<T>`, the BLL mapping extensions, `LoadOwnedTripAsync`, `HttpResultExtensions`, `INavigationService`, `*ChangedMessage`). Use them instead of hand-rolling the boilerplate they replace.

## Project Structure

```
fabrizio app (solution)
├── fabrizio.App          → .NET MAUI client (mobile UI)
├── fabrizio.API          → REST API controllers
├── fabrizio.BLL          → Business logic services
├── fabrizio.Repository   → Repository pattern (data access abstraction)
├── fabrizio.DAL          → EF Core, DbContext, entities
└── fabrizio.DTO          → Shared DTOs & contracts
```

## Allowed Dependencies

Dependency flow is **strictly unidirectional downward**:

- `fabrizio.App` (MAUI) → `fabrizio.DTO` only
- `fabrizio.API` (Controllers) → `fabrizio.BLL`, `fabrizio.DTO`
- `fabrizio.BLL` (Services) → `fabrizio.Repository`, `fabrizio.DTO`
- `fabrizio.Repository` (Repository pattern) → `fabrizio.DAL`
- `fabrizio.DAL` (EF Core, DbContext, entities) → **no internal references**
- `fabrizio.DTO` (Shared contracts) → **no internal references**

## Non-Negotiable Rules

1. **No circular dependencies.** If A references B, then B must never reference A.
2. **No upward references.** Lower layers (DAL, Repository) must NEVER reference upper layers (BLL, API, App).
3. **Controllers are thin wrappers.** Controllers in `fabrizio.API` only handle HTTP concerns (input validation, routing, response formatting). All domain logic belongs in `fabrizio.BLL` services.
4. **Repositories wrap DbContext.** `fabrizio.Repository` exposes only interfaces (`I*Repository : IRepository<TEntity>`). `AppDbContext` / `DbSet<T>` never leave the repository layer, and **BLL service constructors take only `I*Repository`** (never `AppDbContext`). The one accepted EF leak is `IQueryable<T>` returned from a `QueryAll(...)` method: the BLL composes filtering/sorting/paging over it. This is deliberate — it avoids a combinatorial explosion of repository overloads — and is the *only* place EF surfaces upward. `Add` / `Delete` / `SaveChangesAsync` come from `RepositoryBase<TEntity>`; a concrete repository only adds its aggregate-specific queries.
5. **Shared contracts only in DTOs.** Only DTOs, enums, and constants belong in `fabrizio.DTO`. Never place business logic, service implementations, or repository implementations there.
6. **File organization mirrors layers:**
   - Entities: `fabrizio.DAL/` (project root; namespace `fabrizio.DAL.Entities`; no physical `Entities/` folder); infrastructure files prefixed with `_` (e.g., `_AppDbContext.cs`, `_BaseEntity.cs`).
   - Repository interfaces & implementations: `fabrizio.Repository/` (one file per aggregate; interface + implementation in the same file; namespace `fabrizio.Repository`). `RepositoryBase<TEntity>` / `IRepository<TEntity>` live in `RepositoryBase.cs`.
   - Business logic: `fabrizio.BLL/` (project root; one file per aggregate, e.g., `Trip.cs`; larger services use `partial` in adjacent files; namespace `fabrizio.BLL`). Entity → DTO mapping goes in `fabrizio.BLL/Mapping/*MappingExtensions.cs` as `ToDto()` extension methods (not inline in the service).
   - DTOs: `fabrizio.DTO/DTO/` (namespace `fabrizio.Shared.DTO`); contracts: `fabrizio.DTO/Contracts/` (namespace `fabrizio.Shared.Contracts`). Note: project assembly name is `fabrizio.Shared` (file: `fabrizio.DTO/fabrizio.Shared.csproj`).
   - Controllers: `fabrizio.API/Controllers/` (PLURAL naming, e.g., `TripsController`; route `[Route("api/trips")]`; namespace `fabrizio.API.Controllers`).
   - MAUI Pages: `fabrizio.App/Pages/<Area>/`; ViewModels: `fabrizio.App/ViewModels/`; Services: `fabrizio.App/Services/` (interface + implementation in same file); namespace `fabrizio.App` and subdomains (e.g., `fabrizio.App.Services`).

## When Adding Features or Fixing Bugs

**Always follow this bottom-up workflow:**

1. **Start from the bottom (DAL):** Create or modify entities in `fabrizio.DAL/` (project root).
2. **Add repository methods:** Create/update repository interface and implementation in `fabrizio.Repository/` (one file per aggregate).
3. **Implement business logic:** Add service methods in `fabrizio.BLL/` (project root; one file per aggregate).
4. **Create DTOs (if needed):** Add request/response DTOs in `fabrizio.DTO/DTO/` (namespace `fabrizio.Shared.DTO`) if shapes differ from entities.
5. **Wire controllers:** Add controller endpoints in `fabrizio.API/Controllers/` (PLURAL naming, e.g., `TripsController`).
6. **Add client-side code:** Create ViewModels, services, and Pages in `fabrizio.App/`.

**Do NOT shortcut:** Do not have upper layers reference lower layers out of order. This creates technical debt and violates the dependency principle.

## Error Handling & the Result Pattern

BLL services in this codebase return `Result` / `Result<T>` for expected business errors:

- **Success path:** `Result.Success()` (no value), `Result<T>.Success(value)`, or `Result<PagedResult<T>>.Success(...)` for lists.
- **Business failure:** `Result.Fail(new BusinessError(code, message, httpStatusCode))` or `Result<T>.Fail(...)`. `BusinessError` is a `record` in `fabrizio.Shared.Contracts` with positional members `Code`, `Message`, `HttpStatusCode`.
- **Programmer errors:** `ArgumentNullException.ThrowIfNull()`, `throw new ArgumentException()` — these are NOT expressed as a `Result`.
- **Field validation is the BLL's job**, not the DTO's. `Program.cs` suppresses the implicit `[Required]` on non-nullable reference types, so a missing/blank field reaches the service and comes back as `Result.Fail(new BusinessError(...))`. Do not put `[Required]` / data annotations on request DTOs.
- **Controllers:** one line — `return result.ToActionResult();` (`ResultExtensions`: generic → `200` + value, non-generic → `204`, failure → `ProblemDetails` at the error's status). `ToProblem()` is what `ToActionResult` calls for the failure case.
- **Auth:** the controller inherits `AuthorizedControllerBase`; `if (!TryGetAccountId(out var accountId)) return Unauthorized();`.
- **MAUI clients:** call the `HttpResultExtensions` helpers (`GetResultAsync<T>`, `PostResultAsync`, …); each returns `Result` / `Result<T>`, mapping a non-2xx `ProblemDetails` to a `BusinessError` and a transport failure to `network_error`.

## Accepted trade-offs

- **`IQueryable<T>` from `QueryAll(...)`.** Deliberate read seam (see Rule #4). New aggregates may follow it. If you would rather not, a concrete paged repository method (`GetPageAsync(...)`) is the alternative — not a Specification framework.

## Possible follow-ups (not started)

- **`IUnitOfWork`.** `SaveChangesAsync()` currently sits on `RepositoryBase<T>`, so any repository can save the shared `DbContext`. A single injected `IUnitOfWork.SaveChangesAsync()` (repositories only stage `Add`/`Delete`) would remove the "which repository do I save on?" ambiguity. Touches every BLL method that writes.

New code always follows the target architecture (Rules #1–6); do not introduce new deviations.

## Technical Debt

If you discover code that violates these boundaries:

- **Do not replicate the violation.** Keep your changes clean.
- **Flag it as technical debt** in a comment or issue.
- **Propose a refactoring** if the violation is blocking progress.

## Why This Matters

A layered architecture provides:

- **Testability:** Each layer can be tested independently.
- **Reusability:** BLL logic is not coupled to API controllers or MAUI pages.
- **Maintainability:** Changes in one layer don't cascade unexpectedly to others.
- **Clarity:** New developers understand where each type of code belongs.
- **Scalability:** Adding new features doesn't require touching unrelated layers.

## Dependency Visualization

```
fabrizio.App (MAUI UI)
	 ↓
fabrizio.DTO (Shared contracts)
	 ↑      ↑
	 │      │
	 │   fabrizio.API (Controllers)
	 │      │
	 └──→ fabrizio.BLL (Services)
		  │
	  fabrizio.Repository (Abstractions)
		  │
	  fabrizio.DAL (EF Core)
		  │
	  Azure SQL Server
```

Each layer depends only on layers below it. Upper layers never reference lower layers directly.
