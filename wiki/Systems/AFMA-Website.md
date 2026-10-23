# AFMA Website

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Repository** | `https://eclick-dev@dev.azure.com/eclick-dev/AFMA-WEB/_git/AFMA-WEB` |
| **Status** | **GAP — this system is entirely undocumented and outside our source control** |
| **Last reviewed** | 2026-09-14 |

## What we know

**VERIFIED 2026-09-14:** the `AFMA-WEB` repository in Azure DevOps is **empty**. No code has ever been
pushed to it. It has no branches, no commits and no README — Azure DevOps still shows the
"AFMA-WEB is empty. Add some code!" initialisation screen.

**VERIFIED 2026-09-14:** neither `AFMA-STAT` nor `AFMA-MMS` contains any reference to WordPress,
cPanel, or any integration with the public website. The STATS and MMS applications and the website
share no code.

Everything else on this page is a gap.

## What this means

The AFMA public website — a WordPress site on cPanel hosting, per the account brief — exists **only on
its hosting**. There is no copy of its theme, its plugins, its configuration or its content in any
repository we control. If that hosting were lost, the site would be reconstructed from a backup we
have not verified exists, or not at all.

This is the same class of problem as [Risk Register](/Risk-Register) R1 — a system that cannot be
rebuilt from what we hold — but more severe, because here we hold nothing at all.

## What must be established

Each of these is blocked on access A4, A5 and A6 — see [Access Register](/Access-Register).

### WordPress

| Item | Status | Owner | Due |
|---|---|---|---|
| WordPress version, and whether it is current | **GAP** | Ethan | 2026-09-22 |
| Active theme — name, version, whether it is a child theme, whether it has been customised in place | **GAP** | Ethan | 2026-09-22 |
| Full plugin inventory — name, version, licence, whether premium, renewal date | **GAP** | Ethan | 2026-09-22 |
| Forms — which plugin, which forms exist, where submissions go | **GAP** | Ethan | 2026-09-22 |
| PDF tooling — which plugin or service, what it generates | **GAP** | Ethan | 2026-09-22 |
| Members-only or gated content, and how access is controlled | **GAP** | Ethan | 2026-09-22 |
| Any custom code — `functions.php` edits, mu-plugins, custom post types | **GAP** | Ethan | 2026-09-25 |
| Admin user list and roles | **GAP** | Ethan | 2026-09-22 |
| Update policy — who applies core, theme and plugin updates, and how often | **GAP** | Ethan | 2026-09-25 |

### cPanel and hosting

| Item | Status | Owner | Due |
|---|---|---|---|
| Hosting provider, account owner, package, cost, renewal date | **GAP** | Ethan | 2026-09-22 |
| PHP version and configured limits | **GAP** | Ethan | 2026-09-22 |
| MySQL database name, version, size | **GAP** | Ethan | 2026-09-22 |
| Email accounts and forwarders on the domain | **GAP** | Ethan | 2026-09-22 |
| Cron jobs configured in cPanel | **GAP** | Ethan | 2026-09-22 |
| FTP/SFTP and SSH accounts | **GAP** | Ethan | 2026-09-22 |
| cPanel backup configuration and retention | **GAP** | Ethan | 2026-09-22 |

### Domains, DNS and SSL

Recorded on [Hosting and Infrastructure](/Operations/Hosting-and-Infrastructure) once access lands.
At minimum: `afma.co.za` and `afmastats.co.za` — both appear in application configuration — plus
`afma.eclick.co.za`, which appears as the Mailgun sending domain.

### Integrations

| Item | Status | Note |
|---|---|---|
| Mailchimp | **GAP** | No trace in either application codebase. If it integrates with anything, it integrates with the website. See [Integrations and Data Flows](/Operations/Integrations-and-Data-Flows) |
| Power BI | **GAP** | Likewise. The most likely data source is the anonymous `/external/feedsales` endpoint on the STATS admin site — see [Risk Register](/Risk-Register) R10 |
| Google Analytics or similar | **GAP** | Unknown |
| Any link between the website and the STATS portal | **GAP** | No shared code. Likely a plain hyperlink, but unconfirmed |

## First actions

1. Obtain cPanel and WordPress administrator access (A4, A5). *Owner: Ethan. Due: 2026-09-19.*
2. Take a full manual backup — files and database — before touching anything, and store it where we
   control it. *Owner: Ethan. Due: same day as access.*
3. Get the site into source control. A WordPress site does not belong in git wholesale, but the theme,
   any custom plugins and the configuration do. Populate the empty `AFMA-WEB` repository with those.
   *Owner: Henry. Due: 2026-10-03.*
4. Complete the inventory tables above.
5. Only then assess update and security posture — WordPress sites that nobody has inventoried are
   usually carrying out-of-date plugins.
