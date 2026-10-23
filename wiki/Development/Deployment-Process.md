# Deployment Process

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Status** | **GAP** — the actual process is undocumented. What follows is what the repositories reveal, plus the template to complete once Chris has described it. |
| **Last reviewed** | 2026-09-14 |

## What is verified

**There is no deployment automation of any kind.** Verified 2026-09-14:

- No Azure Pipelines YAML in either repository
- No classic build or release definitions
- No deployment scripts
- No Dockerfile, no container configuration
- No infrastructure-as-code

The only deployment artefacts in existence are two Visual Studio folder publish profiles, both for
`AFMA.Stats.Admin`:

| Profile | Target framework | Publish URL | Recorded runs |
|---|---|---|---|
| `FolderProfile.pubxml` | `net6.0` | `D:\Development\Published\AFMA.Admin` | 2025-02-26, 2025-03-02, 2025-03-08 |
| `FolderProfile1.pubxml` | `net8.0` | `D:\Development\Published\AFMA.Admin` | 2025-08-11 |

Both set `DeleteExistingFiles=true`, `SelfContained=false`, configuration `Release`.

The run dates come from committed `.pubxml.user` files. Those are **per-developer local artefacts**,
not a deployment log — they record when someone clicked Publish in Visual Studio, on whichever machine
the file came from. They are not evidence of what is running in production.

There are **no publish profiles at all** for:

- `AFMA.Stats.Client` — the member-facing portal
- Any `AFMA-MMS` project, including `AFMA.Web.Admin`

## What this means

Releasing AFMA STATS today requires a specific person, on a specific machine, that has:

1. The private NuGet packages cached locally, because the repository alone will not restore
   (see [Build and Environment](/Development/Build-and-Environment))
2. Visual Studio with the right publish profile
3. The production connection strings and secrets, from somewhere outside source control
4. Whatever means is used to move `D:\Development\Published\AFMA.Admin` onto the server
5. Credentials for that server

None of steps 3, 4 or 5 are written down anywhere.

This is [Risk Register](/Risk-Register) **R4**.

## Taking the site offline

The one part of the release process that **is** properly built and documented is maintenance mode.
Chris added it in June 2026 and it is well made. See [Maintenance Mode](/Operations/Maintenance-Mode)
for how to use it.

`AFMA-STAT/deploy/app_offline.htm` also exists — the classic ASP.NET Core mechanism. Dropping that
file into the application root takes the site offline immediately without touching configuration. It
carries the same AFMA-branded copy as the maintenance page.

Note that maintenance mode is registered on both `AFMA.Stats.Client` and `AFMA.Stats.Admin` but
**not** on `AFMA-MMS`'s `AFMA.Web.Admin`. MMS has no clean way to be taken offline.

## To be completed from Chris

Question Q2 on [Open Questions](/Open-Questions). *Owner: Ethan. Due: 2026-09-19.*

Fill this in verbatim as described, then verify it by following it.

### Environments

| Environment | Purpose | URL | Server | Database | Who can deploy |
|---|---|---|---|---|---|
| Production — STATS Client | | | | | |
| Production — STATS Admin | | | | | |
| Production — MMS | | | | | |
| Test / staging, if any | | | | | |

`ApplicationSettings:TestSite` is a boolean in configuration — `true` on the STATS Client,
`false` on the STATS Admin, in the committed files. That implies a test deployment exists, or once
did. Establish which. *Owner: Ethan. Due: 2026-09-19.*

### Release procedure, step by step

| # | Step | Who | Notes |
|---|---|---|---|
| 1 | | | |
| 2 | | | |
| 3 | | | |

Specifically to capture:

- How published output reaches the server — FTP, Web Deploy, RDP and copy, a network share?
- Whether the app pool is stopped, and how
- Whether maintenance mode or `app_offline.htm` is used, and at which point
- How `appsettings.json` production values are applied — are they edited on the server, held in a
  separate file, or set as environment variables?
- How database changes are applied, given there are no migration scripts (R11)
- What the rollback procedure is
- Who verifies the release afterwards, and against what

### Release history

**GAP** — there is no record of what was deployed when. The `Version` setting in both applications
still reads the placeholder `"1.0.XXX"`, so **the running application cannot report its own build**.

Fixing that is a small change with a large payoff: stamp the build number into `Version` at publish
time so that a support call can start with "which version are you on" and get an answer.
*Owner: Henry. Due: 2026-10-10.*

| Date | System | Version | What changed | Deployed by |
|---|---|---|---|---|
| | | | | |

## Target state

Once [Risk Register](/Risk-Register) R1 is resolved and the applications restore, the sequence is:

1. **Build pipeline** on both repositories — restore, build, publish artefact. Wire it as a branch
   policy on `master` so nothing merges that does not compile. *Owner: Henry. Due: 2026-10-10.*
2. **Release pipeline** to the production server, with maintenance mode engaged and disengaged as
   part of the release. *Owner: Henry. Due: 2026-10-17.*
3. **Secrets from a store**, injected at deploy time, never in the repository (R3).
4. **Version stamping** so every deployment is identifiable.
5. **A staging environment** if one does not already exist — currently there is nowhere to verify a
   release before members see it.

Until then, the interim rule: **no deployment happens without it being recorded in the release history
table above, the same day, by whoever performed it.** That costs a minute and is the difference
between a supportable account and a guessable one.
