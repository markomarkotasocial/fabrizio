Skill: add-crud-endpoint
========================

Purpose
-------
Guided steps to add a new CRUD resource to this solution following its conventions.

Steps
-----
1. Define entity in `fabrizio.DAL` under `Entities/`.
2. Add a `DbSet<TEntity>` to `AppDbContext` (locate file and use exact namespace).
3. Add EF migration and apply locally:
   - dotnet ef migrations add Add{EntityName} -p fabrizio.DAL -s fabrizio.API
   - dotnet ef database update -p fabrizio.DAL -s fabrizio.API
4. Add repository interface `I{Entity}Repository` and implementation `EntityRepository` in `fabrizio.Repository`. Follow existing patterns: QueryAll, GetById, Add, Delete, SaveChangesAsync.
5. Add DTO(s) in `fabrizio.DTO` when DTO shape differs from entity (e.g., remove navigation properties or internal fields).
6. Add mapping methods in `fabrizio.BLL` service or mapping helpers (manual mapping). Keep BLL service thin but own domain rules.
7. Register repository and BLL service in `fabrizio.API/Program.cs` using AddScoped<I{Repository}, {Repository}> and AddScoped<I{Service}, {Service}>.
8. Add controller in `fabrizio.API/Controllers` named `{Entity}Controller` with RESTful endpoints: GET (list), GET (by id), POST, PUT, DELETE. Use DTOs for input/output and call BLL service methods.
9. Add Swagger comments and example responses. Protect endpoints with [Authorize] if needed.
10. Build solution and run tests. Create a draft PR with changes and migration files. Include example curl snippets in PR description.

Notes
-----
- Follow existing naming conventions and lifetimes. Keep controllers thin and let BLL handle validation and business rules.
