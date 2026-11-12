# Semgrep audit log

Findings from `.github/workflows/semgrep.yaml` (formerly `semgrep-sast.yaml`) (Semgrep OSS with
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

_(No triaged findings.) Baseline clean as of 2026-10-27: a local
`semgrep scan --config p/csharp --config p/security-audit --config p/secrets`
(the configs `semgrep.yaml` runs, semgrep 1.180.0) on `main` reported 0
results._
