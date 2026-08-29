Skill: add-crud-endpoint
========================

Purpose
-------
Guided steps to add a new CRUD resource. Assumes [.github/docs/CONVENTIONS.md](../../docs/CONVENTIONS.md) — use the helpers named there.

Steps
-----
1. Define entity in `fabrizio.DAL` (project root, namespace `fabrizio.DAL.Entities`; no physical `Entities/` folder).
2. Add a `DbSet<TEntity>` to `_AppDbContext.cs` (locate file and use exact namespace).
3. Create EF migration:
   - `dotnet ef migrations add Add{EntityName} -p fabrizio.DAL -s fabrizio.API`
   - Do NOT run `dotnet ef database update`. The developer applies migrations after review. Put the exact command in the PR description.
4. Repository in `fabrizio.Repository` (one file, interface + class):
   - `public interface I{Entity}Repository : IRepository<{Entity}> { /* only entity-specific queries */ }`
   - `public class {Entity}Repository : RepositoryBase<{Entity}>, I{Entity}Repository { public {Entity}Repository(AppDbContext c) : base(c) { } ... }`
   - `Add` / `Delete` / `SaveChangesAsync` and `Context` come from the base — do not re-declare them. Add `GetById`, any `HasOverlapping…`, and `IQueryable<{Entity}> QueryAll(...)` only if a composable read is needed.
5. DTO(s) in `fabrizio.DTO/DTO/` (namespace `fabrizio.Shared.DTO`); contracts in `fabrizio.DTO/Contracts/` (namespace `fabrizio.Shared.Contracts`). Project assembly is `fabrizio.Shared`. **No `[Required]` / data annotations** on request DTOs — the BLL validates.
6. Mapping in `fabrizio.BLL/Mapping/{Entity}MappingExtensions.cs`: `public static {Entity}Dto ToDto(this {Entity} e) => new() { ... };`. The service calls `entity.ToDto()` — never inline `new {Entity}Dto { ... }`.
7. BLL service in `fabrizio.BLL/{Entity}.cs` (namespace `fabrizio.BLL`), constructor takes only `I{Entity}Repository` (+ other BLL services). Methods return `Result` / `Result<T>` / `Result<PagedResult<T>>`. For a resource scoped to another aggregate (e.g. a trip), reuse a `LoadOwned…Async` guard for the 404 / 403 / cancelled ladder.
8. Register in `fabrizio.API/Program.cs`: `AddScoped<I{Entity}Repository, {Entity}Repository>()` and `AddScoped<I{Entity}Service, {Entity}Service>()`.
9. Controller `fabrizio.API/Controllers/{EntityPlural}Controller` (e.g. `TripsController`), `: AuthorizedControllerBase`, `[Route("api/{entity-plural}")]`, RESTful actions. Each action:
   ```csharp
   if (!TryGetAccountId(out var accountId)) return Unauthorized();
   var result = await _service.X(accountId, ...);
   return result.ToActionResult();
   ```
10. Build: `dotnet build fabrizio.API/fabrizio.API.csproj -c Release` (mirrors CI). Draft PR with the diff, migration files, example curl snippets, and the exact `migrations add` command.

Notes
-----
- Controllers stay one-liner-thin; all domain logic and field validation is in the BLL.
- `IQueryable<T>` from `QueryAll()` is the accepted read seam — BLL applies filtering/sorting/paging over it.
