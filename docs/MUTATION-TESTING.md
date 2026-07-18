# Mutation testing (Stryker.NET)

Mutation testing measures how effective the test suite is: Stryker
rewrites the source under test with tiny behavioural changes ("mutants"
— e.g. flipping `<` to `<=`, removing a `!`, replacing a return with
`default`) and checks whether the test suite catches each one. A
mutant that survives ("mutant survives") is a mutation the tests do
not detect — an unenforced piece of behaviour.

## Enforcement

`.github/workflows/stryker.yaml` runs Stryker on:

- **`pull_request`** targeting `main` or `vNext` (paths-filtered to
  `src/`, `tests/`, `stryker-config.json`, and the workflow itself) —
  **this is the release gate**.
- Manual `workflow_dispatch`.
- Weekly Sunday 06:00 UTC (drift catch — external analyzers or SDK
  updates can change the mutant set even with no code change).

`stryker-config.json` sets `thresholds.break` — the mutation score
percentage below which `dotnet stryker` itself exits non-zero. A PR
that regresses below the break threshold fails the workflow and blocks
merge.

## The break threshold

Current: **60%**. This is a conservative starting floor while we
establish a stable observed score across a few CI runs.

**Ratchet-up policy**: once we have 2–3 consecutive PR / weekly runs
showing an observed score of X%, bump the break threshold to `X - 3`
(3-point jitter tolerance) in a follow-up PR. Never lower the
threshold to accommodate a regression — investigate the regression
first.

## Surviving mutants

When Stryker reports a surviving mutant, it means the tests do not
enforce the piece of behaviour the mutant altered. File a
`kind:mutation-survives` issue with:

1. The source file + line the mutant was introduced at (from the
   Stryker HTML report artifact).
2. The mutation kind (e.g. "conditional boundary → `<` to `<=`").
3. A one-line proposal for the test that would catch it.

Fix by adding the missing test — do not suppress the mutant unless it
is a genuine equivalent mutant (a mutation that changes source but not
observable behaviour). Equivalent mutants are rare and require a
justification comment.

## Local run

```bash
dotnet tool install -g dotnet-stryker
dotnet stryker --config-file stryker-config.json
```

Report lands under `StrykerOutput/<timestamp>/reports/mutation-report.html`.

If Stryker crashes at initialisation with
`VisualBasicCommandLineParser` type-init errors, that is a known
Buildalyzer / Stryker 4.15 interaction issue on certain SDK
combinations. Run in CI to observe the score.
