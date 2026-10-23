# Open Questions

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Last reviewed** | 2026-09-14 |

Questions that cannot be answered from the code and need a person. Each has a named person to ask and
a date. When answered, the answer moves onto the relevant page and the question is struck through
here with the date and who answered.

The brief is to work with Henry, and to draw on Chris only where historical context, system
architecture or unresolved technical dependencies genuinely require it. The questions below are
sorted on that basis.

## For Chris — genuinely require historical or architectural context

| # | Question | Why it needs Chris | Due |
|---|---|---|---|
| Q1 | Where can we obtain `Resolvetech.NetCore.Common 1.0.3.1004` and `Alpha.Services.* 9.0.20`? Do we have source, or only binaries? Under what licence? | Unresolved technical dependency — neither application builds without this. See [Risk Register](/Risk-Register) R1 | 2026-09-19 |
| Q2 | What is the actual deployment procedure for AFMA STATS and for MMS — how do published files reach the server, and who can do it? | Undocumented; no pipeline exists. See R4 | 2026-09-19 |
| Q3 | What is `operations@resolvetech.co.za` BCC'd on password resets for, and did AFMA agree to it? | Historical decision with a live data-protection consequence. See R5 | 2026-09-19 |
| Q4 | What consumes `/external/feedsales` and `/external/rawmaterial` on the Admin site, and is anonymous access intended? | Architectural intent; likely the Power BI feed. See R10 | 2026-09-19 |
| Q5 | Why does `AFMA-MMS` exist as a separate fork rather than a branch or a shared solution? What was the intended end state? | Architectural history; determines how we resolve R8 | 2026-09-22 |
| Q6 | Where do AFMA's database backups run, to what schedule, and to where? Has a restore ever been tested? | No backup configuration exists in code. See [Backup and Recovery](/Operations/Backup-and-Recovery) | 2026-09-19 |
| Q7 | Is the Resolve email portal (`emailportal.resolvecloud.co.za`) still in use, or has Mailgun replaced it? | Two email paths exist in code with no indication which is live | 2026-09-22 |
| Q8 | Is `WIN-MDOU42447UT` a dedicated server, a shared one, or a VM — and who owns and pays for it? | Hosting ownership; a default Windows machine name suggests no formal provisioning | 2026-09-19 |

## For Henry — from 2026-09-21

| # | Question | Due |
|---|---|---|
| Q9 | What support items, defects and outstanding developments are currently open on the AFMA boards? | 2026-09-23 |
| Q10 | What is currently in flight that a handover must not interrupt? | 2026-09-23 |
| Q11 | Which environments exist beyond production — is there a test or staging environment for STATS or MMS? | 2026-09-23 |
| Q12 | Who currently holds production access, and to what? | 2026-09-23 |
| Q13 | Is `AFMA-MMS` live, in development, or piloted with members? Nothing in the repository says. | 2026-09-23 |

## For AFMA — via the agreed client contact route

Nothing on this list goes to the client until the account communication route is confirmed. See
[Handover Plan](/Handover/Handover-Plan).

| # | Question | Due |
|---|---|---|
| Q14 | Who at AFMA owns each system, and who may authorise change? | 2026-09-25 |
| Q15 | Which licences and subscriptions does AFMA hold directly rather than through us, and what renews when? | 2026-09-25 |
| Q16 | What are AFMA's expectations for the member-management environment over the next 12 months? | 2026-10-03 |
| Q17 | What availability does AFMA expect around the monthly statistics submission window? | 2026-09-25 |

## For internal decision — Ethan with Pieter

| # | Question | Due |
|---|---|---|
| Q18 | Is the member database in git history (R2) a reportable personal-information incident under POPIA? | 2026-09-19 |
| Q19 | Has a commercial EPPlus licence been bought, and if not, do we buy or migrate? (R7) | 2026-10-03 |
| Q20 | Which credential store do we adopt for this account? | 2026-09-26 |
| Q21 | What is the budget and appetite for remediating R1, R2, R3 and R6 — and is it billable to AFMA, absorbed, or deferred? | 2026-10-03 |

## Answered

*Nothing yet. Entries move here with the answer, the date and who gave it.*
