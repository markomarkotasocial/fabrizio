Skill: add-authenticated-http-client
===================================

Purpose
-------
Add a typed HttpClient service that uses the existing TokenHandler message handler for authenticated API calls.

Steps
-----
1. Define service interface in `fabrizio.App/Services/Abstractions`, e.g. `IExampleApiService` with async methods matching API endpoints.
2. Implement `ExampleApiService` in `fabrizio.App/Services` that accepts `HttpClient` in ctor and calls JSON endpoints.
3. Register service in `MauiProgram.cs` using a typed client and the TokenHandler, matching existing services:
   builder.Services.AddHttpClient<IExampleApiService, ExampleApiService>(client =>
   {
	   client.BaseAddress = new Uri("https://<api-base>/");
	   client.DefaultRequestHeaders.Add("Accept", "application/json");
   }).AddHttpMessageHandler<TokenHandler>();
4. Inject `IExampleApiService` into ViewModels and use methods. Handle exceptions and propagate meaningful messages to UI.

Verification
------------
- Build the app. Run against running API (local or deployed). Verify token is attached by inspecting requests or using a proxy.
