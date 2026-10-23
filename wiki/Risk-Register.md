# Risk Register

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Last reviewed** | 2026-09-14 |
| **Scope of this review** | `AFMA-STAT` at commit `895ebb7`, `AFMA-MMS` at commit `f0d6bc8`. Application code and configuration only. No infrastructure has been reviewed. |

Severity is judged on consequence to AFMA, not on effort to fix.

| ID | Risk | Severity | Status |
|---|---|---|---|
| R1 | Neither application can be built from a clean clone | **Critical** | Open |
| R2 | Member database backup committed permanently to git history | **Critical** | Open |
| R3 | Live production credentials in plain text in both repositories | **Critical** | Open |
| R4 | No automated deployment; release depends on one developer's machine | **High** | Open |
| R5 | Member password-reset emails blind-copied to a third party | **High** | Open |
| R6 | Dependencies with known high-severity vulnerabilities | **High** | Open |
| R7 | EPPlus used under a non-commercial licence in a commercial deployment | **High** | Open |
| R8 | Two divergent forks of the same codebase with no shared history | **High** | Open |
| R9 | No test coverage across ~96,000 lines of C# | **Medium** | Open |
| R10 | Unauthenticated endpoint serving industry feed-sales data | **Medium** | Open — needs confirmation |
| R11 | No database schema source control or migration history | **Medium** | Open |
| R12 | Repository hygiene: build artefacts and IDE state committed | **Low** | Open |
| R13 | Latent null-reference defect in the feed-sales dashboard refresh | **Low** | Open |

---

## R1 — Neither application can be built from a clean clone

**Severity: Critical.** This is the single largest continuity risk on the account. If the current
developer machines were lost tomorrow, AFMA's two applications could not be rebuilt.

**Verified 2026-09-14** by cloning both repositories fresh and running `dotnet restore` on each.

**`AFMA-STAT`** fails with:

```
error NU1101: Unable to find package Resolvetech.NetCore.Common.
No packages exist with this id in source(s): AfmaLocalPackages,
Microsoft Visual Studio Offline Packages, nuget.org
```

`Resolvetech.NetCore.Common 1.0.3.1004` is referenced by five projects. It is not on nuget.org, it is
not in the repository's vendored `Packages/` folder, and no private feed is configured in
`NuGet.config`. It exists only on machines that already have it cached.

**`AFMA-MMS`** fails earlier and harder:

```
error NU1301: The local source '...\AFMA-MMS\Packages' doesn't exist.
```

`AFMA-MMS/nuget.config` clears all package sources and declares a local `Packages` folder that was
never committed. The repository has no `Packages` directory at all. `AFMA-MMS` additionally needs
`Alpha.Services.* 9.0.20`, which exists in no accessible source — `AFMA-STAT` vendors only `9.0.18`.

**The private dependencies.** Both applications depend on two package families that are not public:

| Package family | Versions needed | Where it is |
|---|---|---|
| `Alpha.Services.*` (Core, Extensions, Extensions.Mvc, Models, NHibernate.Repository, Notifications, Notifications.Templates) | `9.0.18` for STAT, `9.0.20` for MMS | `9.0.18` vendored in `AFMA-STAT/Packages/`. **`9.0.20` is missing entirely.** |
| `Resolvetech.NetCore.Common` | `1.0.3.1004` | **Missing entirely.** |

`Alpha.Services.Core.nuspec` names its author as **JJ Swart**. These packages carry the application's
data access, notification and service-composition layers — they are not peripheral.

**Actions.**

1. Obtain the `.nupkg` files for `Alpha.Services.* 9.0.20` and `Resolvetech.NetCore.Common 1.0.3.1004`.
   *Owner: Ethan, via Chris. Due: 2026-09-19.*
2. Establish whether we have, or can obtain, the **source** for these packages, and on what licence
   terms. A binary we cannot rebuild is a dependency we do not control. *Owner: Ethan. Due: 2026-09-22.*
3. Stand up an Azure Artifacts feed in the `eclick-dev` organisation, publish both package families to
   it, then repoint both `NuGet.config` files at it. *Owner: Henry. Due: 2026-09-30.*
4. Prove the fix by having someone who has never built these applications clone and build both from
   scratch on a clean machine. *Owner: Henry. Due: 2026-09-30.*

---

## R2 — Member database backup committed permanently to git history

**Severity: Critical.**

**Verified 2026-09-14.** `AFMA-MMS/afmawims_10072026.bak` is a 58 MB (60,914,176 byte) Microsoft SQL
Server native backup of the `afmawims` database — the member management database. Its file header
confirms the `Microsoft SQL Server` native backup format.

It was committed in `f0d6bc8` ("Initial AFMA MMS migration", Marnus Badenhorst, 2026-08-26), which is
the only commit on `master`. **Deleting the file will not remove it.** It is in the commit history and
will be present in every clone of the repository, permanently, unless the history is rewritten.

This is AFMA member data. Whoever can read the repository can restore the database.

Note that `AFMA-STAT/.gitignore` explicitly forbids exactly this:

```
# Database backups - never commit
_bk/
*.bak
*.trn
```

`AFMA-MMS` uses the stock Visual Studio `.gitignore` instead, which does not exclude `.bak`.

**Actions.**

1. Establish exactly who has, and has ever had, read access to the `AFMA-MMS` repository. Treat the
   contents of that backup as disclosed to all of them. *Owner: Ethan. Due: 2026-09-16.*
2. Decide with AFMA whether this is a reportable personal-information incident. The database holds
   member contact records; POPIA obligations must be assessed rather than assumed away.
   *Owner: Ethan, with Pieter. Due: 2026-09-19.*
3. Rewrite `AFMA-MMS` history to purge the file, force-push, and have every clone re-cloned.
   Coordinate so no work is lost. *Owner: Henry. Due: 2026-09-26.*
4. Copy `AFMA-STAT`'s `.gitignore` backup rules into `AFMA-MMS` and add a server-side push size limit.
   *Owner: Henry. Due: 2026-09-26.*
5. Record where database backups *should* live — see [Backup and Recovery](/Operations/Backup-and-Recovery).

---

## R3 — Live production credentials in plain text in both repositories

**Severity: Critical.**

**Verified 2026-09-14.** Committed `appsettings.json` files contain working secrets. They are not
placeholders. *(Values are deliberately not reproduced in this wiki.)*

| Secret | File | What it opens |
|---|---|---|
| SQL password for user `afmawims_app` on server `WIN-MDOU42447UT`, database `afmawims` | `AFMA-MMS/Website/AFMA.Web.Admin/appsettings.json` | The live member database |
| Mailgun SMTP password for `mailadmin@afma.eclick.co.za` on `smtp.mailgun.org:587` | `AFMA-MMS/Website/AFMA.Web.Admin/appsettings.json` | Ability to send mail as AFMA |
| `WebApplicationSettings:EmailApiKey` | `AFMA-MMS/Website/AFMA.Web.Admin/appsettings.json` | The Resolve email portal API |
| `MaintenanceSettings:BypassToken` | `AFMA-STAT` Admin and Client `appsettings.json` | Bypasses maintenance mode on the live site |
| SMTP password (Stats portal) | `AFMA-STAT` Admin and Client `appsettings.json` | Outbound mail as the STATS portal |

The production database server hostname `WIN-MDOU42447UT` is also disclosed. It is a default Windows
machine name, which suggests a server that was never formally named or provisioned to a standard.

Two connection strings are additionally malformed — they set `Integrated Security=true` **and** supply
a `User ID`/`Password` pair, with the `User ID` left empty. Whichever wins, the intent is unclear and
the credential is redundant. This needs resolving rather than leaving ambiguous.

**Actions.**

1. Rotate every secret listed above. Rotate before purging history, not after — the values are already
   exposed and rotation is what actually protects AFMA. *Owner: Henry. Due: 2026-09-19.*
2. Move secrets out of `appsettings.json` entirely. For IIS-hosted ASP.NET Core the practical options
   are environment variables on the app pool, a `secrets.json` outside the repository, or Azure Key
   Vault if we are willing to take the dependency. Decide one and apply it to both applications.
   *Owner: Henry, decision with Ethan. Due: 2026-09-26.*
3. Commit sanitised `appsettings.json` files carrying empty placeholders, and document every required
   setting on the relevant system page. *Owner: Henry. Due: 2026-09-26.*
4. Enable secret scanning on both repositories — Advanced Security is available in this Azure DevOps
   organisation. *Owner: Ethan. Due: 2026-09-26.*

---

## R4 — No automated deployment; release depends on one developer's machine

**Severity: High.**

**Verified 2026-09-14.** There are no Azure Pipelines definitions, no YAML build files and no release
definitions in either repository. The only deployment artefacts are two Visual Studio folder publish
profiles in `AFMA-STAT/Website/AFMA.Stats.Admin/Properties/PublishProfiles/`:

- `FolderProfile.pubxml` — targets `net6.0`, publishes to `D:\Development\Published\AFMA.Admin`
- `FolderProfile1.pubxml` — targets `net8.0`, publishes to the same folder

Both publish to a **local folder on a developer's machine**. How the contents of that folder reach the
production server is not recorded anywhere. `FolderProfile1.pubxml.user` records a single publish on
2025-08-11; `FolderProfile.pubxml.user` records three in early 2025. These are local developer
artefacts and are not a reliable deployment record.

There are **no publish profiles at all** for `AFMA.Stats.Client` (the member-facing portal) or for any
`AFMA-MMS` project. How those are deployed is unknown.

**Actions.**

1. Document the actual current deployment procedure, step by step, from Chris — including how files
   reach the server and who can do it. *Owner: Ethan, from Chris. Due: 2026-09-19.*
2. Build Azure Pipelines for both solutions once R1 is resolved and packages restore.
   *Owner: Henry. Due: 2026-10-10.*
3. Record it on [Deployment Process](/Development/Deployment-Process) as we learn it, not afterwards.

---

## R5 — Member password-reset emails blind-copied to a third party

**Severity: High.**

**Verified 2026-09-14.** The address `operations@resolvetech.co.za` is hard-coded as a BCC recipient
in live code paths:

| File | Line | Path |
|---|---|---|
| `AFMA-STAT/Website/AFMA.Stats.Client/Areas/Identity/Pages/Account/ForgotPassword.cshtml.cs` | 89 | Member password reset |
| `AFMA-STAT/Website/AFMA.Stats.Admin/Areas/Identity/Pages/Account/ForgotPassword.cshtml.cs` | 87 | Admin password reset |
| `AFMA-STAT/Application/AFMA.ApplicationLayer/Services/NotificationService.cs` | 107, 158, 179, 227 | Member notifications |
| `AFMA-STAT/Application/AFMA.ApplicationLayer/MycotoxinLayer.cs` | 409 | Mycotoxin submission notifications |

Password-reset emails contain reset links. A copy of every AFMA member and administrator password
reset is being sent to an address outside AFMA and outside us.

This may well be a deliberate operational monitoring arrangement from an earlier era. It is recorded
here as a risk because **whether AFMA knows about it and consented to it is unknown**. That question
must be answered before any conclusion is drawn.

Two related hard-coded addresses appear in the same codebase: `jacques@di.co.za` and
`jacques@greengecko.co.za`. The second sits inside `EmailService.ProcessAndSendBatch`, which
**overwrites whatever recipients, subject and body it is given** with test values addressed to that
person. If that method is ever called in production it will send a test email to an external address
instead of the intended message. It appears to be abandoned scaffolding, but it is live code.

**Actions.**

1. Ask Chris what `operations@resolvetech.co.za` was for and whether AFMA agreed to it.
   *Owner: Ethan. Due: 2026-09-19.*
2. If it is not a consented arrangement, remove it and tell AFMA. If it is, make it configuration
   rather than a hard-coded constant, and document it. *Owner: Henry. Due: 2026-09-26.*
3. Delete or gut `EmailService.ProcessAndSendBatch`. *Owner: Henry. Due: 2026-09-26.*

---

## R6 — Dependencies with known high-severity vulnerabilities

**Severity: High.**

**Verified 2026-09-14**, reported by NuGet during restore of both solutions:

| Package | Version | Advisory |
|---|---|---|
| `NHibernate` | 5.4.6 | [GHSA-fg4q-ccq8-3r5q](https://github.com/advisories/GHSA-fg4q-ccq8-3r5q) — high severity |
| `AutoMapper` | 12.0.1 | [GHSA-rvv3-g6hj-g44x](https://github.com/advisories/GHSA-rvv3-g6hj-g44x) — high severity |

NHibernate is the primary ORM for both applications, so this is not an incidental dependency.

Additional dependency concerns, lower individually but compounding:

- `System.Data.SqlClient 4.8.6` — the legacy client, superseded by `Microsoft.Data.SqlClient`.
- Entity Framework Core 6 and 8 packages are referenced side by side in the same solution, alongside
  NHibernate. Two ORMs and two EF major versions in one application.
- `AFMA-STAT` still contains a project targeting `net6.0` (`Infrastructure/AFMA.Infrastructure`).
  .NET 6 is out of support. That project is **orphaned** — it is referenced by neither the solution nor
  any other project, and duplicates the assembly name of the live `Shared/AFMA.Shared.Infrastructure`
  project. It should be deleted.
- Several `System.ServiceModel.*` references use a floating `8.*` version, so the build is not
  reproducible.

**Actions.**

1. Upgrade NHibernate and AutoMapper past the advisories. Both are load-bearing; this needs testing,
   which we do not currently have — see R9. *Owner: Henry. Due: 2026-10-17.*
2. Delete the orphaned `Infrastructure/AFMA.Infrastructure` project. *Owner: Henry. Due: 2026-09-26.*
3. Pin the floating `8.*` versions. *Owner: Henry. Due: 2026-09-26.*

---

## R7 — EPPlus used under a non-commercial licence in a commercial deployment

**Severity: High.** This is a licensing exposure, not a technical one.

**Verified 2026-09-14.** Both applications reference `EPPlus 6.2.10` and configure it as:

```json
"EPPlus": { "ExcelPackage": { "LicenseContext": "NonCommercial" } }
```

Present in `AFMA-STAT` Admin and Client, and in `AFMA-MMS` Web.Admin `appsettings.json`.

EPPlus 5 and later is dual-licensed under the Polyform Noncommercial licence. Declaring
`NonCommercial` in software supplied commercially to a paying client is a licence assertion that does
not match the use. EPPlus generates the Excel exports in the STATS reporting and MMS member export
features, so it cannot be removed without replacing that functionality.

**Actions.**

1. Confirm whether a commercial EPPlus licence has been purchased and, if so, where it is recorded.
   *Owner: Ethan. Due: 2026-09-25.*
2. If not, decide between buying a commercial licence and migrating to a permissively licensed
   alternative (ClosedXML, or `DocumentFormat.OpenXml` directly). Cost and effort both belong in that
   decision. *Owner: Ethan, with Pieter. Due: 2026-10-03.*
3. Record the outcome on [Licences and Subscriptions](/Operations/Licences-and-Subscriptions).

---

## R8 — Two divergent forks of the same codebase with no shared history

**Severity: High.**

**Verified 2026-09-14.** `AFMA-MMS` is a copy of the `AFMA-STAT` codebase. Both contain a solution
named `AFMA Stats.sln` with **identical project GUIDs**. `AFMA-MMS` was committed as a single squashed
commit, so the two repositories share **no common ancestor**. Git cannot merge or rebase between them.

They have since diverged:

| | `AFMA-STAT` | `AFMA-MMS` |
|---|---|---|
| Commits | 7 | 1 |
| Branches | `master`, `develop` (identical) | `master` |
| WIMS entities | 18 | 35 |
| WIMS mappings | 18 | 35 |
| C# files / lines | 441 / 44,812 | 532 / 51,614 |
| Razor views | 282 | 407 |
| Extra web project | — | `AFMA.Web.Admin` |

Shared code — the application layer, data entities, mappings, NHibernate infrastructure, the
communication service — now exists in two copies that drift independently. The decimal-separator fix
made in `AFMA-STAT` (`ca5f61a`, `895ebb7`, July 2026) and the maintenance-mode work (`5af0d88`,
`a18fcab`, June 2026) are **not** in `AFMA-MMS`. Nor are `AFMA-MMS`'s WIMS improvements in `AFMA-STAT`.

Every future fix to shared code must be applied twice, by hand, or the two will diverge further.

**Actions.**

1. Decide the intended end state: one solution with two deployable web applications, or two genuinely
   separate products. This is an architectural decision with commercial consequences and should not be
   made by default. *Owner: Ethan, with Henry and Chris. Due: 2026-10-03.*
2. Until that decision lands, maintain a written list of divergences so nobody assumes a fix in one is
   present in the other. *Owner: Henry. Due: 2026-09-26.*

---

## R9 — No test coverage

**Severity: Medium**, rising to High the moment we attempt the upgrades in R6.

**Verified 2026-09-14.** There are zero test projects in either solution — no unit tests, no
integration tests, nothing. Across 973 C# files and roughly 96,000 lines.

Every change to these applications is currently validated by someone clicking through the site.

**Action.** Before attempting the NHibernate and AutoMapper upgrades, build characterisation tests
around the submission and calculation paths for Feed Sales and Salmonella — the places where a silent
numeric regression would damage AFMA's published industry statistics.
*Owner: Henry. Due: agree scope by 2026-10-03.*

---

## R10 — Unauthenticated endpoint serving industry feed-sales data

**Severity: Medium — needs confirmation before it is treated as a defect.**

**Verified 2026-09-14.** `AFMA-STAT/Website/AFMA.Stats.Admin/Controllers/ExternalController.cs` is
decorated `[AllowAnonymous]` and exposes two actions, `Feedsales()` and `RawMaterial()`, both returning
aggregated feed-sales dashboard data with no authentication.

This may be entirely intentional — a public industry dashboard, or a data source for an external
consumer. It is flagged because nothing in the codebase says which, and it sits on the **Admin**
application rather than the public one.

**Actions.**

1. Establish from Chris what consumes `/external/feedsales` and `/external/rawmaterial`, and whether
   public access is intended. This is also the most likely candidate for how Power BI is fed — see
   [Integrations and Data Flows](/Operations/Integrations-and-Data-Flows). *Owner: Ethan. Due: 2026-09-19.*
2. If it is intentional, document it as a supported interface. If not, put authentication on it.
   *Owner: Henry. Due: 2026-09-26.*

---

## R11 — No database schema source control or migration history

**Severity: Medium.**

**Verified 2026-09-14.** Neither repository contains a single `.sql` file. There is no schema project,
no migration scripts and no change history for the `afmastats` or `afmawims` databases.

The only migrations present are `AFMA-MMS/Website/AFMA.Web.Admin/Data/Migrations/`, which contain the
stock ASP.NET Identity schema and are unrelated to the business schema.

The schema exists only in the production databases and, implicitly, in 70 entity classes and 68
FluentNHibernate mapping classes. There is no way to recreate an empty AFMA database from source, and
no record of how the schema has changed over time.

The 58 MB `.bak` in `AFMA-MMS` (R2) appears to be the informal substitute for this — which is
precisely how a member database ended up in a git repository.

**Actions.**

1. Script both production schemas (structure only, no data) and commit them. *Owner: Henry. Due: 2026-09-30.*
2. Adopt a migration approach — a SQL Server database project, DbUp, or EF migrations — and use it for
   all future schema change. *Owner: Henry, decision with Ethan. Due: 2026-10-10.*

---

## R12 — Repository hygiene

**Severity: Low**, but it is what allowed R2 and R3.

**Verified 2026-09-14.**

- `AFMA-STAT` has **18 files under `.vs/` committed**, including Visual Studio Copilot semantic indices
  (`CodeChunks.db`, `SemanticSymbols.db`), file-content indices and
  `.vs/AFMA Stats/config/applicationhost.config`. `AFMA-STAT/.gitignore` does not exclude `.vs/`.
- `.pubxml.user` files are committed. These are per-developer files.
- Repository sizes are inflated: `AFMA-MMS` carries 175 MB of website content plus a 124 MB `.git`
  directory; `AFMA-STAT` 87 MB plus 40 MB.
- `AFMA-STAT/Packages/` vendors binary `.nupkg` files into source control — a workaround for the
  missing package feed in R1.

**Action.** Replace `AFMA-MMS`'s stock `.gitignore` with a merged version carrying `AFMA-STAT`'s backup
rules; add `.vs/` and `*.pubxml.user` to `AFMA-STAT`'s; remove the tracked `.vs/` files. Do this as
part of the R2 history rewrite so the repository is cleaned once. *Owner: Henry. Due: 2026-09-26.*

---

## R13 — Latent null-reference defect in feed-sales dashboard refresh

**Severity: Low** — dormant unless the method is called.

**Verified 2026-09-14.** In
`AFMA-STAT/Application/AFMA.ApplicationLayer/Services/ExternalApiService.cs`, lines 45-73,
`RefreshFeedsalesDashboardDataAsync` does this:

```csharp
if (!cache.TryGetValue(cacheKey, out FeedsalesApiDashboardModel dashboardModel))
{
    var result = await externalApiLayer.GetFeedsalesReportDataAsync().ConfigureAwait(false);
    dashboardModel.Data = result.Data.Where(...).ToList();   // dashboardModel is null here
```

On a cache miss `dashboardModel` is `null`, and the next line dereferences it. The method throws
`NullReferenceException` on exactly the path it exists to handle. The fetched `result` is never
assigned to `dashboardModel`.

The same file's `GetFeedsalesDashboardDataAsync` does it correctly, which is what makes this look like
a copy-paste slip.

Separately, `ExternalController.RawMaterial()` calls `GetFeedsalesDashboardDataAsync()` — the same
method as `Feedsales()` — so the raw-material view is served finished-product data. This may be
intentional given the model carries both, but it reads as a second copy-paste slip.

**Action.** Fix both when the file is next touched. Neither warrants a release on its own.
*Owner: Henry.*
