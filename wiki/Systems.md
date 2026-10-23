# Systems

The three systems that make up the AFMA technical landscape.

| System | Status of this documentation | Page |
|---|---|---|
| **AFMA STATS** | Application layer documented from source. Hosting unknown. | [AFMA STATS](/Systems/AFMA-STATS) |
| **AFMA MMS** | Application layer documented from source. Live status unknown. | [AFMA MMS](/Systems/AFMA-MMS) |
| **AFMA Website** | Not documented. No access, no source control. | [AFMA Website](/Systems/AFMA-Website) |

## How they relate

```
                 ┌──────────────────────────────┐
                 │  AFMA website (WordPress)     │
                 │  cPanel hosting               │
                 │  Public site, forms, content   │
                 └──────────────┬───────────────┘
                                │  linked, not integrated
                                │  (no shared code found)
   ┌────────────────────────────┴─────────────────────────────┐
   │                                                          │
┌──▼───────────────────────────┐        ┌────────────────────▼──────────┐
│  AFMA STATS  (repo AFMA-STAT)│        │  AFMA MMS  (repo AFMA-MMS)    │
│                              │        │                               │
│  AFMA.Stats.Client           │        │  AFMA.Web.Admin               │
│    member submission portal  │        │    member management          │
│  AFMA.Stats.Admin            │        │  + a copy of Stats.Client     │
│    admin and reporting       │        │    and Stats.Admin            │
└──────────┬───────────────────┘        └────────────┬──────────────────┘
           │                                          │
           │  NHibernate                              │  NHibernate
           ▼                                          ▼
   ┌───────────────┐                          ┌───────────────┐
   │  afmastats    │                          │  afmawims     │
   │  SQL Server   │                          │  SQL Server   │
   └───────────────┘                          └───────────────┘
                    both on WIN-MDOU42447UT
                    (per committed configuration — unverified)
```

**Important:** `AFMA-MMS` is a fork of `AFMA-STAT`, not a separate product built alongside it. It
contains its own copies of `AFMA.Stats.Client` and `AFMA.Stats.Admin` in addition to `AFMA.Web.Admin`.
Which of those copies is deployed anywhere, if any, is unknown. See [Risk Register](/Risk-Register) R8.

The `afmaweb` / `afmawims` database naming is inconsistent across configuration files — `AFMA-STAT`'s
admin `appsettings.json` points its WIMS connection at a database called `afmaweb`, while `AFMA-MMS`
points at `afmawims`. This needs resolving against the live server. *Owner: Ethan. Due: 2026-09-19.*
