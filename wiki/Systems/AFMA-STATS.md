# AFMA STATS

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Repository** | `https://eclick-dev@dev.azure.com/eclick-dev/AFMA-STAT/_git/AFMA-STAT` |
| **Reviewed at** | `895ebb7` on `master`, 2026-09-14 |
| **Application layer** | **VERIFIED** from source |
| **Hosting** | **GAP** — see [Hosting and Infrastructure](/Operations/Hosting-and-Infrastructure) |

The AFMA statistics platform. Members submit periodic production and safety figures; AFMA staff
review, report on and publish aggregated industry statistics.

## Applications in the solution

`AFMA Stats.sln` contains 13 projects across five solution folders.

| Project | Type | Purpose |
|---|---|---|
| `AFMA.Stats.Client` | ASP.NET Core Web (net8.0) | **Member-facing portal.** Where members log in and submit figures. |
| `AFMA.Stats.Admin` | ASP.NET Core Web (net8.0) | **Internal admin and reporting.** Where AFMA staff review submissions and produce reports. |
| `AFMA.ApplicationLayer` | Library (net8.0) | Business logic, one layer class per module |
| `AFMA.ApplicationLayer.Templating` | Razor class library (net8.0) | Email templates |
| `AFMA.AutoMapping` | Library (net8.0) | AutoMapper profiles |
| `AFMA.Data.Entities` | Library (net8.0) | 70 statistics domain entities |
| `AFMA.Data.Mappings` | Library (net8.0) | 68 FluentNHibernate mappings |
| `AFMA.Data.Entities.Wims` | Library (net8.0) | 18 member (WIMS) entities |
| `AFMA.Data.Mappings.Wims` | Library (net8.0) | 18 WIMS mappings |
| `AFMA.Infrastructure` (`Shared/AFMA.Shared.Infrastructure`) | Library (net8.0) | Session factories, settings classes, commands, queries |
| `AFMA.Shared.Services` | Library (net8.0) | Service registration, maintenance-mode middleware |
| `AFMA.Service.Communication` | Library (net8.0) | WCF/SOAP client for the Resolve email portal |
| `Infrastructure/AFMA.Infrastructure` | Library (**net6.0**) | **Orphaned.** Not in the solution, not referenced. Duplicates the assembly name above. Delete. See [Risk Register](/Risk-Register) R6. |

## Functional modules

Derived from controllers and areas, **VERIFIED** 2026-09-14.

### Member portal — `AFMA.Stats.Client`

| Controller | Module |
|---|---|
| `FeedsalesController` | Feed sales submissions — finished product and raw material |
| `SalmonellaController` | Salmonella test result submissions |
| `MycotoxinController` | Mycotoxin test result submissions |
| `IndustryProfileController` | Industry profile submissions — costs, fleet, production, staffing |
| `AccountController`, `Areas/Identity` | Login, registration, password reset |
| `ExternalController` | External/anonymous views |
| `HomeController`, `ErrorController` | Landing and error handling |

### Admin — `AFMA.Stats.Admin`

| Controller / Area | Module |
|---|---|
| `FeedsalesReportController`, `Areas/Feedsales` | Feed sales reporting |
| `SalmonellaReportController`, `Areas/Salmonella` | Salmonella reporting |
| `MycotoxinReportController`, `Areas/Mycotoxin` | Mycotoxin reporting |
| `IndustryProfileReportController`, `Areas/IndustryProfile` | Industry profile reporting |
| `FinishedProductController` | Finished product management |
| `MemberController` | Member administration |
| `AuditController` | Audit trail |
| `Areas/General` | General lookups and administration |
| `ExternalController` | **Anonymous** feed-sales and raw-material dashboards. See [Risk Register](/Risk-Register) R10 |

**GAP** — the business rules behind each module are not documented: submission periods and deadlines,
validation rules, how figures are aggregated, who may reopen a closed period. This is the knowledge
most at risk of being lost and it exists nowhere in writing. *Owner: Ethan with Henry. Due: 2026-10-10.*

## Technical stack

**VERIFIED** 2026-09-14.

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC, .NET 8 (one orphaned net6.0 project) |
| ORM | **NHibernate 5.4.6** with FluentNHibernate 3.3.0 — the primary data access |
| Secondary ORM | Entity Framework Core 6.0.x — used for ASP.NET Identity |
| Identity | ASP.NET Core Identity via `NHibernate.AspNetCore.Identity 7.0.12` |
| Mapping | AutoMapper 12.0.1 |
| Logging | Serilog 3.1.1 → SQL Server `Logs` table, auto-created, minimum level `Warning` |
| Excel | EPPlus 6.2.10 — **non-commercial licence declared**, see [Risk Register](/Risk-Register) R7 |
| Paging | X.PagedList.Mvc.Core 8.4.7 |
| UI notifications | AspNetCoreHero.ToastNotification (Notyf) 1.1.0, SweetAlert |
| CAPTCHA | DNTCaptcha.Core 4.9.1 |
| API docs | Swashbuckle.AspNetCore 6.5.0 |
| Private packages | `Alpha.Services.* 9.0.18`, `Resolvetech.NetCore.Common 1.0.3.1004` — see [Risk Register](/Risk-Register) R1 |

Views use Razor with **runtime compilation enabled in production configuration**
(`AddRazorRuntimeCompilation`). That means `.cshtml` changes take effect without a redeploy — which is
convenient and is also a route by which the production site can drift from source control. Worth a
deliberate decision rather than leaving as-is.

## Authentication and authorisation

**VERIFIED** 2026-09-14.

- ASP.NET Core Identity, cookie authentication, backed by NHibernate.
- Login path `/Identity/Account/Login`, logout `/Identity/Account/Logout`.
- Cookie expiry **180 minutes**, sliding. Session idle timeout **3 hours**.
- Two roles in `AFMA-STAT`: `Admin` and `User`.
- `AllowAnonymous` appears on: error pages, the Identity forgot/reset password flow (expected), and
  `ExternalController` (see [Risk Register](/Risk-Register) R10).

**GAP** — the role model is not documented: what `Admin` and `User` may each do, how a member user is
created and linked to a member organisation, who grants `Admin`. *Owner: Henry. Due: 2026-09-30.*

## Configuration

Settings read from `appsettings.json`. **These files currently contain live secrets** — see
[Risk Register](/Risk-Register) R3.

| Section | Key settings |
|---|---|
| `ConnectionStrings` | `ApplicationRepository` → `afmastats`; `WimsApplicationRepository` → `afmaweb` (Admin only) |
| `ApplicationSettings` | `Deployment`, `BaseUrl`, `ClientUrl`, `PageSize` (25), `CachMinutes` (1440), `Version`, `TestSite` |
| `SMTPSettings` | `Host`, `Port`, `Username`, `Password`, `FromName` (AFMA), `FromAddress` (`system@afmastats.co.za`), `Auth`, `Debug`, `DebugEmail`, `SubmissionStatsEmail` (`stats@afma.co.za`) |
| `Serilog` | MSSqlServer sink, `Logs` table, `autoCreateSqlTable: true`, minimum level `Warning` |
| `MaintenanceSettings` | See [Maintenance Mode](/Operations/Maintenance-Mode) |
| `EPPlus` | `LicenseContext: NonCommercial` — see R7 |
| `AllowedHosts` | `*` — no host filtering. Tighten to the real hostnames. |

Note the committed values are **development defaults** (`Server=(local)`, `BaseUrl: https://localhost:44365`,
`Deployment: "Not Deployed"`, `Version: "1.0.XXX"`). Production values are applied on the server by a
means that is not documented. The `Version` placeholder means **the running application cannot report
which build it is** — worth fixing as part of the deployment work in R4.

## Known defects

| Defect | Where | Detail |
|---|---|---|
| Null reference on cache miss | `ExternalApiService.RefreshFeedsalesDashboardDataAsync` | [Risk Register](/Risk-Register) R13 |
| Raw material view serves finished-product data | `ExternalController.RawMaterial()` | [Risk Register](/Risk-Register) R13 |
| Malformed connection strings | `appsettings.json` | Both `Integrated Security=true` and an empty `User ID` with a password. See R3 |

## Recent change history

**VERIFIED** 2026-09-14. Seven commits total.

| Commit | Date | Author | Change |
|---|---|---|---|
| `895ebb7` | 2026-07-06 | Marnus Badenhorst | Custom decimal model binder accepting comma and dot for feed sales and raw materials |
| `ca5f61a` | 2026-07-06 | Marnus Badenhorst | Decimal parsing accepts comma and dot as separators |
| `b021c1e` | 2026-06-24 | Chris Delport | Populate `.gitignore` with backup and publish rules |
| `bb9c3ed` | 2026-06-24 | Chris Delport | Add `.gitignore` for db backups and publish output |
| `a18fcab` | 2026-06-24 | Chris Delport | Client-supplied offline message, multi-paragraph body with mailto links |
| `5af0d88` | 2026-06-24 | Chris Delport | Configurable maintenance-mode middleware with bypass token |
| `1b81644` | 2026-05-12 | Marnus Badenhorst | Initial commit |

The decimal-separator work is worth understanding: South African locale input uses a comma decimal
separator, and members were evidently entering figures both ways. `InvariantDecimalModelBinderProvider`
is registered first in the model binder chain in `AFMA.Stats.Client/Program.cs` to accept both. A
regression here would silently corrupt submitted industry figures, so it belongs in the first set of
characterisation tests (see [Risk Register](/Risk-Register) R9).

## Hosting

**GAP** — entirely unknown. Committed configuration names SQL Server `WIN-MDOU42447UT`, and the
application is IIS-hosted ASP.NET Core, but nothing else is established: which server, which site
bindings, which app pools, what the public hostnames are, where logs go, or who administers it.
*Owner: Ethan. Due: 2026-09-19.* See [Hosting and Infrastructure](/Operations/Hosting-and-Infrastructure).
