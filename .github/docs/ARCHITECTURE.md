# Layered Architecture & Project Boundaries

This repository enforces a strict **layered architecture** where dependency direction ALWAYS flows downward. Violating these boundaries breaks the design and causes maintainability issues.

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
4. **Repositories wrap DbContext.** `fabrizio.Repository` exposes only interfaces (`I*Repository`). The `DbContext` is never exposed to upper layers. Note: Repositories may expose `IQueryable<T>` from `QueryAll()` for read operations; upper layers (BLL) apply filtering/sorting/paging. This is a known trade-off (leaky abstraction) accepted to avoid overloading repository methods. Future refactoring may introduce explicit query specifications, but current code follows this pattern.
5. **Shared contracts only in DTOs.** Only DTOs, enums, and constants belong in `fabrizio.DTO`. Never place business logic, service implementations, or repository implementations there.
6. **File organization mirrors layers:**
   - Entities: `fabrizio.DAL/` (project root; namespace `fabrizio.DAL.Entities`; no physical `Entities/` folder); infrastructure files prefixed with `_` (e.g., `_AppDbContext.cs`, `_BaseEntity.cs`).
   - Repository interfaces & implementations: `fabrizio.Repository/` (one file per aggregate; both interface and implementation in the same file; namespace `fabrizio.Repository`).
   - Business logic: `fabrizio.BLL/` (project root; one file per aggregate, e.g., `Trip.cs`; larger services use `partial` in adjacent files; namespace `fabrizio.BLL`).
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
- **Controllers:** Translate failures via `result.ToProblem()` (extension in `fabrizio.API/Extensions/ResultExtensions.cs`), which returns an `IActionResult` (`ObjectResult` wrapping a `ProblemDetails`). Success case: `return Ok(result.Value);`.
- **MAUI clients:** Consume `Result<T>` (or `Result<PagedResult<T>>`) from the API; on error, deserialize to `ApiProblem` and rewrap in a local `Result<T>` for UI consumption.
- **Authentication:** Extract accountId from claims: `User.FindFirstValue("accountId")` + `int.TryParse` guard. Return `Unauthorized()` if claim missing or invalid.

## Current Deviations from Target Architecture

The following patterns exist in the codebase and should NOT be expanded or replicated in new code. Agents should note these as known trade-offs:

### Deviation 1: IQueryable<T> from Repositories
- **Pattern:** Repositories expose `IQueryable<T>` from `QueryAll()` for read operations.
- **Violation:** Leaky abstraction; upper layers (BLL) apply filtering/sorting/paging.
- **Reason:** Avoids overloading repository methods; enables efficient query composition.
- **Status:** Accepted trade-off; do not expand this pattern to new aggregates without explicit approval.
- **Refactoring path:** Replace with explicit query specifications (e.g., Specification pattern) in future refactoring.
- **Agent guidance:** When modifying code that uses this pattern, match the existing style. In new code, seek permission before adopting IQueryable.

### Deviation 2: Direct AppDbContext Usage in Services
- **Pattern:** `AccountService` and `TripService` inject `AppDbContext` directly for certain read operations, not just via repositories.
- **Violation:** BLL should only use repositories, not DbContext.
- **Reason:** Historical; allowed for performance-critical queries.
- **Status:** Known tech debt; do not expand to new services.
- **Refactoring path:** Migrate to repository methods or explicit query specifications.
- **Agent guidance:** When modifying these services, use repositories where possible. If DbContext is required, match the style and document why (performance, circular-dependency avoidance, etc.).

## How Agents Should Apply These Rules

1. **New code:** Always follow the target architecture (Rule #1–6). Do not introduce new deviations.
2. **Existing deviations:** When modifying code that deviates from the target, preserve the deviation's style and match the surrounding code. Mark deviations in PR description ("Note: This file uses IQueryable in QueryAll(); matched existing style.").
3. **Refactoring existing deviations:** If you discover an opportunity to fix a deviation, propose it separately in a comment or as a future task. Do not mix deviation fixes with feature additions.

## Technical Debt Summary

If you discover OTHER code that violates these boundaries (outside Known Deviations):

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
