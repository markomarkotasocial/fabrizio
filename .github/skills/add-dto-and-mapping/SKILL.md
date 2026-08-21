Skill: add-dto-and-mapping
==========================

Purpose
-------
Add DTO classes to `fabrizio.DTO` and implement mapping between entities and DTOs in BLL following repository conventions.

Steps
-----
1. Create DTO class in `fabrizio.DTO` (project is referenced by BLL and MAUI). Keep DTOs minimal and serializable.
2. If DTOs are shared between API and MAUI, prefer `fabrizio.DTO` to avoid duplication. Update ProjectReference on projects that need them.
3. Implement mapping in `fabrizio.BLL` service method(s). Use manual mapping in the service or a static mapping helper (avoid adding large mapping libraries unless necessary).
   - Example mapping:
	 var dto = new EntityDto { Id = entity.Id, Name = entity.Name };
4. Use DTOs in controller actions for inputs and outputs. Convert to/from entities in BLL layer.
5. Add unit tests for mapping behavior if mapping contains business logic.

Notes
-----
- Keep entity navigation properties away from DTOs unless required. Use ID references or nested DTOs explicitly.
- When adding properties, consider backward compatibility with clients.
