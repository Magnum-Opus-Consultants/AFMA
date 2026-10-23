# Hosting and Infrastructure

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Status** | **GAP** — no infrastructure console access held. Everything below is either derived from committed configuration or blank pending access. |
| **Last reviewed** | 2026-09-14 |

## What the code tells us

**VERIFIED 2026-09-14**, from committed configuration. These are clues about the infrastructure, not
a description of it — committed values are development defaults in most cases.

| Host or endpoint | Role | Source |
|---|---|---|
| `WIN-MDOU42447UT` | SQL Server hosting `afmawims` | `AFMA-MMS/Website/AFMA.Web.Admin/appsettings.json` |
| `smtp.mailgun.org:587` | Outbound SMTP, authenticated, user `mailadmin@afma.eclick.co.za` | same file |
| `emailportal.resolvecloud.co.za` | SOAP/WCF email service (`EmailService.svc`) | `AFMA.Service.Communication/Connected Services/EmailServiceReference/ConnectedService.json` |
| `relay03.geckomail.co.za:25` | Unauthenticated SMTP relay, hard-coded | `AFMA.Service.Communication/EmailService.cs`, `SendSmtpNotification` |
| `afmastats.co.za` | STATS portal domain — `system@afmastats.co.za` is the sending address | `SMTPSettings:FromAddress` |
| `afma.co.za` | AFMA's own domain | `afma@afma.co.za`, `stats@afma.co.za`, `petru@afma.co.za` |
| `afma.eclick.co.za` | Mailgun sending domain | `SMTPSettings:Username` |

`WIN-MDOU42447UT` is a **default Windows machine name**. A production database server that was never
renamed suggests a machine provisioned ad hoc rather than to a standard. Worth establishing what it
actually is before assuming it is a managed server.

The applications are IIS-hosted ASP.NET Core (`Microsoft.NET.Sdk.Web`, framework-dependent, publish
profiles set `SelfContained=false`), so there is a Windows server with IIS and the ASP.NET Core 8
hosting bundle somewhere.

## Servers — to complete

Blocked on access A7 and A8. *Owner: Ethan. Due: 2026-09-19.*

| Item | STATS server | MMS server | Website (cPanel) |
|---|---|---|---|
| Provider and location | | | |
| Server name / hostname | | | |
| Operating system and version | | | |
| Specification — CPU, RAM, disk | | | |
| Who owns the contract | | | |
| Monthly cost | | | |
| Renewal date | | | |
| Who has administrative access | | | |
| Patching — who, how often | | | |
| Monitoring and alerting | | | |
| Support contract | | | |

## IIS configuration — to complete

| Site | Application pool | .NET version | Bindings | Physical path | Certificate |
|---|---|---|---|---|---|
| AFMA STATS — Client | | | | | |
| AFMA STATS — Admin | | | | | |
| AFMA MMS | | | | | |

Also capture: whether the sites share an app pool, the app pool identity and whether it is the account
used for SQL integrated security, idle timeout and recycling settings, and where the ASP.NET Core
stdout logs are written.

## Databases — to complete

Blocked on access A8.

| Item | `afmastats` | `afmawims` |
|---|---|---|
| Server instance | | `WIN-MDOU42447UT` (per config) |
| SQL Server version and edition | | |
| Database size | | |
| Recovery model | | |
| Collation | | |
| Logins and their permissions | | `afmawims_app` (per config) |
| Maintenance plans | | |
| Where the Serilog `Logs` table lives and how it is pruned | | |

Two things to resolve on first contact with the server:

1. **Database naming inconsistency.** `AFMA-STAT`'s admin configuration points its WIMS connection at
   a database called `afmaweb`; `AFMA-MMS` points at `afmawims`. Establish which exists, and whether
   both do.
2. **The Serilog sink has `autoCreateSqlTable: true`** and writes to a `Logs` table in the
   application database at minimum level `Warning`. Nothing prunes it. Check its size — an unbounded
   log table inside the application database is a slow-motion disk problem.

## Domains, DNS and SSL — to complete

Blocked on access A6. *Owner: Ethan. Due: 2026-09-19.*

| Domain | Registrar | Registrant / owner | Expiry | Auto-renew | DNS hosted at |
|---|---|---|---|---|---|
| `afma.co.za` | | | | | |
| `afmastats.co.za` | | | | | |
| `afma.eclick.co.za` | | | | | |
| *(others to be discovered)* | | | | | |

### DNS records to record

At minimum, for each domain: A and AAAA records, CNAMEs, MX, and the mail authentication records —
**SPF, DKIM and DMARC**. Mail authentication matters here: AFMA sends member notifications through
Mailgun from `system@afmastats.co.za`, and historically through a Gecko relay and a Resolve portal.
Several sending paths across several domains is exactly the configuration that produces deliverability
failures, and members not receiving submission reminders is a visible client problem.

### Certificates

| Domain | Issuer | Expiry | Renewal — automatic or manual | Who is alerted |
|---|---|---|---|---|
| | | | | |

**GAP** — nobody currently receives certificate expiry warnings for AFMA's domains, as far as we know.
Establish this and fix it before anything else on this page. An expired certificate takes the member
portal down with no warning. *Owner: Ethan. Due: 2026-09-19.*

## Immediate actions

1. Obtain server and SQL access (A7, A8). *Owner: Ethan. Due: 2026-09-19.*
2. Establish certificate expiry dates for every AFMA domain and get alerting onto them.
   *Owner: Ethan. Due: 2026-09-19.*
3. Confirm what `WIN-MDOU42447UT` actually is and who pays for it. *Owner: Ethan. Due: 2026-09-19.*
4. Check the size of the Serilog `Logs` table. *Owner: Henry. Due: 2026-09-25.*
5. Complete every table on this page. *Owner: Ethan with Henry. Due: 2026-10-03.*
