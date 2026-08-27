Skill: add-authenticated-http-client
===================================

Purpose
-------
Add a typed HttpClient service that uses the existing TokenHandler message handler for authenticated API calls.

Steps
-----
1. Define service interface and implementation in `fabrizio.App/Services/` in the same file (e.g., `Example.cs` contains both `IExampleService` and `ExampleService`). Note: `Services/Abstractions/` is reserved for cross-cutting state contracts like `IAccountState`; most API service interfaces live alongside their implementations in `Services/`.
2. Implement `ExampleService` in `fabrizio.App/Services/Example.cs` that accepts `HttpClient` in constructor and calls JSON endpoints.
3. Register service in `MauiProgram.cs` using a typed client and the `TokenHandler`, matching existing services:
   ```csharp
   builder.Services.AddHttpClient<IExampleService, ExampleService>(client =>
   {
	   client.BaseAddress = new Uri("https://<api-base>/");
	   client.DefaultRequestHeaders.Add("Accept", "application/json");
   }).AddHttpMessageHandler<TokenHandler>();
   ```
4. Inject `IExampleService` into ViewModels and use methods. Handle exceptions and propagate meaningful messages to UI. Services may return `Result<T>` or `PagedResult<T>` from the API; deserialize these into `ApiProblem` on error and rewrap in `Result<T>` for UI consumption (see how existing `ITripService` and `IProfileService` handle this in `fabrizio.App/Services/`).

Verification
------------
- Build the MAUI project: `dotnet workload install maui` (first time), then `dotnet build fabrizio.App/fabrizio.App.csproj`.
- Run against running API (local or deployed). Verify token is attached by inspecting requests or using a proxy (e.g., Fiddler or Charles).
- This solution has no automated test project. Verify changes by testing the app manually on your platform target.
