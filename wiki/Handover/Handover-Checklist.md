# Handover Checklist

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Last updated** | 2026-09-14 |

The line-by-line record. Update the status column as items complete; do not delete rows.

Status values: **Done** / **In progress** / **Blocked** / **Not started**

## 1. Access

| # | Item | Owner | Due | Status |
|---|---|---|---|---|
| 1.1 | Azure DevOps — `AFMA-STAT` repository | Ethan | — | **Done** 2026-09-14 |
| 1.2 | Azure DevOps — `AFMA-MMS` repository | Ethan | — | **Done** 2026-09-14 |
| 1.3 | Azure DevOps — `AFMA-WEB` repository | Ethan | — | **Done** 2026-09-14 (empty) |
| 1.4 | Azure DevOps — AFMA Wiki, contribute rights | Ethan | 2026-09-16 | Not started |
| 1.5 | Azure DevOps — Boards, all three projects | Ethan | 2026-09-16 | Not started |
| 1.6 | Azure DevOps — Pipelines, Artifacts, Advanced Security | Ethan | 2026-09-16 | Not started |
| 1.7 | Windows/IIS server hosting STATS and MMS | Ethan | 2026-09-19 | Not started |
| 1.8 | SQL Server `WIN-MDOU42447UT` | Ethan | 2026-09-19 | Not started |
| 1.9 | cPanel — AFMA website | Ethan | 2026-09-19 | Not started |
| 1.10 | WordPress administrator | Ethan | 2026-09-19 | Not started |
| 1.11 | Domain registrar and DNS | Ethan | 2026-09-19 | Not started |
| 1.12 | Mailgun | Ethan | 2026-09-22 | Not started |
| 1.13 | Resolve email portal | Ethan | 2026-09-22 | Not started |
| 1.14 | Mailchimp | Ethan | 2026-09-22 | Not started |
| 1.15 | Power BI workspace | Ethan | 2026-09-22 | Not started |
| 1.16 | Backup system | Ethan | 2026-09-19 | Not started |
| 1.17 | Existing credential store | Ethan | 2026-09-19 | Not started |
| 1.18 | Licence and billing records | Ethan | 2026-09-25 | Not started |

## 2. Critical risk remediation

| # | Item | Risk | Owner | Due | Status |
|---|---|---|---|---|---|
| 2.1 | Rotate all credentials committed to git | R3 | Henry | 2026-09-19 | Not started |
| 2.2 | Establish who has had `AFMA-MMS` read access | R2 | Ethan | 2026-09-16 | Not started |
| 2.3 | POPIA assessment on the exposed member database | R2 | Ethan + Pieter | 2026-09-19 | Not started |
| 2.4 | Obtain `Alpha.Services.* 9.0.20` and `Resolvetech.NetCore.Common 1.0.3.1004` | R1 | Ethan via Chris | 2026-09-19 | Not started |
| 2.5 | Establish whether source exists for the private packages, and licence terms | R1 | Ethan | 2026-09-22 | Not started |
| 2.6 | Azure Artifacts feed created and packages published | R1 | Henry | 2026-09-30 | Not started |
| 2.7 | Both `NuGet.config` repointed; vendored `Packages/` removed | R1 | Henry | 2026-09-30 | Not started |
| 2.8 | **Clean-machine build of both solutions proven** | R1 | Henry | 2026-09-30 | Not started |
| 2.9 | Purge `.bak` from `AFMA-MMS` history; force-push; re-clone | R2 | Henry | 2026-09-26 | Not started |
| 2.10 | `.gitignore` merged in both repositories; tracked `.vs/` removed | R2, R12 | Henry | 2026-09-26 | Not started |
| 2.11 | Server-side push size limit | R2 | Henry | 2026-09-26 | Not started |
| 2.12 | Secrets moved out of `appsettings.json` | R3 | Henry | 2026-09-26 | Not started |
| 2.13 | Sanitised `appsettings.json` committed with placeholders | R3 | Henry | 2026-09-26 | Not started |
| 2.14 | Secret scanning enabled on both repositories | R3 | Ethan | 2026-09-26 | Not started |
| 2.15 | Credential store chosen and populated | R3 | Ethan + Pieter | 2026-09-26 | Not started |
| 2.16 | `operations@resolvetech.co.za` BCC resolved | R5 | Ethan → Henry | 2026-09-26 | Not started |
| 2.17 | `EmailService.ProcessAndSendBatch` removed | R5 | Henry | 2026-09-26 | Not started |
| 2.18 | Orphaned net6.0 project deleted | R6 | Henry | 2026-09-26 | Not started |
| 2.19 | Floating `8.*` package versions pinned | R6 | Henry | 2026-09-26 | Not started |
| 2.20 | NHibernate and AutoMapper upgraded past advisories | R6 | Henry | 2026-10-17 | Not started |
| 2.21 | EPPlus licence position decided | R7 | Ethan + Pieter | 2026-10-03 | Not started |
| 2.22 | `/external/` anonymous access confirmed or secured | R10 | Ethan → Henry | 2026-09-26 | Not started |
| 2.23 | Production schemas scripted and committed | R11 | Henry | 2026-09-30 | Not started |
| 2.24 | Migration approach adopted | R11 | Henry + Ethan | 2026-10-10 | Not started |
| 2.25 | `AFMA-STAT` / `AFMA-MMS` architecture decision taken | R8 | Ethan + Henry + Chris | 2026-10-03 | Not started |

## 3. Backup and recovery

| # | Item | Owner | Due | Status |
|---|---|---|---|---|
| 3.1 | Establish whether any automated backup is running | Ethan | 2026-09-19 | Not started |
| 3.2 | If none, get one running | Henry | on confirmation | Not started |
| 3.3 | Verified offsite baseline backup of both databases | Henry | 2026-09-22 | Not started |
| 3.4 | Backup location decided and documented | Ethan + Pieter | 2026-09-26 | Not started |
| 3.5 | One test restore performed and recorded | Henry | 2026-09-30 | Not started |
| 3.6 | Recovery procedures written | Henry | 2026-10-10 | Not started |
| 3.7 | RPO and RTO agreed with AFMA | Ethan | 2026-10-10 | Not started |
| 3.8 | Member-uploaded document storage located and backed up | Henry | 2026-10-03 | Not started |

## 4. Documentation — the twelve review areas

| # | Area | Owner | Due | Status |
|---|---|---|---|---|
| 4.1 | Website, WordPress, cPanel | Ethan | 2026-09-22 | **Blocked** on 1.9, 1.10 |
| 4.2 | Hosting, domains, DNS, SSL, backups | Ethan | 2026-10-03 | **Blocked** on 1.7, 1.11 |
| 4.3 | Plugins, forms, PDF tools, third-party services | Ethan | 2026-09-22 | **Blocked** on 1.10 |
| 4.4 | Mailchimp, Power BI, other integrations | Ethan | 2026-09-25 | In progress — email paths documented |
| 4.5 | AFMA STATS application and hosting | Ethan | 2026-10-03 | In progress — application documented |
| 4.6 | Feed Sales and Salmonella datasets and flows | Ethan + Henry | 2026-10-10 | In progress — application side documented |
| 4.7 | Repositories, branches, deployment | Ethan | — | **Done** 2026-09-14 (deployment still a gap) |
| 4.8 | Licences, subscriptions, renewals, costs, ownership | Ethan + Pieter | 2026-10-03 | **Blocked** on 1.18 |
| 4.9 | Security, access control, backup, recovery | Ethan | 2026-10-03 | In progress — application findings recorded |
| 4.10 | Support items, defects, outstanding developments | Ethan | 2026-10-03 | **Blocked** on 1.5 |
| 4.11 | Technical debt, legacy dependencies, undocumented functionality | Ethan | — | **Done** 2026-09-14 |
| 4.12 | Member-management opportunities | Ethan | 2026-10-03 | In progress — MMS assessed |

## 5. Business knowledge — highest loss risk

Nothing here exists in writing anywhere today.

| # | Item | Owner | Due | Status |
|---|---|---|---|---|
| 5.1 | Feed Sales — periods, deadlines, validation, aggregation, corrections | Ethan + Henry | 2026-10-10 | Not started |
| 5.2 | Salmonella — testing regime, frequency, thresholds, publication | Ethan + Henry | 2026-10-10 | Not started |
| 5.3 | Mycotoxin — same | Ethan + Henry | 2026-10-10 | Not started |
| 5.4 | Industry Profile — cost, fleet, production, staffing structures | Ethan + Henry | 2026-10-10 | Not started |
| 5.5 | Role model — what `Admin`, `User` and `Member` may each do | Henry | 2026-09-30 | Not started |
| 5.6 | Member onboarding — how a member user is created and linked | Henry | 2026-09-30 | Not started |
| 5.7 | MMS live status and the `ComingSoon` gap list | Henry | 2026-09-30 | Not started |
| 5.8 | Local development environment setup, end to end | Henry | 2026-09-30 | Not started |

## 6. Process and governance

| # | Item | Owner | Due | Status |
|---|---|---|---|---|
| 6.1 | This wiki published to Azure DevOps | Ethan | 2026-09-16 | Not started |
| 6.2 | Branch policies on `master`, both repositories | Ethan | 2026-09-26 | Not started |
| 6.3 | `README.md` in each repository pointing here | Henry | 2026-09-26 | Not started |
| 6.4 | Build pipelines | Henry | 2026-10-10 | **Blocked** on 2.8 |
| 6.5 | Release pipelines | Henry | 2026-10-17 | **Blocked** on 6.4 |
| 6.6 | Version stamping at publish time | Henry | 2026-10-10 | Not started |
| 6.7 | Support model agreed | Ethan + Henry | 2026-10-24 | Not started |
| 6.8 | Client communication cadence established | Ethan | 2026-09-19 | Not started |
| 6.9 | Wiki review cycle set, owner per section | Ethan | 2026-10-24 | Not started |
| 6.10 | Renewal calendar live with alerts | Ethan | 2026-10-03 | Not started |
| 6.11 | Joiner/leaver access procedure | Ethan | 2026-09-26 | Not started |
| 6.12 | Characterisation test scope agreed | Henry | 2026-10-03 | Not started |

## 7. Verification

The handover is complete when these pass, and not before.

| # | Test | Owner | Due | Status |
|---|---|---|---|---|
| 7.1 | Someone who has never built these applications clones and builds both, using only this wiki | Henry | 2026-09-30 | Not started |
| 7.2 | A test restore of both databases succeeds and is recorded | Henry | 2026-09-30 | Not started |
| 7.3 | A deployment is performed by someone other than its usual owner, following this wiki | Henry | 2026-10-17 | Not started |
| 7.4 | Every area on [Technical Landscape](/Technical-Landscape) reads **VERIFIED** | Ethan | 2026-10-24 | Not started |
| 7.5 | Every **Critical** and **High** item on the [Risk Register](/Risk-Register) is closed or has an accepted, dated decision | Ethan | 2026-10-24 | Not started |
| 7.6 | A third person performs a routine support task on AFMA using only this wiki | Ethan | 2026-10-31 | Not started |
