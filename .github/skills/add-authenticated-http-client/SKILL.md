Skill: add-authenticated-http-client
===================================

Purpose
-------
Add a MAUI API service. Assumes [.github/docs/CONVENTIONS.md](../../docs/CONVENTIONS.md).

Steps
-----
1. Interface + implementation in one file under `fabrizio.App/Services/` (e.g. `Example.cs` has both `IExampleService` and `ExampleService`). `Services/Abstractions/` is only for cross-cutting state contracts (`IAccountState`).
2. `ExampleService` takes `HttpClient` in its constructor. **Each method is one line** delegating to a `HttpResultExtensions` helper:
   ```csharp
   public Task<Result<ThingDto>> GetThing(Guid id) => _http.GetResultAsync<ThingDto>($"api/things/{id}");
   public Task<Result>          AddThing(CreateThingRequest r) => _http.PostResultAsync("api/things", r);
   public Task<Result<ThingDto>> UpdateThing(Guid id, UpdateThingRequest r) => _http.PutResultAsync<ThingDto>($"api/things/{id}", r);
   public Task<Result>          DeleteThing(Guid id) => _http.DeleteResultAsync($"api/things/{id}");
   ```
   The helper reads a non-2xx `ProblemDetails` into a `BusinessError` and a transport failure into `network_error` — do not add your own `try/catch`.
   For a list endpoint the wire shape is a bare `PagedResult<T>`: `_http.GetResultAsync<PagedResult<T>>(url)`, then map to whatever the interface exposes.
3. Register in `MauiProgram.cs` as a typed client with the `TokenHandler`:
   ```csharp
   builder.Services.AddHttpClient<IExampleService, ExampleService>(client =>
   {
	   client.BaseAddress = new Uri("https://<api-base>/");
	   client.DefaultRequestHeaders.Add("Accept", "application/json");
   }).AddHttpMessageHandler<TokenHandler>();
   ```
   Exception: a login-style call that must NOT carry a token / must NOT trigger logout-on-401 uses a standalone `HttpClient` (see `AuthService`), not this pattern.
4. Inject `IExampleService` into ViewModels; act on the returned `Result` (`IsSuccess`, `Value`, `Error?.Message`).

Verification
------------
- Build: `dotnet build fabrizio.App/fabrizio.App.csproj` (needs the `maui` workload; CI does not build MAUI).
- Run against the API; confirm the token is attached (a proxy such as Fiddler/Charles) and errors surface as readable messages.
- No automated test project — verify by running the app.
