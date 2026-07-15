# OSSF Scorecard baseline + policy

[OSSF Scorecard](https://github.com/ossf/scorecard) scores the
`Wolfgang.TryPattern` repo's security posture — branch protection,
signed commits, pinned dependencies, dangerous-workflow patterns, etc.
— on a 0-10 scale. This document captures the baseline score and the
policy for handling regressions.

Workflow: [`.github/workflows/scorecard.yml`](../.github/workflows/scorecard.yml)
Results: [Security → Code scanning](https://github.com/Chris-Wolfgang/Try-Pattern/security/code-scanning)
Public score: <https://scorecard.dev/viewer/?uri=github.com/Chris-Wolfgang/Try-Pattern>

## Score floor

**Target: score ≥ 7.5** on `main`.

Any PR whose merged score drops the repo below 7.5 must:

1. Explain the regression in the PR body — what specifically dropped
   and why.
2. After merge, add a line to `CHANGELOG.md`'s `[Unreleased] → Security`
   section documenting the accepted lower score, e.g.:

   ```markdown
   - **OSSF Scorecard**: score dropped from 8.2 → 7.6 because the
     `github/codeql-action` update introduced a new
     `Dangerous-Workflow` finding (accepted; codeql-action is a
     first-party GitHub action and the check is a false positive on
     the workflow_run trigger).
   ```

This is a soft gate (reviewer attention), not a hard CI-blocking
check. Scorecard's rules are strict enough that a single stale
dependency or transient false positive can drop the score below floor
while a fix is in flight; hard-gating would produce persistent noise.
If the score drops below **6.0**, treat as an incident: file an
`incident` issue and block further release-prep PRs until back above
floor.

## Baseline (populate after first successful run)

The workflow's first push-to-main run establishes the baseline. Fill
in this table after that run completes.

| Metric | Value |
|---|---|
| First-scan date | *(populate)* |
| First-scan score | *(populate)* /10 |
| Failed checks | *(populate — list each with the score contribution)* |
| Passing checks | *(populate — count only, or list the notable ones)* |

## Known-acceptable findings

Populated over time. Every entry names the check, the finding, and
why we accept it rather than fix it. Review annually.

| Check | Finding | Rationale | Reviewed |
|---|---|---|---|
| *(populate after first run)* | | | |

Example row (for reference — delete once real entries exist):

| Check | Finding | Rationale | Reviewed |
|---|---|---|---|
| `Signed-Releases` | Release artifacts are not signed via cosign / Sigstore | The nupkg + snupkg are signed by nuget.org's own signing pipeline on ingestion, which satisfies consumer verification without an additional sigstore step. Revisit if nuget.org drops that guarantee. | 2026-07-14 |

## Relationship to other security work

- **CodeQL** (`codeql.yaml`) scans the *code* for vulnerabilities.
- **DevSkim** (`Security Scan (DevSkim)` in `pr.yaml`) scans source
  patterns for dangerous API usage.
- **Gitleaks** (`Secrets Scan (gitleaks)` in `pr.yaml`) scans for
  committed secrets.
- **OSSF Scorecard** (this workflow) scans the *repo configuration*
  and *release-engineering practices* — no code inspection.

All four are complementary and none subsumes the others.
