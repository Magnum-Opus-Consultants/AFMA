# Repositories and Branches

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Status** | **VERIFIED** 2026-09-14 |

## Repositories

Azure DevOps organisation `eclick-dev`. Each repository sits in its own project of the same name.

| Repository | Clone URL | State at 2026-09-14 |
|---|---|---|
| `AFMA-STAT` | `https://eclick-dev@dev.azure.com/eclick-dev/AFMA-STAT/_git/AFMA-STAT` | 7 commits, branches `master` and `develop`, HEAD `895ebb7` |
| `AFMA-MMS` | `https://eclick-dev@dev.azure.com/eclick-dev/AFMA-MMS/_git/AFMA-MMS` | 1 commit, branch `master` only, HEAD `f0d6bc8` |
| `AFMA-WEB` | `https://eclick-dev@dev.azure.com/eclick-dev/AFMA-WEB/_git/AFMA-WEB` | **Empty. Never initialised.** |

Default branch on both populated repositories is **`master`**, not `main`.

## Branches

### `AFMA-STAT`

| Branch | State |
|---|---|
| `master` | Default. HEAD `895ebb7`. |
| `develop` | **Identical to `master`.** `git log origin/master..origin/develop` and the reverse both return nothing. |

`develop` exists but carries no work. Either the team stopped using it or it was created and never
diverged. Either way there is currently **no branching strategy in effect** — the last seven commits
all went straight to `master`.

### `AFMA-MMS`

| Branch | State |
|---|---|
| `master` | Default and only branch. Single commit `f0d6bc8`. |

## Commit history

### `AFMA-STAT` — 7 commits

| Commit | Date | Author | Subject |
|---|---|---|---|
| `895ebb7` | 2026-07-06 | Marnus Badenhorst | Add custom decimal model binder to accept both comma and dot for feed sales and raw materials |
| `ca5f61a` | 2026-07-06 | Marnus Badenhorst | Fix decimal parsing to accept both comma and dot as decimal separators |
| `b021c1e` | 2026-06-24 | Chris Delport | chore: populate .gitignore with backup and publish rules |
| `bb9c3ed` | 2026-06-24 | Chris Delport | chore: add .gitignore for db backups and publish output |
| `a18fcab` | 2026-06-24 | Chris Delport | feat: use client-supplied offline message and render multi-paragraph body with mailto links |
| `5af0d88` | 2026-06-24 | Chris Delport | feat: add configurable maintenance-mode middleware with bypass token |
| `1b81644` | 2026-05-12 | Marnus Badenhorst | Initial commit |

Contributors: Chris Delport `<chris.delport@eclick.co.za>` (4), Marnus Badenhorst
`<badenhorstmarnus@gmail.com>` (3).

### `AFMA-MMS` — 1 commit

| Commit | Date | Author | Subject |
|---|---|---|---|
| `f0d6bc8` | 2026-08-26 | Marnus Badenhorst | Initial AFMA MMS migration |

## The fork relationship

**VERIFIED 2026-09-14.** `AFMA-MMS` is a copy of the `AFMA-STAT` codebase, not an independent project:

- Both contain a solution file named `AFMA Stats.sln`.
- The project GUIDs are **identical** between the two solutions.
- `AFMA-MMS` contains `AFMA.Stats.Client` and `AFMA.Stats.Admin` — the STATS applications — in
  addition to `AFMA.Web.Admin`.

Because `AFMA-MMS` was committed as a single squashed commit, the repositories share **no common
ancestor**. Git cannot merge, rebase or cherry-pick between them in any meaningful way. Any transfer
of code is a manual copy.

### Known divergences

Both directions, as at 2026-09-14. This list must be maintained until the architecture decision in
[Risk Register](/Risk-Register) R8 is taken.

**In `AFMA-STAT`, missing from `AFMA-MMS`:**

| Change | Commits |
|---|---|
| Decimal model binder — comma and dot separators for feed sales and raw materials | `ca5f61a`, `895ebb7` |
| Configurable maintenance-mode middleware with bypass token | `5af0d88` |
| Client-supplied offline message, multi-paragraph rendering with mailto links | `a18fcab` |
| `.gitignore` rules forbidding database backups and publish output | `bb9c3ed`, `b021c1e` |

The last of these is why `AFMA-MMS` holds a 58 MB database backup — see [Risk Register](/Risk-Register) R2.

**In `AFMA-MMS`, missing from `AFMA-STAT`:**

| Change |
|---|
| 17 additional WIMS entities and mappings — committees, documents, manufacturers, suppliers, member users, notification templates |
| `AFMA.Web.Admin` web project |
| `ExcelExportService` / `IExcelExportService` |
| `MaxFileSizeAttribute` |
| `Model/Export` model set, `DeleteViewModel`, `MemberUserBranchViewModel` |
| AutoMapper profiles: `CommitteeMapping`, `DocumentMapping`, `NotificationMapping`, `WimsMemberUserMapping` |
| `Alpha.Services.*` upgraded 9.0.18 → 9.0.20 |
| EF Core 8 and `Microsoft.AspNetCore.Identity.UI` references |
| `Member` role |

## Repository size and contents

| | `AFMA-STAT` | `AFMA-MMS` |
|---|---|---|
| Working tree | 87 MB (`Website/`) + 1.7 MB (`Application/`) | 175 MB (`Website/`) + 1.8 MB (`Application/`) |
| `.git` | 40 MB | **124 MB** |
| Files at checkout | 2,492 | 3,289 |
| C# files / lines | 441 / 44,812 | 532 / 51,614 |
| Razor views | 282 | 407 |
| Test projects | **0** | **0** |

Both are heavier than they should be. Contributing factors, all verified:

- `AFMA-MMS` carries `afmawims_10072026.bak` — 58 MB, in history permanently (R2).
- `AFMA-STAT` vendors `.nupkg` binaries in `Packages/` (R1).
- `AFMA-STAT` has 18 files under `.vs/` committed, including Visual Studio Copilot index databases
  (R12).
- Both commit the full `wwwroot` vendor asset tree — Font Awesome, ApexCharts, DataTables, FullCalendar,
  PDFMake and others.

## `.gitignore` state

| Repository | File | Assessment |
|---|---|---|
| `AFMA-STAT` | 8 lines: `_bk/`, `*.bak`, `*.trn`, `publish/` | Correctly forbids database backups. **Does not exclude `.vs/` or `*.pubxml.user`**, both of which are committed. |
| `AFMA-MMS` | Stock Visual Studio template, ~350 lines | Comprehensive on build output. **Does not exclude `*.bak`** — which is how the database backup was committed. |

**Action.** Merge the two: `AFMA-STAT`'s backup rules into `AFMA-MMS`, the Visual Studio template's
`.vs/` and `*.user` rules into `AFMA-STAT`. Do it as part of the R2 history rewrite.
*Owner: Henry. Due: 2026-09-26.*

## Governance — what is missing

| Control | State | Action |
|---|---|---|
| Branch policies on `master` | **None** | Require pull request, one reviewer, linked work item. *Owner: Ethan. Due: 2026-09-26* |
| Pull request history | **None** — every commit is a direct push | Follows from the above |
| Build validation | **None** | Blocked on [Risk Register](/Risk-Register) R1 |
| Secret scanning | **Not enabled** — Advanced Security is available in this organisation | *Owner: Ethan. Due: 2026-09-26* |
| Commit message convention | Inconsistent — Chris uses `feat:`/`chore:` prefixes, Marnus does not | Agree one and record it here |
| Repository push size limit | **None** | Would have prevented R2. *Owner: Henry. Due: 2026-09-26* |
| `README.md` in either repository | **None** | Every repository should carry one pointing at this wiki. *Owner: Henry. Due: 2026-09-26* |
