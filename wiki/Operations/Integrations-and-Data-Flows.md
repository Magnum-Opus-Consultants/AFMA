# Integrations and Data Flows

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Status** | **PARTIAL** — email paths verified from source; Mailchimp and Power BI not located |
| **Last reviewed** | 2026-09-14 |

## Email — three separate sending paths

**VERIFIED 2026-09-14.** The applications can send mail three different ways. Which are live is not
established. This matters: three sending paths across three domains is how deliverability problems
start, and AFMA members not receiving submission reminders is a visible client failure.

### Path 1 — SMTP via the Alpha notification service

The main path. `AddAfmaNotificationServices` configures
`Alpha.Services.Notifications` from the `SMTPSettings` section.

| | `AFMA-STAT` (committed values) | `AFMA-MMS` (committed values) |
|---|---|---|
| Host / port | *(blank)* | `smtp.mailgun.org` / `587` |
| Username | *(blank)* | `mailadmin@afma.eclick.co.za` |
| Auth | `false` | `true` |
| Debug | `true` | `false` |
| From | `AFMA <system@afmastats.co.za>` | `AFMA <system@afmastats.co.za>` |
| Debug recipient | `stats@afma.co.za` | `stats@afma.co.za` |

`AFMA-STAT`'s committed values are development placeholders; production values are applied on the
server by an undocumented means. `AFMA-MMS`'s look production-shaped — see
[Risk Register](/Risk-Register) R3.

When `Debug` is `true`, mail is redirected to `DebugEmail` instead of the real recipient. **Confirm
`Debug` is `false` in production** or members receive nothing and `stats@afma.co.za` receives
everything. *Owner: Henry. Due: 2026-09-19.*

### Path 2 — Resolve email portal (SOAP/WCF)

`AFMA.Service.Communication` holds a WCF client generated against
`https://emailportal.resolvecloud.co.za/EmailService.svc`, authenticated with the
`WebApplicationSettings:EmailApiKey`. It exposes queue, sent and error listings plus message sending.

This looks like a legacy managed-email platform predating Mailgun. Whether it is still in use is
question Q7 on [Open Questions](/Open-Questions).

Note the defect: `EmailService.ProcessAndSendBatch` **discards the recipients, subject and body it is
given** and substitutes test values addressed to `jacques@greengecko.co.za`. See
[Risk Register](/Risk-Register) R5.

### Path 3 — direct SMTP relay, hard-coded

`EmailService.SendSmtpNotification` opens an unauthenticated connection to
`relay03.geckomail.co.za:25` and sends as `afma@afma.co.za`. Host, port and sender are all hard-coded
— no configuration involved.

An open relay reachable without credentials is worth checking on its own merits.
*Owner: Ethan. Due: 2026-09-22.*

### Third-party BCC

`operations@resolvetech.co.za` is hard-coded as a BCC recipient on password resets and member
notifications. See [Risk Register](/Risk-Register) **R5** — this is the most significant data-flow
finding on the account.

## Mailchimp

**GAP.** `mailchimp` appears nowhere in either codebase. The only matches in a full-text search were
inside the bundled Font Awesome icon library.

If Mailchimp is in use for AFMA, it integrates through the WordPress site, or it is operated manually
by AFMA staff, or it is connected to something we have not yet seen. It is not wired into STATS or MMS.

To establish, blocked on access A11:

| Item | Owner | Due |
|---|---|---|
| Who owns the Mailchimp account, and who pays | Ethan | 2026-09-22 |
| Which audiences exist and how they are populated | Ethan | 2026-09-22 |
| Whether member data flows from MMS or the STATS database into Mailchimp, and by what means | Ethan | 2026-09-22 |
| Whether that flow is manual — a CSV export someone runs — and if so, who runs it | Ethan | 2026-09-22 |
| Overlap with MMS distribution groups and notification batches, which cover similar ground | Ethan / Henry | 2026-09-30 |

That last row is worth thinking about commercially. MMS already models distribution groups, member
users and notification templates. If AFMA is also paying for Mailchimp and maintaining lists by hand,
there is either a consolidation opportunity or a duplication problem.

## Power BI

**GAP.** `powerbi` and `power bi` appear nowhere in either codebase. No dataset definitions, no
gateway configuration, no embedding, no API calls.

The most likely mechanism, and the first thing to check:
`AFMA.Stats.Admin/Controllers/ExternalController` is decorated `[AllowAnonymous]` and serves
aggregated feed-sales and raw-material dashboard data at `/external/feedsales` and
`/external/rawmaterial`, cached in memory. An endpoint named "External", returning aggregated
statistics, deliberately open to anonymous callers, is very plausibly what a Power BI dataset is
pointed at.

Question Q4 on [Open Questions](/Open-Questions), and [Risk Register](/Risk-Register) R10.

To establish, blocked on access A12:

| Item | Owner | Due |
|---|---|---|
| Which Power BI workspace, who owns it, what licence tier | Ethan | 2026-09-22 |
| What the datasets connect to — the `/external/` endpoints, direct SQL, or a manual export | Ethan | 2026-09-22 |
| Refresh schedule, and whether an on-premises data gateway is involved | Ethan | 2026-09-22 |
| Which reports exist and who consumes them | Ethan | 2026-09-25 |
| Whether any report is published publicly | Ethan | 2026-09-22 |

## Feed Sales — data flow

**PARTIAL.** The application side is mapped. What happens to the data afterwards is not.

```
Member logs into AFMA.Stats.Client
        │
        ▼
FeedsalesController — submission forms
  finished product and raw material
        │
        │  InvariantDecimalModelBinderProvider
        │  accepts both "1,5" and "1.5"   ← July 2026 fix, AFMA-STAT only
        ▼
FeedsalesLayer  (AFMA.ApplicationLayer)
        │
        ▼  NHibernate
   afmastats database
        │
        ├──▶ AFMA.Stats.Admin — FeedsalesReportController, Areas/Feedsales
        │      AFMA staff review and report. Excel export via EPPlus
        │
        ├──▶ ExternalApiService — in-memory cached dashboard aggregate
        │      served anonymously at /external/feedsales    ← likely Power BI source
        │
        └──▶ Email notification to stats@afma.co.za on submission
               BCC operations@resolvetech.co.za   ← see R5
```

**GAP** — the business rules are undocumented: submission periods and deadlines, validation rules,
how figures aggregate to industry totals, who may reopen a closed period, how corrections are handled.
This is the knowledge most at risk on the account. *Owner: Ethan with Henry. Due: 2026-10-10.*

The decimal-separator handling deserves particular care. South African locale input uses a comma;
members were evidently entering both. A regression there would silently change submitted industry
figures rather than fail visibly. It belongs in the first characterisation tests
([Risk Register](/Risk-Register) R9).

## Salmonella — data flow

Same shape, one module across:

```
Member → AFMA.Stats.Client / SalmonellaController
       → SalmonellaLayer → afmastats (NHibernate)
       → AFMA.Stats.Admin / SalmonellaReportController, Areas/Salmonella
       → notification email (BCC operations@resolvetech.co.za — see R5)
```

Unlike Feed Sales, Salmonella has **no external/anonymous endpoint**. Salmonella results are member
food-safety test data, so that is almost certainly correct — but confirm it is deliberate rather than
incidental. *Owner: Ethan. Due: 2026-09-22.*

**GAP** — the same business-rule gap applies: testing regime, reporting frequency, thresholds, what
AFMA publishes from it and at what aggregation. *Owner: Ethan with Henry. Due: 2026-10-10.*

## Other statistics modules

Mycotoxin, Industry Profile and Finished Product follow the same client → layer → `afmastats` →
admin-reporting pattern. `MycotoxinLayer` also carries the hard-coded BCC (R5).

Industry Profile is the most complex by view structure — costs, fleet, production, staffing,
administration and marketing breakdowns. Its rules are undocumented. *Owner: Henry. Due: 2026-10-10.*

## Summary of external dependencies

| Dependency | Type | Criticality | Documented | Access held |
|---|---|---|---|---|
| Mailgun | SMTP | High — all member email | No | **No** (A9) |
| Resolve email portal | SOAP API | Unknown — may be legacy | No | **No** (A10) |
| `relay03.geckomail.co.za` | SMTP relay | Unknown — hard-coded | No | **No** |
| `Alpha.Services.*` packages | Private NuGet | **Critical — nothing builds without it** | No | **No** ([R1](/Risk-Register)) |
| `Resolvetech.NetCore.Common` | Private NuGet | **Critical — nothing builds without it** | No | **No** ([R1](/Risk-Register)) |
| EPPlus | Library | High — all Excel export | No | Licence unclear ([R7](/Risk-Register)) |
| Mailchimp | SaaS | Unknown | No | **No** (A11) |
| Power BI | SaaS | Unknown | No | **No** (A12) |
| Google Fonts CDN | CDN | Low — maintenance page only | This wiki | n/a |

Nine external dependencies. **None** of them documented before today, and access held to none.
