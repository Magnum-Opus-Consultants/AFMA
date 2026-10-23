# Technical Landscape — Review Status

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Last reviewed** | 2026-09-14 |

This page tracks the twelve areas of the technical landscape review. It is the index against which
completeness is judged. Nothing here is closed until the linked page carries **VERIFIED** facts.

| # | Area | Status | Where it lives | Owner | Due |
|---|---|---|---|---|---|
| 1 | AFMA website, WordPress and cPanel environments | **GAP** — no access, no code in source control | [AFMA Website](/Systems/AFMA-Website) | Ethan | 2026-09-19 |
| 2 | Hosting, domains, DNS, SSL and backups | **GAP** — no console access held | [Hosting and Infrastructure](/Operations/Hosting-and-Infrastructure), [Backup and Recovery](/Operations/Backup-and-Recovery) | Ethan | 2026-09-19 |
| 3 | Plugins, forms, PDF tools and third-party services | **GAP** — depends on WordPress access | [AFMA Website](/Systems/AFMA-Website) | Ethan | 2026-09-22 |
| 4 | Mailchimp, Power BI and other integrations | **PARTIAL** — neither appears anywhere in either codebase; email and SMTP paths are mapped | [Integrations and Data Flows](/Operations/Integrations-and-Data-Flows) | Ethan | 2026-09-22 |
| 5 | AFMA STATS application and hosting environment | **PARTIAL** — application fully mapped; hosting unknown | [AFMA STATS](/Systems/AFMA-STATS) | Ethan | 2026-09-19 |
| 6 | Feed Sales and Salmonella datasets and data flows | **PARTIAL** — application-side modules mapped; database schema and downstream flows unknown | [Integrations and Data Flows](/Operations/Integrations-and-Data-Flows) | Ethan / Henry | 2026-09-25 |
| 7 | Repositories, branches and deployment processes | **VERIFIED** — both repositories reviewed in full | [Repositories and Branches](/Development/Repositories-and-Branches), [Deployment Process](/Development/Deployment-Process) | Ethan | Done 2026-09-14 |
| 8 | Licences, subscriptions, renewals, costs, account ownership | **GAP** — one licensing exposure already identified in code | [Licences and Subscriptions](/Operations/Licences-and-Subscriptions) | Ethan | 2026-09-25 |
| 9 | Security, access-control, backup and recovery | **PARTIAL** — application-level findings verified and severe; infrastructure unknown | [Risk Register](/Risk-Register), [Backup and Recovery](/Operations/Backup-and-Recovery) | Ethan | 2026-09-19 |
| 10 | Existing support items, defects, outstanding developments | **GAP** — Azure DevOps boards not yet reviewed | [Open Questions](/Open-Questions) | Ethan | 2026-09-19 |
| 11 | Technical debt, legacy dependencies, undocumented functionality | **VERIFIED** — substantial findings recorded | [Risk Register](/Risk-Register), [Build and Environment](/Development/Build-and-Environment) | Ethan | Done 2026-09-14 |
| 12 | Future opportunities in AFMA's member-management environment | **PARTIAL** — MMS scope mapped, maturity assessed | [AFMA MMS](/Systems/AFMA-MMS) | Ethan | 2026-09-25 |

## What the repository review established

Reviewed 2026-09-14 against `AFMA-STAT` at `895ebb7` and `AFMA-MMS` at `f0d6bc8`. The findings that
change the shape of this account:

1. **Neither application can be built from a clean clone.** Both fail dependency restore. This is
   verified by running the restore, not inferred. See [Build and Environment](/Development/Build-and-Environment).
2. **A 58 MB SQL Server backup of the member database is committed to `AFMA-MMS` and is permanent in
   its history.** See [Risk Register](/Risk-Register), R2.
3. **Live production credentials are committed in plain text** to both repositories. See
   [Risk Register](/Risk-Register), R3.
4. **`AFMA-MMS` is a fork of `AFMA-STAT` with no shared history.** The two codebases have diverged
   and cannot be merged automatically. See [Repositories and Branches](/Development/Repositories-and-Branches).
5. **There is no automated build, no automated deployment and no test suite** across roughly 96,000
   lines of C#. Deployment is a manual publish from a developer's machine.
6. **Member password-reset emails are silently blind-copied to a third-party address.** See
   [Risk Register](/Risk-Register), R5.

## What has not been looked at at all

- Azure DevOps boards, pipelines, test plans, artifacts (all three projects)
- The AFMA Wiki's existing content
- Any hosting console: cPanel, the Windows/IIS server, DNS, the SQL Server instance
- The WordPress installation
- Mailchimp and Power BI accounts
- Any licence, invoice, renewal or billing record
- The `afmastats` and `afmawims` database schemas as they exist in production
