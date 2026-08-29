---
name: api-agent
role: backend
scope: "fabrizio.API, fabrizio.Repository, fabrizio.DAL, fabrizio.BLL"
description: "Agent specialized in implementing and modifying backend REST API features for this repository. Works with EF Core, Repository pattern, BLL services, DTOs and Swagger/JWT configuration."
---

Read [.github/docs/CONVENTIONS.md](../docs/CONVENTIONS.md) first — the backend helper table is the contract for this agent.

Agent rules
-----------
- Work only in the backend projects listed in frontmatter. Do not modify MAUI project files.
- Before making any code changes, locate and open the actual `_AppDbContext` source file and entity definitions in `fabrizio.DAL/` (project root; namespace `fabrizio.DAL.Entities`). Confirm namespaces, DbSet names, and that no physical `Entities/` folder exists.
- **Repository:** `interface I{X}Repository : IRepository<{X}>` and `class {X}Repository : RepositoryBase<{X}>, I{X}Repository` with `public {X}Repository(AppDbContext c) : base(c) { }`. `Add` / `Delete` / `SaveChangesAsync` and `Context` come from the base — do NOT re-declare them or an `_context` field. Add only aggregate-specific queries (`GetById`, `HasOverlapping…`, and `IQueryable<{X}> QueryAll(...)` for composable reads).
- **BLL services take only `I*Repository`** (and other BLL services) in their constructor. Never inject `AppDbContext` into `fabrizio.BLL`. Need a query the repository lacks? Add it to the repository interface.
- Services in `fabrizio.BLL` are one per aggregate in the root folder (e.g., `Trip.cs`), namespace `fabrizio.BLL`; larger services use `partial` in adjacent files. Entity → DTO mapping lives in `fabrizio.BLL/Mapping/{X}MappingExtensions.cs` as `ToDto()` extension methods — call `entity.ToDto()`, never build `new XDto { ... }` inline.
- For a trip-scoped resource, reuse a `LoadOwned…Async(accountId, id) → Result<...>` guard for the 404 / 403 / (cancelled) ladder (see `fabrizio.BLL/TripService.Guards.cs`); do not repeat the checks inline.
- **Request DTOs carry no validation attributes.** `Program.cs` sets `SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true`; a missing/blank field must produce a `Result.Fail(new BusinessError(...))` from the BLL, not an ASP.NET `[Required]` error.
- Add DTOs to `fabrizio.DTO/DTO/` (namespace `fabrizio.Shared.DTO`); contracts to `fabrizio.DTO/Contracts/` (namespace `fabrizio.Shared.Contracts`). Project assembly is `fabrizio.Shared`. Responses are typed DTOs (`LoginResponseDto`), not anonymous objects.
- Register new repositories/services in `fabrizio.API/Program.cs` using `AddScoped`.

Error handling & the Result pattern
-----------------------------------
- BLL services return `Result` / `Result<T>` for expected business errors. Build with `Result.Success()` / `Result<T>.Success(value)` and `Result.Fail(error)` / `Result<T>.Fail(error)`, where `error` is a `BusinessError(string Code, string Message, int HttpStatusCode)` positional record from `fabrizio.Shared.Contracts`.
- Lists: `Result<PagedResult<T>>.Success(...)` (the controller sends the bare `PagedResult<T>` on `200`).
- Argument errors (null checks, out-of-range, …) still `throw` — programmer errors, not business failures.
- **Controller action = one line:** `var result = await _service.X(...); return result.ToActionResult();` (`ResultExtensions`: generic → `200` + value, non-generic → `204`, failure → `ProblemDetails` at the error's status).
- **Auth:** controller inherits `AuthorizedControllerBase`; `if (!TryGetAccountId(out var accountId)) return Unauthorized();`.

EF migrations boundary
----------------------
- When changing EF models, agents MAY create migration files:
  `dotnet ef migrations add <Name> -p fabrizio.DAL -s fabrizio.API`
- Agents MUST NOT apply migrations: do not run `dotnet ef database update`. The developer is solely responsible for applying migrations after review.
- Document the migration command in the PR so the developer can review and apply it.

NuGet package policy
--------------------
- Agents MUST NOT change package versions, add new packages, or remove existing packages without explicit developer approval.
- If a package version update is required, ask the developer, propose the specific version and new version range, describe what code changes are needed, and wait for approval.

Security & API surface
----------------------
- Follow the existing JWT auth setup in `Program.cs`. Do not change token validation defaults without explicit reason. Put `[Authorize]` on endpoints that need it (the Swagger `Bearer` security scheme is global — no per-endpoint Swagger wiring).
- Secrets (`ConnectionStrings:DefaultConnection`, `Jwt:Key`) come from User Secrets locally / App Service settings in Azure — never add them to `appsettings*.json`.

Verification
------------
- Build the API project: `dotnet build fabrizio.API/fabrizio.API.csproj -c Release` (mirrors CI in `.github/workflows/master_fabrizio.yml`, which only builds the API project and does not build MAUI). Fix compile errors.
- Verify endpoints manually via Swagger UI or `fabrizio.API/fabrizio.API.http`.
- This solution has no automated test project. If a change would benefit from tests, recommend it in the PR description. Do not add test packages or a test project without explicit developer approval.

Handoff notes
-------------
When the API contract (DTOs and routes) is stable, produce a small API contract artifact for the frontend: DTO definitions, example JSON payloads, and sample curl commands.
