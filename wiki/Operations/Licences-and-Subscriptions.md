# Licences and Subscriptions

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Status** | **GAP** — no licence or billing record has been located. One exposure already identified. |
| **Last reviewed** | 2026-09-14 |

## The one thing already known

**EPPlus 6.2.10 is configured for non-commercial use in both applications.** See
[Risk Register](/Risk-Register) **R7**.

```json
"EPPlus": { "ExcelPackage": { "LicenseContext": "NonCommercial" } }
```

EPPlus 5 and later is dual-licensed under the Polyform Noncommercial licence. Declaring
`NonCommercial` in software supplied commercially to a paying client is a licence assertion that does
not match the use. EPPlus generates the Excel exports in STATS reporting and MMS member export, so it
cannot simply be removed.

Resolve this before anything else on this page. *Owner: Ethan, with Pieter. Due: 2026-10-03.*

## Software licences — to complete

| Item | Version | Licence type | Commercial licence held? | Cost | Renewal | Who pays | Evidence |
|---|---|---|---|---|---|---|---|
| **EPPlus** | 6.2.10 | Polyform Noncommercial / commercial | **Unknown — declared NonCommercial in config** | | | | |
| `Alpha.Services.*` | 9.0.18 / 9.0.20 | Private, author "JJ Swart" | **Unknown — no licence terms held** | | | | |
| `Resolvetech.NetCore.Common` | 1.0.3.1004 | Private | **Unknown — no licence terms held** | | | | |
| Visual Studio | 2022 | | | | | | |
| Windows Server | | | | | | | |
| SQL Server | | | | | | | |
| Azure DevOps | | | | | | | |
| WordPress premium plugins | | | | | | | |
| WordPress theme | | | | | | | |

The two private package families matter beyond cost. We do not hold, and have not seen, **any licence
terms** for code that AFMA's applications cannot run without. Establishing our right to use,
redistribute and — if necessary — maintain that code is part of question Q1 on
[Open Questions](/Open-Questions). It is a contractual question as much as a technical one.

Open-source dependencies under permissive licences (NHibernate, AutoMapper, Serilog, FluentNHibernate,
X.PagedList, DNTCaptcha, Swashbuckle, the bundled front-end libraries) do not need individual rows
here unless a specific one proves to have restrictive terms.

## Subscriptions and services — to complete

Blocked on access A14. *Owner: Ethan. Due: 2026-09-25.*

| Service | Account owner | Plan | Cost | Billing cycle | Renewal date | Payment method | Who is alerted |
|---|---|---|---|---|---|---|---|
| Hosting — STATS/MMS server | | | | | | | |
| Hosting — cPanel / website | | | | | | | |
| Domain `afma.co.za` | | | | | | | |
| Domain `afmastats.co.za` | | | | | | | |
| SSL certificates | | | | | | | |
| Mailgun | | | | | | | |
| Resolve email portal | | | | | | | |
| Mailchimp | | | | | | | |
| Power BI | | | | | | | |
| Azure DevOps `eclick-dev` | | | | | | | |
| Backup service, if any | | | | | | | |
| Monitoring, if any | | | | | | | |

## Account ownership — the question that matters most

For each service above, establish **in whose name the account stands**: AFMA's, ours, or an
individual's.

This is the single most important thing on this page. An account registered to an individual rather
than to an organisation is a continuity risk regardless of who pays for it — if that person is
unreachable, the service cannot be recovered, transferred or cancelled. Given the account brief's
concern with continuity, and given that we have already found a personal Gmail address as the author
of every commit in one repository and a private NuGet package attributed to an individual, this needs
checking rather than assuming.

| Question | Owner | Due |
|---|---|---|
| Which accounts stand in AFMA's name? | Ethan | 2026-09-25 |
| Which stand in ours? | Ethan | 2026-09-25 |
| Which stand in an individual's name? | Ethan | 2026-09-25 |
| Which have a single point of failure for recovery — one email address, one phone number, one person? | Ethan | 2026-09-25 |
| Where is billing directed, and who reconciles it? | Ethan, with Pieter | 2026-09-25 |

## Renewal calendar

**GAP** — no renewal calendar exists. Nobody is currently alerted before an AFMA domain, certificate
or subscription lapses.

Build one as the tables above are completed, with a reminder at 60 and 30 days for every dated item.
Domain and certificate expiry belong on it first — both take AFMA's systems down with no warning and
both are trivially preventable. *Owner: Ethan. Due: 2026-10-03.*

| Item | Expiry / renewal | 60-day alert to | 30-day alert to |
|---|---|---|---|
| | | | |

## Cost summary

To be completed once the tables above are populated. AFMA's total annual cost of ownership is
currently unknown to us, which makes it impossible to have an informed commercial conversation about
the account — including about the member-management opportunity the brief identifies.

*Owner: Ethan, with Pieter. Due: 2026-10-03.*
