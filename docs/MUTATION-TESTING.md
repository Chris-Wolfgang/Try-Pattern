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
  `src/`, `tests/`, and the workflow itself) —
  **this is the release gate**.
- Manual `workflow_dispatch`.
- Weekly Sunday 06:00 UTC (drift catch — external analyzers or SDK
  updates can change the mutant set even with no code change).

`tests/Wolfgang.TryPattern.Tests.Unit/stryker-config.json` sets `thresholds.break` — the mutation score
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
cd tests/Wolfgang.TryPattern.Tests.Unit
dotnet stryker
```

Run it **from the test project directory**, where it picks up
`stryker-config.json` automatically. Report lands under
`tests/Wolfgang.TryPattern.Tests.Unit/StrykerOutput/<timestamp>/reports/mutation-report.html`.

Do not run it from the repository root. With `TryPattern.sln` in the
working directory Stryker switches to solution mode and analyses every
project, including `examples/VB.*.vbproj`. Stryker's Buildalyzer ships an
older `Microsoft.CodeAnalysis.VisualBasic` than the .NET 10 SDK's Roslyn,
so the VB parser fails with `TypeLoadException: Roslyn.Utilities.IObjectWritable`
(stryker-mutator/stryker-net#3666, #281 here).
