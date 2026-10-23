# AFMA Technical Wiki

This wiki is the central technical record for the AFMA account. It exists so that no part of AFMA's
systems depends on knowledge held by a single person.

> **Rule of this wiki:** if a fact about an AFMA system is not written here, it is not considered known.
> Anything discovered in a call, a support ticket or a developer's head is written here the same day.

## Status conventions

Every page uses the same three markers. Do not soften them.

| Marker | Meaning |
|---|---|
| **VERIFIED** | Confirmed first-hand, with the evidence named (file, command output, console screenshot). Includes the date it was checked. |
| **UNVERIFIED** | Believed true, taken from someone's word or from an older document. Has a named owner and a date by which it must be verified. |
| **GAP** | Known to be missing or unknown. Has a named owner and a date. |

A page with no markers has not been started.

## Start here

| Page | What it answers |
|---|---|
| [Account Overview](/Account-Overview) | What AFMA is, what we run for them, who does what |
| [Technical Landscape](/Technical-Landscape) | The full review checklist and where each area stands |
| [Risk Register](/Risk-Register) | What is currently broken, exposed or fragile |
| [Access Register](/Access-Register) | What access we hold, what we are missing |
| [Open Questions](/Open-Questions) | Questions that need Chris, Henry or AFMA to answer |

## Sections

- **[Systems](/Systems)** — AFMA STATS, AFMA MMS, the AFMA website
- **[Development](/Development)** — repositories, branches, building, deploying
- **[Operations](/Operations)** — hosting, backups, integrations, licences, maintenance mode
- **[Handover](/Handover)** — the handover plan and its checklist

## Current state of this wiki

Created 2026-09-14. The repository-derived pages (Systems, Development) are populated from a
first-hand review of the `AFMA-STAT` and `AFMA-MMS` repositories at the commits named on those pages.
The infrastructure pages (Operations) are structured but largely **GAP** — they need console access
that has not yet been granted. See [Access Register](/Access-Register).
