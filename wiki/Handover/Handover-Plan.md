# Handover Plan

| | |
|---|---|
| **Owner** | Ethan Sevenster |
| **Created** | 2026-09-14 |
| **Covers** | 2026-09-14 to 2026-10-31 |

## Constraint that shapes the plan

Henry is away until **2026-09-21**. That is a week of limited execution capacity, and it is better
spent than wasted.

The week before Henry returns is used for the work that needs no second person: securing access,
establishing facts, writing them down, and setting up how this account will be run. The result is
that when Henry is available, he starts from a written picture rather than a blank page — and his
time goes on execution rather than on explaining the landscape to someone who has not looked at it
yet.

Chris is drawn on only where historical context, system architecture or unresolved technical
dependencies genuinely require it. The questions reserved for him are listed on
[Open Questions](/Open-Questions), gathered into one set rather than dribbled out.

## Phase 1 — Establish the picture (2026-09-14 to 2026-09-20)

**Objective:** know what exists, hold the access to see it, and have it written down.

**Done, 2026-09-14.** Both repositories reviewed in full. This wiki created. Findings recorded in the
[Risk Register](/Risk-Register). Three critical issues established with evidence:

- Neither application builds from a clean clone (R1)
- A member database backup sits permanently in git history (R2)
- Live production credentials are committed in plain text (R3)

**Remaining this week:**

| # | Task | Owner | Due |
|---|---|---|---|
| 1.1 | Publish this wiki to the AFMA Wiki in Azure DevOps | Ethan | 2026-09-16 |
| 1.2 | Secure Azure DevOps access — wiki contribute, boards, pipelines, artifacts, Advanced Security | Ethan | 2026-09-16 |
| 1.3 | Establish who has had read access to `AFMA-MMS`, given R2 | Ethan | 2026-09-16 |
| 1.4 | Raise R2 with Pieter — POPIA assessment | Ethan | 2026-09-19 |
| 1.5 | Send Chris the consolidated question set (Q1–Q8) | Ethan | 2026-09-16 |
| 1.6 | Request infrastructure access — server, SQL, cPanel, WordPress, DNS, Mailgun | Ethan | 2026-09-19 |
| 1.7 | Establish whether **any** database backup is running | Ethan | 2026-09-19 |
| 1.8 | Establish certificate expiry dates and get alerting on them | Ethan | 2026-09-19 |
| 1.9 | Review Azure DevOps boards across all three projects | Ethan | 2026-09-19 |
| 1.10 | Set up the account communication route and cadence | Ethan | 2026-09-19 |
| 1.11 | Prepare the Henry briefing pack from this wiki | Ethan | 2026-09-20 |

Items 1.7 and 1.8 are the two that could turn into an incident without warning. They come first.

## Phase 2 — Stop the bleeding (2026-09-21 to 2026-09-30)

**Objective:** close the critical risks. Henry available from 2026-09-21.

| # | Task | Owner | Due | Risk |
|---|---|---|---|---|
| 2.1 | Brief Henry against this wiki, agree ownership of each item | Ethan + Henry | 2026-09-22 | — |
| 2.2 | **Rotate every credential committed to git** | Henry | 2026-09-19 | R3 |
| 2.3 | Obtain the missing private NuGet packages | Ethan via Chris | 2026-09-19 | R1 |
| 2.4 | Stand up an Azure Artifacts feed; publish the packages; repoint both `NuGet.config` | Henry | 2026-09-30 | R1 |
| 2.5 | **Prove a clean-machine build of both solutions** | Henry | 2026-09-30 | R1 |
| 2.6 | Purge the database backup from `AFMA-MMS` history; force-push; re-clone everywhere | Henry | 2026-09-26 | R2 |
| 2.7 | Merge `.gitignore` rules across both repositories; remove tracked `.vs/` | Henry | 2026-09-26 | R2, R12 |
| 2.8 | Move secrets out of `appsettings.json`; commit sanitised placeholders | Henry | 2026-09-26 | R3 |
| 2.9 | Enable secret scanning on both repositories | Ethan | 2026-09-26 | R3 |
| 2.10 | Set branch policies on `master` — PR required, one reviewer, linked work item | Ethan | 2026-09-26 | — |
| 2.11 | Resolve the `operations@resolvetech.co.za` BCC question and act on the answer | Ethan → Henry | 2026-09-26 | R5 |
| 2.12 | Take a verified offsite baseline backup of both databases | Henry | 2026-09-22 | — |
| 2.13 | Script both production schemas and commit them | Henry | 2026-09-30 | R11 |
| 2.14 | Perform and record one test restore | Henry | 2026-09-30 | — |
| 2.15 | Delete the orphaned net6.0 project; pin floating package versions | Henry | 2026-09-26 | R6 |

**2.5 is the milestone that matters.** Until someone who has never built these applications can clone
and build them from this wiki alone, the account is still dependent on one person's machine, and
nothing else in this plan is secure.

## Phase 3 — Complete the landscape (2026-10-01 to 2026-10-17)

**Objective:** every area of the [Technical Landscape](/Technical-Landscape) review verified, not
merely started.

| # | Task | Owner | Due |
|---|---|---|---|
| 3.1 | Complete the hosting, IIS and database inventory | Ethan + Henry | 2026-10-03 |
| 3.2 | Complete domains, DNS, SSL and mail authentication records | Ethan | 2026-10-03 |
| 3.3 | Get the WordPress site inventoried and into `AFMA-WEB` | Henry | 2026-10-03 |
| 3.4 | Locate and document Mailchimp and Power BI | Ethan | 2026-09-25 |
| 3.5 | Complete licences, subscriptions, costs and account ownership | Ethan + Pieter | 2026-10-03 |
| 3.6 | Document the backup arrangement and write the recovery procedures | Henry | 2026-10-10 |
| 3.7 | Document the deployment process, then automate it with pipelines | Henry | 2026-10-10 |
| 3.8 | Build the release history and version stamping | Henry | 2026-10-10 |
| 3.9 | **Document the business rules** — Feed Sales, Salmonella, Mycotoxin, Industry Profile | Ethan + Henry | 2026-10-10 |
| 3.10 | Consolidate the support, defect and development register from the boards | Ethan | 2026-10-03 |
| 3.11 | Decide the EPPlus licence position | Ethan + Pieter | 2026-10-03 |
| 3.12 | Decide the `AFMA-STAT` / `AFMA-MMS` architecture question | Ethan + Henry + Chris | 2026-10-03 |
| 3.13 | Agree characterisation test scope; begin the NHibernate and AutoMapper upgrades | Henry | 2026-10-17 |

**3.9 is the most valuable item in this phase and the easiest to postpone.** The business rules behind
the statistics modules — submission periods, validation, aggregation, corrections — exist in nobody's
writing. They are the knowledge that would be hardest to reconstruct and that AFMA would notice the
loss of first. Do not let it slip behind the infrastructure work.

## Phase 4 — Establish the steady state (from 2026-10-20)

**Objective:** the account runs on documented process rather than on individual recall.

| # | Task | Owner | Due |
|---|---|---|---|
| 4.1 | Agree the support model — who takes what, response expectations, escalation | Ethan + Henry | 2026-10-24 |
| 4.2 | Establish the client communication cadence with AFMA | Ethan | 2026-10-24 |
| 4.3 | Set the wiki review cycle — monthly, with an owner per section | Ethan | 2026-10-24 |
| 4.4 | Set the renewal calendar live with 60- and 30-day alerts | Ethan | 2026-10-24 |
| 4.5 | Agree the 12-month technical roadmap, including the member-management opportunity | Ethan + Pieter | 2026-10-31 |
| 4.6 | Verify the handover: a third person follows this wiki to perform a routine task unaided | Ethan | 2026-10-31 |

**4.6 is the only real test of whether the handover worked.** Everything before it is preparation.

## How delegation works on this account

| Person | Owns |
|---|---|
| **Ethan** | The account's technical landscape. Access, documentation, risk, client-facing technical communication, decisions that carry commercial weight. |
| **Henry** | Execution: build, deployment, remediation, infrastructure documentation. From 2026-09-21. |
| **Chris** | Historical context, architecture, unresolved technical dependencies. Consulted, not tasked. Questions batched. |

Working rules:

1. **Nothing is delegated verbally.** Every task has a wiki row with an owner and a date.
2. **Nothing is "done" until it is written here** and someone else can follow it.
3. **Missing access is raised the day it is discovered**, not accumulated into a list.
4. **Client communication on account structure stays with Ethan and Pieter.** Technical delivery
   communication follows the route agreed in 1.10.
5. **Every question for Chris goes on [Open Questions](/Open-Questions) first**, so his time is used
   once and the answer is captured where everyone can find it.

## What could go wrong with this plan

Stated plainly, because a plan that only describes success is not a plan.

| Risk to the plan | Mitigation |
|---|---|
| The private NuGet packages cannot be obtained at all | Then R1 becomes a rebuild question, not a packaging one, and the account's cost profile changes materially. Escalate to Pieter the day this becomes likely — do not absorb it quietly. |
| Access requests stall | Escalate at the due date, in writing. Every stalled request blocks a section of the review. |
| R2 turns out to be a reportable incident | The POPIA assessment (1.4) moves to the front of the plan and everything else waits. |
| The remediation work is larger than the account's budget | Surface it as Q21 on [Open Questions](/Open-Questions) with a costed option set. The decision is Pieter's; the estimate is ours. |
| Phase 3 business-rule documentation slips behind infrastructure work | It is listed first in this row for that reason. Protect the time. |
