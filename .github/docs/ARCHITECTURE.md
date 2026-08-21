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
4. **Repositories wrap DbContext.** `fabrizio.Repository` exposes only interfaces (`I*Repository`). Never expose `DbContext` or `IQueryable<T>` to upper layers.
5. **Shared contracts only in DTOs.** Only DTOs, enums, and constants belong in `fabrizio.DTO`. Never place business logic, service implementations, or repository implementations there.
6. **File organization mirrors layers:**
   - Entities: `fabrizio.DAL/Entities/`
   - Repository interfaces & implementations: `fabrizio.Repository/`
   - Business logic: `fabrizio.BLL/Services/`
   - DTOs: `fabrizio.DTO/`
   - Controllers: `fabrizio.API/Controllers/`

## When Adding Features or Fixing Bugs

**Always follow this bottom-up workflow:**

1. **Start from the bottom (DAL):** Create or modify entities in `fabrizio.DAL/Entities/`.
2. **Add repository methods:** Create/update repository interface and implementation in `fabrizio.Repository/`.
3. **Implement business logic:** Add service methods in `fabrizio.BLL/Services/`.
4. **Create DTOs (if needed):** Add request/response DTOs in `fabrizio.DTO/` if shapes differ from entities.
5. **Wire controllers:** Add controller endpoints in `fabrizio.API/Controllers/`.
6. **Add client-side code:** Create ViewModels, services, and Pages in `fabrizio.App/`.

**Do NOT shortcut:** Do not have upper layers reference lower layers out of order. This creates technical debt and violates the dependency principle.

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
