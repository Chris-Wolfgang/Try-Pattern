# Semgrep audit log

Findings from `.github/workflows/semgrep-sast.yaml` (Semgrep OSS with
the `p/csharp` and `p/security-audit` rulesets) are triaged here.
Each finding gets a decision — **fixed**, **false-positive**, or
**accepted-risk** — with a one-line rationale and the commit / PR
that carries the resolution.

Semgrep findings also appear in the Security tab (Code Scanning
alerts), which is the authoritative live view. This file is the
committed audit trail — a paper copy so future maintainers can see
the reasoning behind an "accepted-risk" or "false-positive"
disposition without archaeology into old PR discussions.

## Layout

Entries are appended chronologically. When a batch of findings is
addressed in one PR, group them under one date/PR header.

## Findings

_(No triaged findings yet — the workflow ships in the PR that adds
this file. First entries will appear after the initial CI baseline
run.)_
