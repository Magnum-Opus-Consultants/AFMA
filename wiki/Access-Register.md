# Access Register

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Last reviewed** | 2026-09-14 |

What access we hold, what we are missing, and who is chasing it. Missing access is the reason most of
this wiki is still **GAP**.

> This page records *that* access exists and who holds it. It never records passwords, keys or
> tokens. Those belong in the credential store — see [Credential handling](#credential-handling).

## Access we hold — confirmed

| System | Held by | Level | Confirmed |
|---|---|---|---|
| Azure DevOps `eclick-dev` — `AFMA-STAT` repository | Ethan | Read/clone (verified by cloning 2026-09-14) | 2026-09-14 |
| Azure DevOps `eclick-dev` — `AFMA-MMS` repository | Ethan | Read/clone (verified by cloning 2026-09-14) | 2026-09-14 |
| Azure DevOps `eclick-dev` — `AFMA-WEB` repository | Ethan | Read (repository is empty) | 2026-09-14 |
| AFMA Wiki in Azure DevOps | Anthony | Unknown level | Reported, not verified |

## Access required — outstanding

Each row is a blocker on a section of the [Technical Landscape](/Technical-Landscape) review.

| # | Access needed | Unblocks | Request from | Owner | Due |
|---|---|---|---|---|---|
| A1 | AFMA Wiki in Azure DevOps — contribute rights | Publishing this wiki at all | Azure DevOps org admin | Ethan | 2026-09-16 |
| A2 | Azure DevOps Boards — all three AFMA projects | Support items, defects, outstanding developments (area 10) | Azure DevOps org admin | Ethan | 2026-09-16 |
| A3 | Azure DevOps Pipelines, Artifacts, Advanced Security | Deployment automation, private package feed (R1), secret scanning (R3) | Azure DevOps org admin | Ethan | 2026-09-16 |
| A4 | cPanel for the AFMA website | Website, WordPress, plugins, forms, PDF tools (areas 1 and 3) | Chris / Henry | Ethan | 2026-09-19 |
| A5 | WordPress administrator login | Same as A4 | Chris / Henry | Ethan | 2026-09-19 |
| A6 | Domain registrar and DNS management | Domains, DNS, SSL (area 2) | Chris / Henry | Ethan | 2026-09-19 |
| A7 | Windows/IIS server hosting AFMA STATS and MMS | Hosting environment, deployment, logs (areas 2, 5, 7) | Chris / Henry | Ethan | 2026-09-19 |
| A8 | SQL Server instance `WIN-MDOU42447UT` — `afmastats` and `afmawims` | Schema, datasets, data flows, backups (areas 6, 9) | Chris / Henry | Ethan | 2026-09-19 |
| A9 | Mailgun account (`mailadmin@afma.eclick.co.za`) | Mail deliverability, integrations (area 4) | Chris / Henry | Ethan | 2026-09-22 |
| A10 | Resolve email portal (`emailportal.resolvecloud.co.za`) | Legacy email service dependency (area 4) | Chris | Ethan | 2026-09-22 |
| A11 | Mailchimp account | Integrations (area 4) | AFMA / Chris | Ethan | 2026-09-22 |
| A12 | Power BI workspace | Integrations, reporting (areas 4, 6) | AFMA / Chris | Ethan | 2026-09-22 |
| A13 | Backup system and offsite backup location | Backup and recovery (area 9) | Chris / Henry | Ethan | 2026-09-19 |
| A14 | Licence, subscription and renewal records | Costs and account ownership (area 8) | Pieter / finance | Ethan | 2026-09-25 |
| A15 | Existing credential store, wherever it currently is | Everything | Chris / Henry | Ethan | 2026-09-19 |

**Escalate rather than wait.** Per the handover brief, missing access is to be raised immediately, not
accumulated. Anything on this list still outstanding at its due date goes into the weekly account
note.

## Access we should not need — and should remove

Identified from the code review, to confirm against the actual environments:

| Item | Why it matters |
|---|---|
| Whoever can currently read `AFMA-MMS` can restore the member database from its git history | See [Risk Register](/Risk-Register) R2 |
| Whoever can read either repository holds live production credentials | See [Risk Register](/Risk-Register) R3 |
| `operations@resolvetech.co.za` receives copies of member password-reset mail | See [Risk Register](/Risk-Register) R5 |

**GAP** — there is no record of who has access to any AFMA system. Building that record is the point
of this page. *Owner: Ethan. Due: 2026-09-26.*

## Credential handling

**Decision required.** There is currently no credential store for this account. Secrets live in
`appsettings.json` in git, which is how R3 happened.

Until a store is chosen and populated:

1. Do not paste any credential into this wiki, into a work item, or into email or chat.
2. Record in this wiki only *that* a credential exists, what it opens, and who holds it.
3. Rotate anything that has been in git — exposure is already assumed.

*Owner: Ethan, decision with Pieter and Henry. Due: 2026-09-26.*

## Leaver and joiner procedure

**GAP** — there is no procedure for granting or revoking access to AFMA systems when someone joins or
leaves the account. Given the handover in progress, this is needed before, not after.
*Owner: Ethan. Due: 2026-09-26.*
