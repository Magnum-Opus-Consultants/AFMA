# Account Overview

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Last reviewed** | 2026-09-14 |
| **Source of truth** | This page |

## The client

The Animal Feed Manufacturers Association (AFMA) is an industry association. Its members are feed
manufacturers. AFMA collects production and safety statistics from those members, aggregates them,
and publishes industry-level reporting back to the membership.

AFMA office contact points that appear in the systems themselves (**VERIFIED** 2026-09-14, from
application configuration and templates):

| Contact | Appears as | Where |
|---|---|---|
| `stats@afma.co.za` | Statistics mailbox, submission notifications | `SMTPSettings:SubmissionStatsEmail`, `SMTPSettings:DebugEmail` |
| `petru@afma.co.za` | Named contact on the maintenance page | `MaintenanceSettings:Message`, `deploy/app_offline.htm` |
| `afma@afma.co.za` | Outbound "from" address on the legacy email service | `AFMA.Service.Communication/EmailService.cs` |
| `system@afmastats.co.za` | Outbound "from" address on the STATS portal | `SMTPSettings:FromAddress` |
| +27 (0) 12 663 9097 | AFMA office telephone | Maintenance page copy |

**UNVERIFIED** — these are addresses the software uses, not a confirmed contact list. The actual
client-side stakeholders, their roles and who may authorise change must be confirmed and recorded
here. *Owner: Ethan. Due: 2026-09-21.*

## What we run for AFMA

| System | What it is | Repository | Page |
|---|---|---|---|
| **AFMA STATS** | Member-facing statistics submission portal plus an internal admin/reporting application. Covers Feed Sales, Salmonella, Mycotoxin, Industry Profile and Finished Product. | `AFMA-STAT` | [AFMA STATS](/Systems/AFMA-STATS) |
| **AFMA MMS** | Member management system — members, branches, contacts, committees, documents, distribution groups, notification batches, training courses. A fork of the STATS codebase. | `AFMA-MMS` | [AFMA MMS](/Systems/AFMA-MMS) |
| **AFMA website** | The public WordPress site on cPanel hosting. | `AFMA-WEB` (empty) | [AFMA Website](/Systems/AFMA-Website) |

## Who does what

| Person | Role on this account | Notes |
|---|---|---|
| **Ethan Sevenster** | Accountable for the technical landscape of the account | Owns this wiki |
| **Henry** | Technical handover counterpart | Available from 2026-09-21 (returning from NAB) |
| **Chris Delport** | Historical context, system architecture, unresolved technical dependencies | Engage only where history or architecture genuinely requires it. Contributor to `AFMA-STAT` (maintenance mode, offline page, June 2026) |
| **Marnus Badenhorst** | Original author of both codebases | Author of every commit in `AFMA-MMS` and the majority in `AFMA-STAT` |
| **Anthony** | Holds access to the AFMA Wiki in Azure DevOps | |

**GAP** — Marnus Badenhorst is the author of the initial commit of both repositories and of the
`AFMA-MMS` codebase in its entirety. His current relationship to the account is not recorded here.
This matters because the code carries dependencies that may only be resolvable through him — see
[Risk Register](/Risk-Register), R1. *Owner: Ethan. Due: 2026-09-21.*

## Azure DevOps organisation

Organisation: `eclick-dev` (`https://dev.azure.com/eclick-dev`)

| Project | Contains |
|---|---|
| `AFMA-STAT` | The AFMA STATS solution |
| `AFMA-MMS` | The AFMA member management solution |
| `AFMA-WEB` | Empty repository. No code has ever been pushed. |
| `AFMA` (wiki) | The AFMA Wiki referenced in the handover |

**GAP** — Boards, pipelines, test plans and artifacts have not been reviewed in any of these
projects. *Owner: Ethan. Due: 2026-09-19.*
