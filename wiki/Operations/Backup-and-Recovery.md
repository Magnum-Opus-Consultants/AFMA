# Backup and Recovery

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Status** | **GAP** — no backup arrangement has been verified for any AFMA system |
| **Last reviewed** | 2026-09-14 |

## Current position

We cannot currently answer the most basic question about this account: **if AFMA's member database
were lost this afternoon, what would we restore from, and how far back would it take us?**

Nothing in either repository configures, schedules or documents a backup. The only backup artefact
that exists anywhere we can see is `afmawims_10072026.bak` — a manual SQL Server backup taken on
2026-07-10 and then **committed into the `AFMA-MMS` git repository**, where it remains permanently.
See [Risk Register](/Risk-Register) R2.

That single fact tells us most of what we need to know about the current state of backup practice on
this account: backups are taken by hand, when someone needs one, and stored wherever is convenient.

## What must be established

Question Q6 on [Open Questions](/Open-Questions), blocked on access A13. *Owner: Ethan. Due: 2026-09-19.*

### Databases

| Item | `afmastats` | `afmawims` | AFMA website MySQL |
|---|---|---|---|
| Is a backup running at all? | | | |
| Full backup — schedule and time | | | |
| Differential backup — schedule | | | |
| Transaction log backup — schedule | | | |
| Recovery model | | | |
| Where backups are written | | | |
| Offsite or second-location copy | | | |
| Retention period | | | |
| Encrypted at rest? | | | |
| Who is alerted on failure | | | |
| **Date a restore was last tested** | | | |

The last row is the one that matters. A backup nobody has restored is a hypothesis.

### Application and file backups

| Item | Status |
|---|---|
| Member-uploaded documents — MMS handles member documents, committee documents and certificates. Where are the files stored, and are they backed up? | **GAP** |
| Batch email attachments | **GAP** |
| IIS configuration | **GAP** |
| Server-side `appsettings.json` production values — currently the only copy of the production configuration | **GAP** |
| WordPress files and uploads | **GAP** |

The fourth row deserves attention. Production connection strings and secrets are applied on the server
by an undocumented means (see [Deployment Process](/Development/Deployment-Process)). If those files
are not backed up, the production configuration exists in exactly one place and is not recoverable.

## Recovery targets — to agree with AFMA

Not yet discussed with the client. Question Q17 on [Open Questions](/Open-Questions).

| System | RPO — acceptable data loss | RTO — acceptable downtime | Agreed with | Date |
|---|---|---|---|---|
| AFMA STATS — member portal | | | | |
| AFMA STATS — admin | | | | |
| AFMA MMS | | | | |
| AFMA website | | | | |

These will not be uniform. The member submission window is the period when STATS downtime hurts most —
if members cannot submit against a deadline, that is an immediate and visible client problem.
Establish when that window falls and set the targets around it.

## Recovery procedures — to write

None of these exist. Each needs writing and then **testing**, because an untested procedure is not a
procedure.

| Scenario | Procedure | Tested |
|---|---|---|
| Restore a single database to a point in time | **Not written** | Never |
| Rebuild the STATS application server from scratch | **Not written** | Never |
| Rebuild the MMS application server from scratch | **Not written** | Never |
| Restore the WordPress site | **Not written** | Never |
| Recover from ransomware or a compromised server | **Not written** | Never |
| Recover a single member's deleted data | **Not written** | Never |

Note the dependency: **the server rebuild procedures cannot be written until
[Risk Register](/Risk-Register) R1 is fixed.** We currently cannot build the applications from source,
so "rebuild the server" has no answer beyond "restore a machine image, if one exists". That is worth
stating plainly, because it means R1 is not only a development problem — it is a disaster-recovery
problem.

## Where backups should live

**Decision required**, as part of resolving R2. Database backups must not go into git. They need a
location that is:

- Outside the application server, so a server loss does not take the backups with it
- Access-controlled and auditable, given the contents are member personal information
- Encrypted at rest
- Retained on an agreed schedule, with old copies actually deleted

*Owner: Ethan, decision with Pieter and Henry. Due: 2026-09-26.*

## Immediate actions

1. Establish whether **any** automated backup is currently running on the AFMA databases. This is the
   single most urgent unanswered question on the account. *Owner: Ethan. Due: 2026-09-19.*
2. If none is running, get one running before anything else on this page.
   *Owner: Henry. Due: immediately on confirmation.*
3. Take a verified, offsite copy of both databases as a baseline, stored properly rather than in git.
   *Owner: Henry. Due: 2026-09-22.*
4. Perform one test restore and record the date and result. *Owner: Henry. Due: 2026-09-30.*
5. Complete the tables on this page. *Owner: Ethan. Due: 2026-10-03.*
