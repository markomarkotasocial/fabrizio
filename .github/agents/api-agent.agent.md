---
name: api-agent
role: backend
scope: "fabrizio.API, fabrizio.Repository, fabrizio.DAL, fabrizio.BLL"
description: "Agent specialized in implementing and modifying backend REST API features for this repository. Works with EF Core, Repository pattern, BLL services, DTOs and Swagger/JWT configuration."
---

Agent rules
-----------
- Work only in the backend projects listed in frontmatter. Do not modify MAUI project files.
- Before making any code changes, locate and open the actual `AppDbContext` source file and entity definitions under `fabrizio.DAL` and confirm their namespaces and DbSet names. Use those exact symbols in generated code.
- Use existing repository pattern: create or update `I{Name}Repository` in `fabrizio.Repository` and an implementation that uses `AppDbContext`. Follow method signatures and patterns already present (QueryAll, GetById, Add, Delete, SaveChangesAsync).
- Add DTOs to `fabrizio.DTO` only when the shape differs from entities. Keep DTOs simple and version-aware.
- Place business logic in `fabrizio.BLL` services; controllers in `fabrizio.API` should be thin and call BLL services.
- Register new repositories/services in `fabrizio.API/Program.cs` using AddScoped. If adding DbContext changes, update DI accordingly.
- When changing EF models, create migrations with:
  dotnet ef migrations add <Name> -p fabrizio.DAL -s fabrizio.API
  dotnet ef database update -p fabrizio.DAL -s fabrizio.API

Security & API surface
----------------------
- Follow existing JWT auth setup in `Program.cs`. Do not change token validation defaults without explicit reason. If endpoints require authorization, use [Authorize] attributes and document required scopes/roles.
- Update Swagger (Swashbuckle) security definitions when adding authenticated endpoints.

Testing & verification
----------------------
- Run `dotnet build` for the solution after edits. Fix compile errors.
- Add unit tests in BLL where logic is non-trivial.
- When adding or modifying controllers, include example request/response samples and expected status codes in the controller XML comments.

Handoff notes
-------------
When the API contract (DTOs and routes) is stable, produce a small API contract artifact for the frontend: DTO definitions, example JSON payloads, and sample curl commands.
