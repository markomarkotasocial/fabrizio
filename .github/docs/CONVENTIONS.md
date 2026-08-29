# Canonical Helpers & Conventions

Read this before writing backend or MAUI code. These helpers already exist — **use them, do not hand-roll the boilerplate they replace.** An agent that re-implements this logic inline reintroduces the duplication these were built to remove.

---

## Backend (`fabrizio.API` / `fabrizio.BLL` / `fabrizio.Repository`)

| Use | Instead of | Where |
|---|---|---|
| `class XController : AuthorizedControllerBase` + `if (!TryGetAccountId(out var accountId)) return Unauthorized();` | `User.FindFirstValue("accountId")` + `int.TryParse` guard | `fabrizio.API/Controllers/AuthorizedControllerBase.cs` |
| `return result.ToActionResult();` (generic → `200` + value, non-generic → `204`, failure → `ProblemDetails`) | `if (!result.IsSuccess) return result.ToProblem(); return Ok(result.Value);` | `fabrizio.API/Extensions/ResultExtensions.cs` |
| `class XRepository : RepositoryBase<X>, IXRepository { public XRepository(AppDbContext c) : base(c) {} /* only X-specific queries */ }` and `interface IXRepository : IRepository<X>` | re-declaring `Add` / `Delete` / `SaveChangesAsync` / `_context` in every repo | `fabrizio.Repository/RepositoryBase.cs` |
| `entity.ToDto()` (extension methods per aggregate) | inline `new XDto { Id = e.Id, ... }` in the service | `fabrizio.BLL/Mapping/*MappingExtensions.cs` |
| a private `LoadOwnedXAsync(accountId, id) → Result<X>` guard for the 404 / 403 / (cancelled) ladder | repeating `GetById → null? 404 → owner? 403` in every method | pattern: `fabrizio.BLL/TripService.Guards.cs` (`LoadOwnedTripAsync`) |

### Backend rules that follow from the helpers

- **BLL service constructors take only `I*Repository`** (and other BLL services). **Never inject `AppDbContext`** into `fabrizio.BLL`. If a service needs a query the repository doesn't have, add a method to the repository interface.
- **Request DTOs carry no validation attributes.** `Program.cs` sets `MvcOptions.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true`, so model binding is lenient and `fabrizio.BLL` is the single validator. A missing/blank field must produce a `Result.Fail(new BusinessError(...))`, not an ASP.NET `[Required]` error.
- **`Result` factory methods are `Success` / `Fail`** — `Result.Success()`, `Result<T>.Success(value)`, `Result.Fail(err)`, `Result<T>.Fail(err)`. `BusinessError` is a positional `record (Code, Message, HttpStatusCode)` in `fabrizio.Shared.Contracts`.
- **List endpoints:** service returns `Result<PagedResult<T>>.Success(...)`; the controller's `ToActionResult()` sends the bare `PagedResult<T>` on `200`.
- **Response DTOs are typed.** Return `LoginResponseDto`, not `new { Token = ... }`.
- **`IQueryable<T> QueryAll(...)` from a repository is an accepted read seam** — the BLL composes filtering/sorting/paging over it. This is deliberate (avoids a combinatorial explosion of repository overloads); it is the *only* place EF leaks upward. No `DbContext` / `DbSet` above the repository layer.
- Verify: `dotnet build fabrizio.API/fabrizio.API.csproj -c Release` (mirrors CI). No automated test project.

---

## MAUI (`fabrizio.App`)

| Use | Instead of | Where |
|---|---|---|
| `http.GetResultAsync<T>(url)` / `PostResultAsync<T>(url, body)` / `PostResultAsync(url, body)` / `PutResultAsync<T>` / `PutResultAsync` / `DeleteResultAsync(url)` — each returns `Result` / `Result<T>`, reads `ProblemDetails` on non-2xx, `network_error` on a transport exception | a per-method `try { ReadFromJsonAsync } catch { ... }` block | `fabrizio.App/Services/HttpResultExtensions.cs` |
| `_navigation.GoToApp()` / `_navigation.GoToLogin()` (injected `INavigationService`) | `Application.Current.MainPage = new AppShell()` / `new LoginPage()` | `fabrizio.App/Services/INavigationService.cs` |
| a client-side wrapper (`TripCardModel`) that exposes computed display members over a DTO | putting `DateRangeText` / `IsCurrent` / … on a `fabrizio.Shared` DTO | `fabrizio.App/ViewModels/TripCardModel.cs` |
| `WeakReferenceMessenger.Default.Send(new XChangedMessage())` after a mutation, and `Register<XChangedMessage>` in the list VM to reload on next appearance | hoping a list refreshes itself after create/edit/delete on another page | `fabrizio.App/Messages.cs` (`TripsChangedMessage`) |

### MAUI rules that follow from the helpers

- **Service method = one line** delegating to a `HttpResultExtensions` call. Keep the `ITripService` / `IProfileService` *interface* stable so ViewModels don't change.
- **Request bodies go out as `StringContent` with a `Content-Length`** (the helper's `AsJson` does this). Do not build `JsonContent` — the Android stack can drop a chunked request body.
- **Login-style calls use a standalone `HttpClient`** (see `AuthService`): no `TokenHandler`, no factory. A 401 there means bad credentials, not an expired session.
- **`fabrizio.Shared` DTOs stay pure data.** Presentation/computed members live on a client wrapper or the ViewModel.
- **ViewModels are in `namespace fabrizio.App.ViewModels`** (file under `ViewModels/`), inherit `BaseViewModel` (`_BaseViewModel.cs`: `IsBusy`, `EmptyMessage`, `HasError` — no `Title`).
- **Pages set `BindingContext` in the code-behind ctor** from the DI-injected ViewModel: `public XPage(XViewModel vm) { InitializeComponent(); BindingContext = vm; }`.
- **DI lifetimes:** `AddTransient` for Pages, ViewModels, `AppShell`; `AddSingleton` for `IAccountState`, `AuthService` (with `AddSingleton<IAuthService>(sp => sp.GetRequiredService<AuthService>())`), `INavigationService`; `AddHttpClient<TInterface, TImpl>(...).AddHttpMessageHandler<TokenHandler>()` for API services.
- Verify: `dotnet build fabrizio.App/fabrizio.App.csproj` (needs the `maui` workload; CI does not build MAUI). No automated test project — run the app.

---

## Configuration & secrets

- **Local secrets live in User Secrets, never in `appsettings*.json`.** `ConnectionStrings:DefaultConnection` and `Jwt:Key` are supplied via `dotnet user-secrets` locally and App Service settings (`Jwt__Key`, connection-strings blade) in Azure. Do not re-add them to committed files.
- EF migrations: agents MAY run `dotnet ef migrations add <Name> -p fabrizio.DAL -s fabrizio.API`; agents MUST NOT run `dotnet ef database update` (the developer applies migrations).
