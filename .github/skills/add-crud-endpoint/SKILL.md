Skill: add-crud-endpoint
========================

Purpose
-------
Guided steps to add a new CRUD resource to this solution following its conventions.

Steps
-----
1. Define entity in `fabrizio.DAL` (project root, namespace `fabrizio.DAL.Entities`; no physical `Entities/` folder).
2. Add a `DbSet<TEntity>` to `_AppDbContext.cs` (locate file and use exact namespace).
3. Create EF migration:
   - `dotnet ef migrations add Add{EntityName} -p fabrizio.DAL -s fabrizio.API`
   - Do NOT run `dotnet ef database update`. Migrations are applied by the developer after review. Include the exact migration command in the PR description.
4. Add repository interface `I{Entity}Repository` and implementation `EntityRepository` in `fabrizio.Repository` (one file per aggregate). Follow existing patterns: QueryAll, GetById, Add, Delete, SaveChangesAsync.
5. Add DTO(s) in `fabrizio.DTO` when DTO shape differs from entity. Remember: the project is named `fabrizio.Shared`; DTOs go in `fabrizio.DTO/DTO/` (namespace `fabrizio.Shared.DTO`); contracts go in `fabrizio.DTO/Contracts/` (namespace `fabrizio.Shared.Contracts`).
6. Add mapping methods in `fabrizio.BLL` service (one file per aggregate, e.g., `Trip.cs`). Keep service thin but own domain rules.
7. Register repository and BLL service in `fabrizio.API/Program.cs` using `AddScoped<I{Repository}, {Repository}>` and `AddScoped<I{Service}, {Service}>`.
8. Add controller in `fabrizio.API/Controllers` named `{EntityPlural}Controller` (e.g., `TripsController`) with `[Route("api/{entity-plural}")]` and RESTful endpoints: GET (list), GET (by id), POST, PUT, DELETE. BLL service methods should return `Result<T>` or `PagedResult<T>` for expected business errors; controllers translate via `result.ToProblem()` (see `fabrizio.API/Extensions/ResultExtensions.cs`). Use DTOs for input/output.
9. Add Swagger comments and example responses. Protect endpoints with `[Authorize]` if needed. Extract accountId from claims: `User.FindFirstValue("accountId")` + null check and `int.TryParse` guard; return `Unauthorized()` on failure.
10. Build the API project: `dotnet build fabrizio.API/fabrizio.API.csproj -c Release` (mirrors CI in `.github/workflows/master_fabrizio.yml`, which only builds the API project). Create a draft PR with changes and migration files. Include example curl snippets and the exact `migrations add` command in PR description.

Notes
-----
- Follow existing naming conventions and DI lifetimes. Keep controllers thin; all domain logic belongs in BLL.
- The repository pattern in this codebase exposes `IQueryable<T>` from `QueryAll()` for read operations; upper layers (BLL) apply filtering/sorting/paging. This is a known trade-off (leaky abstraction) accepted to avoid overloading repository methods.
