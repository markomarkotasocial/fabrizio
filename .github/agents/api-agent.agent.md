---
name: api-agent
role: backend
scope: "fabrizio.API, fabrizio.Repository, fabrizio.DAL, fabrizio.BLL"
description: "Agent specialized in implementing and modifying backend REST API features for this repository. Works with EF Core, Repository pattern, BLL services, DTOs and Swagger/JWT configuration."
---

Agent rules
-----------
- Work only in the backend projects listed in frontmatter. Do not modify MAUI project files.
- Before making any code changes, locate and open the actual `_AppDbContext` source file and entity definitions in `fabrizio.DAL/` (project root; namespace `fabrizio.DAL.Entities`). Confirm namespaces, DbSet names, and that no physical `Entities/` folder exists.
- Use existing repository pattern: create or update `I{Name}Repository` in `fabrizio.Repository` and an implementation that uses `AppDbContext`. Follow method signatures already present. Note: repositories may expose `IQueryable<T>` from `QueryAll()` for read operations; this is a known trade-off (leaky abstraction) for efficient query composition in BLL. Do not expose `DbContext` directly to upper layers.
- Services in `fabrizio.BLL` are organized one per aggregate in root folder (e.g., `Trip.cs`). Namespace `fabrizio.BLL`. Larger services use `partial` in adjacent files (e.g., `AccommodationBooking.cs`).
- Add DTOs to `fabrizio.DTO/DTO/` (namespace `fabrizio.Shared.DTO`) when shape differs from entities. Contracts go in `fabrizio.DTO/Contracts/` (namespace `fabrizio.Shared.Contracts`). Remember: project assembly is `fabrizio.Shared`.
- Place business logic in `fabrizio.BLL` services; controllers in `fabrizio.API` should be thin and call BLL services.
- Register new repositories/services in `fabrizio.API/Program.cs` using `AddScoped`. All repositories and services use Scoped lifetime.

Error handling & the Result pattern
-----------------------------------
- BLL services return `Result` / `Result<T>` for expected business errors. Build them with `Result.Success()` / `Result<T>.Success(value)` and `Result.Fail(error)` / `Result<T>.Fail(error)`, where `error` is a `BusinessError(string Code, string Message, int HttpStatusCode)` record from `fabrizio.Shared.Contracts`.
- Lists are returned as `Result<PagedResult<T>>.Success(...)`.
- Argument errors (null checks, out-of-range, etc.) still `throw` (e.g., `ArgumentNullException.ThrowIfNull()`); these are programmer errors, not business failures.
- Controllers translate failures via `result.ToProblem()` (extension in `fabrizio.API/Extensions/ResultExtensions.cs`), which returns an `IActionResult` wrapping a `ProblemDetails`. Success case: `return Ok(result.Value);`.
- Extract `accountId` from claims: `var accountIdClaim = User.FindFirstValue("accountId");` + `int.TryParse` guard. Return `Unauthorized()` if claim is missing or invalid.

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
- Follow existing JWT auth setup in `Program.cs`. Do not change token validation defaults without explicit reason. If endpoints require authorization, use `[Authorize]` attributes and document required scopes/roles.
- Update Swagger (Swashbuckle) security definitions when adding authenticated endpoints.

Verification
------------
- Build the API project: `dotnet build fabrizio.API/fabrizio.API.csproj -c Release` (mirrors CI in `.github/workflows/master_fabrizio.yml`, which only builds the API project and does not build MAUI). Fix compile errors.
- Verify endpoints manually via Swagger UI or `fabrizio.API/fabrizio.API.http`.
- This solution has no automated test project. If a change would benefit from tests, recommend it in the PR description. Do not add test packages or a test project without explicit developer approval.

Handoff notes
-------------
When the API contract (DTOs and routes) is stable, produce a small API contract artifact for the frontend: DTO definitions, example JSON payloads, and sample curl commands.
