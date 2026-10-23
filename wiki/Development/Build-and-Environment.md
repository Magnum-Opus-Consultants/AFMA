# Build and Environment

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Status** | **VERIFIED** 2026-09-14 — the verdict is that neither application builds |

## Verdict

**Neither AFMA application can be built from a clean clone.** Both fail at dependency restore, before
compilation is even attempted, on private packages that are not available from any source we hold.

This was established by cloning both repositories fresh and running `dotnet restore`. It is not an
inference from reading configuration.

This is [Risk Register](/Risk-Register) **R1** and it is the highest-priority item on the account. A
system nobody can build is a system nobody can fix, patch or hand over.

## Evidence

### `AFMA-STAT`

```
$ git clone https://eclick-dev@dev.azure.com/eclick-dev/AFMA-STAT/_git/AFMA-STAT
$ cd AFMA-STAT && dotnet restore "AFMA Stats.sln"

error NU1101: Unable to find package Resolvetech.NetCore.Common.
  No packages exist with this id in source(s): AfmaLocalPackages,
  Microsoft Visual Studio Offline Packages, nuget.org
```

Five projects fail to restore: `AFMA.ApplicationLayer`, `AFMA.AutoMapping`, `AFMA.Shared.Services`,
`AFMA.Stats.Client`, `AFMA.Stats.Admin`. These include both web applications, so nothing deployable
builds.

Seven projects do restore — the entity, mapping, templating, infrastructure and communication
libraries.

### `AFMA-MMS`

```
$ git clone https://eclick-dev@dev.azure.com/eclick-dev/AFMA-MMS/_git/AFMA-MMS
$ cd AFMA-MMS && dotnet restore "AFMA Stats.sln"

error NU1301: The local source '...\AFMA-MMS\Packages' doesn't exist.
```

Every project fails. `AFMA-MMS/nuget.config` does this:

```xml
<packageSources>
  <clear />
  <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  <add key="AFMA-Local-Packages" value="Packages" />
</packageSources>
```

It declares a local `Packages` folder. **That folder is not in the repository** — `git ls-files`
returns zero paths under `Packages/`. NuGet treats a missing local source as a hard error, so restore
stops before it reaches nuget.org.

## The private dependencies

Two package families are not publicly available and are load-bearing.

| Package | Version needed by STAT | Version needed by MMS | Where it is |
|---|---|---|---|
| `Alpha.Services.Core` | 9.0.18 | 9.0.20 | 9.0.18 vendored in `AFMA-STAT/Packages/`. 9.0.20 **missing** |
| `Alpha.Services.Extensions` | 9.0.18 | 9.0.20 | as above |
| `Alpha.Services.Extensions.Mvc` | 9.0.18 | 9.0.20 | as above |
| `Alpha.Services.Models` | 9.0.18 | 9.0.20 | as above |
| `Alpha.Services.NHibernate.Repository` | 9.0.18 | 9.0.20 | as above |
| `Alpha.Services.Notifications` | 9.0.18 | 9.0.20 | as above |
| `Alpha.Services.Notifications.Templates` | 9.0.18 | 9.0.20 | as above |
| `Resolvetech.NetCore.Common` | 1.0.3.1004 | 1.0.3.1004 | **missing entirely** |

`Alpha.Services.Core.nuspec` records the author as **JJ Swart**. The packages ship as `net8.0` DLLs
with no source.

These are not peripheral utilities. They provide:

- `Alpha.Services.Core.Builders.ServiceCollectionBuilder` — the service composition mechanism that
  every `Program.cs` is built on
- `Alpha.Services.NHibernate.Repository.Configuration` — the NHibernate repository and session
  configuration for both databases
- `Alpha.Services.Notifications` and `.Templates` — the entire email notification pipeline

Without them there is no data access and no email.

## What restore does tell us

Two high-severity advisories surfaced during the partial restore:

| Package | Version | Advisory |
|---|---|---|
| `NHibernate` | 5.4.6 | [GHSA-fg4q-ccq8-3r5q](https://github.com/advisories/GHSA-fg4q-ccq8-3r5q) |
| `AutoMapper` | 12.0.1 | [GHSA-rvv3-g6hj-g44x](https://github.com/advisories/GHSA-rvv3-g6hj-g44x) |

See [Risk Register](/Risk-Register) R6.

## Developer environment prerequisites

Recorded so a new developer knows what to install. **UNVERIFIED beyond the framework targets** — this
list is derived from the project files, not from a working developer machine. Confirm with Chris and
correct this section. *Owner: Ethan. Due: 2026-09-19.*

| Requirement | Detail |
|---|---|
| Visual Studio | 2022 (solution records `VisualStudioVersion 17.0.32002.185`) |
| .NET SDK | 8.0 — 12 of 13 projects target `net8.0` |
| **ASP.NET Core 8 runtime** | Required to run the web applications. Note: a machine with only the .NET 8 *base* runtime (`Microsoft.NETCore.App 8.0.14`) cannot run these — `Microsoft.AspNetCore.App 8.0.x` is needed |
| .NET 6 SDK | Only if the orphaned `Infrastructure/AFMA.Infrastructure` project is kept. It should be deleted instead — see R6 |
| SQL Server | Local instance. Development connection strings expect `Server=(local)` with databases `afmastats` and `afmaweb`/`afmawims` |
| IIS Express | `AFMA.Stats.Client` uses ports 52403 (HTTP) and 44360 (HTTPS); Kestrel profile uses 7154/5072 |
| Private packages | See above. **Without these, nothing builds.** |

**GAP** — there is no documented procedure for getting a working local database. With no schema in
source control (R11) and no seed data, a new developer cannot stand up a development environment even
once the packages are resolved. *Owner: Henry. Due: 2026-09-30.*

## Target frameworks

| Repository | net8.0 | net6.0 |
|---|---|---|
| `AFMA-STAT` | 12 projects | 1 project — `Infrastructure/AFMA.Infrastructure`, orphaned |
| `AFMA-MMS` | 13 projects | 1 project — same orphan, carried over by the fork |

.NET 6 is out of support. The orphaned project is not referenced by any solution or project file and
duplicates the assembly name `AFMA.Infrastructure` used by the live
`Shared/AFMA.Shared.Infrastructure` project. Delete it. *Owner: Henry. Due: 2026-09-26.*

## Build reproducibility

| Issue | Detail |
|---|---|
| Floating versions | `System.ServiceModel.Http`, `.NetTcp` and `.Primitives` are all pinned to `8.*`. The same source can produce different binaries on different days. Pin them. |
| Mixed EF Core majors | 6.0.11, 6.0.16 and 8.0.13 referenced within one solution |
| Two ORMs | NHibernate 5.4.6 is primary; EF Core is used for Identity. Both are live |
| Duplicated assembly name | `AFMA.Infrastructure` produced by two different project files |
| No lock files | No `packages.lock.json` in either repository |

## Route to a reproducible build

The sequence matters — each step depends on the one before.

1. **Obtain the missing packages.** `Alpha.Services.* 9.0.20` and `Resolvetech.NetCore.Common
   1.0.3.1004`, as `.nupkg` files. *Owner: Ethan, via Chris. Due: 2026-09-19.*
2. **Establish whether source exists** for those packages and on what terms we may hold it.
   *Owner: Ethan. Due: 2026-09-22.*
3. **Create an Azure Artifacts feed** in `eclick-dev` and publish both families to it.
   *Owner: Henry. Due: 2026-09-30.*
4. **Repoint both `NuGet.config` files** at the feed. Remove the vendored `AFMA-STAT/Packages/`
   folder. *Owner: Henry. Due: 2026-09-30.*
5. **Pin floating versions** and add `packages.lock.json`. *Owner: Henry. Due: 2026-09-30.*
6. **Prove it.** Someone who has never built these applications clones both on a clean machine and
   builds them, following only this page. Update this page with whatever they had to work out for
   themselves. *Owner: Henry. Due: 2026-09-30.*

Step 6 is the one that matters. Until it passes, the account is still dependent on knowledge held in
one person's head, which is the thing the handover exists to fix.
