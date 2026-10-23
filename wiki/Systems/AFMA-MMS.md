# AFMA MMS — Member Management System

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Repository** | `https://eclick-dev@dev.azure.com/eclick-dev/AFMA-MMS/_git/AFMA-MMS` |
| **Reviewed at** | `f0d6bc8` on `master`, 2026-09-14 |
| **Application layer** | **VERIFIED** from source |
| **Live status** | **GAP** — unknown whether this is deployed, piloted or in development |

AFMA's member management system: members, branches, contacts, committees, documents, distribution
groups, notification batches and training courses. Internally the domain is called **WIMS**.

> **Read this first.** `AFMA-MMS` is a **fork of the `AFMA-STAT` codebase**, committed as a single
> squashed commit with no shared git history. It contains its own copies of `AFMA.Stats.Client` and
> `AFMA.Stats.Admin` alongside the new `AFMA.Web.Admin`. Fixes made in `AFMA-STAT` since May 2026 are
> **not** present here. See [Risk Register](/Risk-Register) R8.

## Two critical issues before anyone works on this repository

1. **It does not build.** `nuget.config` clears all package sources and points at a local `Packages`
   folder that was never committed. Restore fails immediately with `NU1301`. It also needs
   `Alpha.Services.* 9.0.20`, which exists nowhere we can reach. See [Risk Register](/Risk-Register) R1.
2. **It contains a 58 MB backup of the live member database**, `afmawims_10072026.bak`, permanently in
   its git history. See [Risk Register](/Risk-Register) R2. **Do not clone or share this repository
   further until that is resolved.**

## The MMS application — `AFMA.Web.Admin`

The project that makes this repository different from `AFMA-STAT`.

| Controller | Module |
|---|---|
| `MemberController` | Members, branches, contacts, manufacturers, suppliers, services |
| `CommitteeController` | Committees and committee membership |
| `DistributionController` | Distribution groups and their members |
| `BatchController` | Notification batches — bulk email to members |
| `NotificationTemplateController` | Notification templates and types |
| `DocumentTemplateController` | Document templates |
| `DocumentTypeController` | Document type management |
| `CertificateController` | Member certificates |
| `TrainingCourseController` | Training courses |
| `ProfileController`, `Areas/Profile` | Member self-service profile |
| `MenuController` | Navigation |
| `ComingSoonController` | **Placeholder for unbuilt modules** |
| `HomeController` | Landing |

`ComingSoonController` is a direct signal that the module set is incomplete. Which modules it stands
in for is not recorded. *Owner: Henry. Due: 2026-09-30.*

## Domain model

**VERIFIED** 2026-09-14. The WIMS domain here is roughly twice the size of the copy in `AFMA-STAT`:
**35 entities and 35 mappings**, against 18 and 18.

Entities present here and absent from `AFMA-STAT`:

`Committee`, `CommitteeDocument`, `MemberCommittee`, `DocumentTemplate`, `DocumentType`,
`MemberDocument`, `Manufacturer`, `MemberBranchManufacturer`, `Supplier`, `MemberBranchSupplier`,
`MemberBranchService`, `MemberUser`, `MemberUserBranch`, `MemberUserDistributionGroup`,
`NotificationContact`, `NotificationTemplate`, `NotificationType`

Also added here: `ExcelExportService` / `IExcelExportService`, `MaxFileSizeAttribute`, an
`Application/.../Model/Export` model set, and `DeleteViewModel`.

That shape — members, branches, the organisations that supply and manufacture for them, the committees
they sit on, the documents they hold, and the groups they are mailed as — is a reasonable foundation
for the member-management environment the account brief identifies as an opportunity.

## Technical stack

Identical to [AFMA STATS](/Systems/AFMA-STATS) except:

| Difference | Detail |
|---|---|
| Private packages | `Alpha.Services.* 9.0.20` (STAT uses 9.0.18) — **not available anywhere** |
| Additional packages | `Microsoft.AspNetCore.Identity.UI 8.0.13`, `Microsoft.EntityFrameworkCore.Sqlite 8.0.11`, EF Core 8.0.13 alongside the existing EF Core 6 references |
| Roles | `Admin`, `Member`, `User` — `Member` is new here |
| Maintenance mode | **Not registered.** `AFMA.Web.Admin/Program.cs` does not add the maintenance middleware, so the June 2026 maintenance-mode capability does not apply to MMS |
| CAPTCHA | Not registered on `AFMA.Web.Admin` |
| Caching and Serilog | Explicitly registered via `ServiceActions.Caching` and `ServiceActions.Serilog` |

The EF Core Sqlite reference is unexplained — nothing else in the account suggests a Sqlite database.
Likely scaffolding from the Identity templates. *Owner: Henry to confirm and remove. Due: 2026-09-30.*

## Configuration

**Contains live production secrets.** See [Risk Register](/Risk-Register) R3.

| Setting | Value |
|---|---|
| `ConnectionStrings:WimsApplicationRepository` | `Server=WIN-MDOU42447UT; Database=afmawims; User Id=afmawims_app; Password=<redacted>` |
| `ConnectionStrings:ApplicationRepository` | `Server=(local); Database=afmastats; Integrated Security=true` |
| `SMTPSettings:Host` / `Port` | `smtp.mailgun.org` / `587` |
| `SMTPSettings:Username` | `mailadmin@afma.eclick.co.za` |
| `SMTPSettings:FromAddress` | `system@afmastats.co.za` |
| `SMTPSettings:Auth` / `Debug` | `true` / `false` |
| `WebApplicationSettings:EmailApiKey` | `<redacted>` — Resolve email portal |
| `WebApplicationSettings:Deployment` | `"Not Deployed"` |
| `EPPlus:ExcelPackage:LicenseContext` | `NonCommercial` — see R7 |

Two things stand out. `SMTPSettings:Auth` is `true` and `Debug` is `false` here, where `AFMA-STAT`
has the reverse — this configuration looks **production-shaped**, which makes the committed Mailgun
password a live credential rather than a development leftover. And `Deployment` still reads
`"Not Deployed"`, so the file's own marker contradicts that. Resolve which is true before assuming
either. *Owner: Ethan. Due: 2026-09-19.*

## Database

`afmawims` on `WIN-MDOU42447UT`, accessed by SQL login `afmawims_app` via NHibernate.

**GAP** — no schema in source control, no migrations, no data dictionary. The only schema artefact in
existence is the `.bak` file that should not be there. See [Risk Register](/Risk-Register) R11.

The stock ASP.NET Identity EF migrations under `Website/AFMA.Web.Admin/Data/Migrations/` relate only
to Identity tables, not the business schema.

## Change history

**VERIFIED** 2026-09-14. One commit.

| Commit | Date | Author | Change |
|---|---|---|---|
| `f0d6bc8` | 2026-08-26 | Marnus Badenhorst | Initial AFMA MMS migration |

A single squashed commit means there is **no development history for this system at all** — no record
of what was built when, why a decision was taken, or what was tried and abandoned. Everything about
its evolution lives only in the author's memory. This is the clearest illustration on the account of
the dependency the handover brief is trying to remove.

## Opportunity assessment

The account brief identifies AFMA's member-management environment as an area of significant potential.
On the evidence of the code:

**What is genuinely there.** A substantially built member domain — 35 entities covering members,
branches, contacts, committees, documents, suppliers, manufacturers, distribution groups and
notification templates, with working admin screens (407 Razor views) and an Excel export service.
This is well past prototype.

**What stands between it and a sellable product.**

1. It does not build (R1) — nothing can proceed until this is fixed.
2. The member database is exposed in git history (R2) — this must be closed before the system is
   promoted to anyone.
3. `ComingSoonController` marks unbuilt modules that nobody has enumerated.
4. It is a fork, so it inherits every `AFMA-STAT` fix only by hand (R8).
5. Maintenance mode, added to STATS in June 2026, is absent here — MMS has no way to be taken offline
   cleanly.
6. Live status is unknown — nobody has yet established whether members are using this.

**Recommended sequence:** fix R1, close R2, then establish live status and the `ComingSoon` gap list.
Only then is a conversation with AFMA about the member-management opportunity built on something
solid. Attempting the commercial conversation first risks promising on a system we cannot currently
compile. *Owner: Ethan, with Henry from 2026-09-21.*
