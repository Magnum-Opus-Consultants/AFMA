# Maintenance Mode

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Status** | **VERIFIED** 2026-09-14 from source |
| **Applies to** | `AFMA.Stats.Client` and `AFMA.Stats.Admin` |
| **Does not apply to** | `AFMA-MMS` / `AFMA.Web.Admin` — the middleware is not registered there |

The AFMA STATS portal can be taken offline cleanly, with a branded page and the correct HTTP status,
without a redeployment. This is a proper operational capability, built by Chris Delport in June 2026
(`5af0d88`, `a18fcab`).

## How it works

`MaintenanceMiddleware` sits early in the request pipeline. On every request:

1. If `MaintenanceSettings:Enabled` is `false`, the request passes through untouched.
2. If the request carries `?preview=<token>` matching `BypassToken`, the middleware sets a bypass
   cookie and redirects to the same URL with the token stripped from the query string.
3. If the request carries a valid bypass cookie, it passes through to the live site.
4. Otherwise the middleware returns **HTTP 503** with a `Retry-After` header and the branded
   maintenance page.

Token comparison is length-stable, so the token cannot be guessed character by character from response
timing. The bypass cookie is `HttpOnly`, `SameSite=Lax`, and `Secure` when the request is over HTTPS.
The heading and message are HTML-encoded before rendering.

Settings are read through `IOptionsMonitor`, so **changing `appsettings.json` takes effect without
restarting the application**.

## Settings

Section `MaintenanceSettings` in `appsettings.json`, bound to
`AFMA.Shared.Services.Maintenance.MaintenanceSettings`.

| Setting | Default in repository | Purpose |
|---|---|---|
| `Enabled` | `false` | Master switch |
| `Heading` | "AFMA Stats System Temporarily Unavailable" | Page headline |
| `Message` | AFMA-supplied copy, see below | Page body. Supports multiple paragraphs |
| `BypassToken` | *(set — treat as a live secret)* | Secret that grants bypass. **Blank disables bypass entirely** |
| `BypassCookieName` | `afma_maintenance_bypass` | Cookie name |
| `BypassQueryParameter` | `preview` | Query parameter carrying the token |
| `BypassCookieDays` | `7` | Cookie lifetime. `0` makes it a session cookie |
| `RetryAfterSeconds` | `3600` | Value of the `Retry-After` header |

The current `Message` is client-supplied copy:

> Please note that the AFMA Stats System is temporarily unavailable.
>
> We sincerely apologise for any inconvenience caused and appreciate your patience during this period.
>
> For any assistance or further information regarding the submission of monthly figures, please contact
> the AFMA office on +27 (0) 12 663 9097 or email petru@afma.co.za.

Do not change that wording without AFMA's agreement — it names an AFMA staff member and their phone
number, and members will act on it.

## Procedure — taking AFMA STATS offline

**Before you start:** check whether a submission deadline falls within the window. Taking the portal
down while members are submitting against a deadline is the most damaging thing that can be done to
this account operationally.

1. Confirm the window with AFMA and agree the message wording if it differs from the standard copy.
2. On the server, edit `appsettings.json` for the application concerned:
   ```json
   "MaintenanceSettings": { "Enabled": true }
   ```
   Do both `AFMA.Stats.Client` and `AFMA.Stats.Admin` unless only one is being worked on.
3. No restart is needed — `IOptionsMonitor` picks the change up.
4. Verify in a private browser window that you get the maintenance page and a 503.
5. Verify that `https://<site>/?preview=<token>` lets you through, so you can test the work.
6. Do the work.
7. Set `Enabled` back to `false`.
8. Verify the site is live in a private browser window — one that does not hold a bypass cookie.

**Step 8 matters.** Anyone who used the bypass is holding a cookie for seven days and will see the
live site whether or not maintenance mode is still on. Always confirm with a clean browser session.

## The `app_offline.htm` alternative

`AFMA-STAT/deploy/app_offline.htm` is the classic ASP.NET Core mechanism: drop the file into the
application root and IIS stops the application and serves that file for every request. Remove it and
the application starts again.

| | Maintenance mode | `app_offline.htm` |
|---|---|---|
| Application keeps running | Yes | No — it is shut down |
| Bypass for testing | Yes | No |
| Correct 503 and `Retry-After` | Yes | No — returns 503 without the header |
| Works during a file deployment | No — the files are being replaced | Yes |

Use maintenance mode for planned windows where you need to test before reopening. Use `app_offline.htm`
when replacing application files, because maintenance mode's own assemblies are among the files being
overwritten.

## Known gaps

| Gap | Action |
|---|---|
| The bypass token is committed to git and must be treated as compromised | Rotate as part of [Risk Register](/Risk-Register) R3. *Owner: Henry. Due: 2026-09-19.* |
| `AFMA-MMS` has no maintenance mode — the middleware is not registered in `AFMA.Web.Admin/Program.cs` | Port it. *Owner: Henry. Due: 2026-09-30.* |
| The maintenance page references `/assets/images/afma-full.png` with a fallback to `/img/afma-full.png` | Confirm one of those resolves on each deployed site, or the page shows a broken image to members. *Owner: Henry. Due: 2026-09-30.* |
| The page loads Google Fonts from a CDN | If the outage is network-related the font will not load. Cosmetic, but worth a local fallback. |
| No record is kept of maintenance windows | Log each one in the release history table on [Deployment Process](/Development/Deployment-Process) |
