Skill: add-dto-and-mapping
==========================

Purpose
-------
Add DTO classes to `fabrizio.DTO` and implement mapping between entities and DTOs in BLL following repository conventions.

Steps
-----
1. Create DTO class in `fabrizio.DTO` (project is referenced by BLL and MAUI; note: assembly name is `fabrizio.Shared`). DTOs go in `fabrizio.DTO/DTO/` (namespace `fabrizio.Shared.DTO`); contracts go in `fabrizio.DTO/Contracts/` (namespace `fabrizio.Shared.Contracts`). Keep DTOs minimal and serializable.
2. If DTOs are shared between API and MAUI, prefer `fabrizio.DTO` to avoid duplication. Update ProjectReference on projects that need them.
3. Implement mapping in `fabrizio.BLL` service method(s) (one file per aggregate). Use manual mapping in the service or a static mapping helper (avoid adding large mapping libraries unless necessary).
   - Example mapping:
	 ```csharp
	 var dto = new EntityDto { Id = entity.Id, Name = entity.Name };
	 ```
4. Use DTOs in controller actions for inputs and outputs. Convert to/from entities in BLL layer. BLL service methods return `Result<T>` or `PagedResult<T>` for expected business errors.
5. Verify schema is correct by running `dotnet build fabrizio.API/fabrizio.API.csproj -c Release` (mirrors CI).

Notes
-----
- Keep entity navigation properties away from DTOs unless required. Use ID references or nested DTOs explicitly.
- When adding properties, consider backward compatibility with clients.
- This solution has no automated test project. Verify mapping changes by building and testing endpoints manually via Swagger or `.http` files.
