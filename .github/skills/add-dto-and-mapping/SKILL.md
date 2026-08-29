Skill: add-dto-and-mapping
==========================

Purpose
-------
Add a DTO to `fabrizio.DTO` and its entity → DTO mapping. Assumes [.github/docs/CONVENTIONS.md](../../docs/CONVENTIONS.md).

Steps
-----
1. Create the DTO in `fabrizio.DTO/DTO/` (namespace `fabrizio.Shared.DTO`); contracts in `fabrizio.DTO/Contracts/` (namespace `fabrizio.Shared.Contracts`). Assembly is `fabrizio.Shared`; it is already referenced by `fabrizio.BLL` (→ API) and `fabrizio.App`, so no new `ProjectReference`.
2. **DTOs are pure data** — no computed/presentation members (those go on a MAUI client wrapper or ViewModel), no `[Required]` / data annotations (the BLL validates), no entity navigation objects (use ID references or explicit nested DTOs).
3. Mapping goes in `fabrizio.BLL/Mapping/{Aggregate}MappingExtensions.cs` as a static extension — NOT inline in the service:
   ```csharp
   public static {Entity}Dto ToDto(this {Entity} e) => new()
   {
	   Id = e.Id,
	   Name = e.Name,
	   // child collections via their own ToDto(): (e.Children ?? Enumerable.Empty<Child>()).Select(c => c.ToDto()).ToList()
   };
   ```
   The service and other mappers call `e.ToDto()`. No mapping libraries.
4. Controllers use DTOs for input/output; conversion to/from entities happens in the BLL. Service methods return `Result` / `Result<T>` / `Result<PagedResult<T>>`.
5. Verify: `dotnet build fabrizio.API/fabrizio.API.csproj -c Release`. No automated test project — check the JSON via Swagger / `.http`.

Notes
-----
- When adding a property, consider backward compatibility with the deployed MAUI build.
