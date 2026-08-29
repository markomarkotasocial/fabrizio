---
name: feature-orchestrator
role: orchestrator
subagents:
  - api-agent
  - maui-agent
description: "Coordinate full-stack feature delivery. Delegates backend work to api-agent, then frontend work to maui-agent after API contract is stable."
---

Orchestrator rules
------------------
- Lead with a minimal API contract (DTOs + routes + status codes). Ask api-agent to implement the contract first.
- Do not start frontend implementation until api-agent confirms DTO shapes and exposes Swagger examples or sample JSON.
- Produce handoff artifacts: DTO files, example request/response JSON, curl requests, and a short integration checklist.

Both agents assume [.github/docs/CONVENTIONS.md](../docs/CONVENTIONS.md). The wire contract is fixed by it: a `2xx` carries the **bare value** (or `204`), a `4xx` carries a `ProblemDetails`; list endpoints are `Result<PagedResult<T>>` server-side → bare `PagedResult<T>` on the wire.

Recommended flow
----------------
1. Define the API contract (DTOs, routes, HTTP methods, success/error status codes). Send to api-agent.
2. api-agent implements backend endpoints (controllers via `AuthorizedControllerBase` + `ToActionResult`, BLL via `Result`, repos via `RepositoryBase`), creates migration files (agents do NOT apply migrations), updates DI, and provides example JSON payloads.
3. Once the backend reports a stable contract, ask maui-agent to: reuse the shared DTOs; add/extend an `I{X}Service` whose methods delegate to `HttpResultExtensions`; add ViewModel + Page.
4. Integration: run the API (local or deployed base address), run the MAUI app against it, validate end-to-end.

Handoff artifact checklist
-------------------------
- DTO definitions (C#, in `fabrizio.Shared`) and example JSON
- Endpoint list: method, route, auth requirement, request/response shape and status codes (bare value on `2xx` / `204`, `ProblemDetails` on `4xx`)
- **Exact migration command** (e.g., `dotnet ef migrations add AddTrip -p fabrizio.DAL -s fabrizio.API`) for developer review. Agents create migration files but do NOT apply them.
