# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

### Changed

### Deprecated

### Removed

### Fixed

### Security

## [0.3.4] - 2026-06-30

Follow-up Tier-2 cleanup round surfaced by the v0.3.3 AI code-review
pass. No public API change in `Wolfgang.TryPattern` itself; PATCH bump.

### Changed

- **docs (README)** — Database-access example converted from sync
  ADO inside `Try.Run` to `Try.RunAsync` with `ExecuteReaderAsync` /
  `OpenAsync` / `ReadAsync` and a `CancellationToken` parameter. The
  next example in the file was renamed from "Async database access"
  to "Async query returning a list" to avoid two sections sharing the
  same name
  ([#226](https://github.com/Chris-Wolfgang/Try-Pattern/pull/226), closes #221).
- **benchmarks** — `Wolfgang.TryPattern.Benchmarks.csproj`
  `<TargetFramework>` bumped from `net8.0` to `net10.0` so the
  published benchmark chart reflects the modern runtime consumers
  actually use. `benchmarks.yaml` SDK install updated from `8.0.x` to
  `10.0.x`
  ([#227](https://github.com/Chris-Wolfgang/Try-Pattern/pull/227), closes #220).
- **tests** — `xunit.runner.visualstudio` bumped from `3.0.0` to
  `3.1.5` (latest GA on the 3.x line; xunit core stays at 2.9.3 —
  runner 3.x supports xunit 1.x / 2.x / 3.x test discovery)
  ([#229](https://github.com/Chris-Wolfgang/Try-Pattern/pull/229), closes #218).

### Fixed

- **PublicApiAnalyzer manifest** — `Result<T>.Value.get` moved from
  the top-level `PublicAPI.Shipped.txt` into the existing per-TFM
  split (`PublicApi/modern/...` carries `Value.get -> T?`,
  `PublicApi/legacy/...` carries `Value.get -> T`). The top-level
  entry was silently wrong against modern (net5+) builds because
  RS0017 matches on member name, not return-type signature — so a
  future `Value` return-type regression would not have been caught
  ([#228](https://github.com/Chris-Wolfgang/Try-Pattern/pull/228), closes #219).
- **build scripts** — `scripts/build-pr.ps1` restored
  `-UseBasicParsing` on `Invoke-WebRequest` (was silently failing
  under PowerShell Core's stricter parser)
  ([#230](https://github.com/Chris-Wolfgang/Try-Pattern/pull/230)).

### Chore

- **tests directory + namespace alignment** — test project's
  directory, csproj filename, assembly name, and root namespace
  now all match under a single `Wolfgang.TryPattern.Tests.Unit`
  identity. Previously the directory was
  `tests/Wolfgang.TryPattern.Tests/` but the csproj was
  `Wolfgang.TryPattern.Tests.Unit.csproj` and the `.cs` files
  declared `namespace Wolfgang.TryPattern.Tests;` — three different
  names for the same project. `TryPattern.sln` and `stryker-config.json`
  paths updated to match
  ([#231](https://github.com/Chris-Wolfgang/Try-Pattern/pull/231), closes #222).

## [0.3.3] - 2026-06-28

Tier-1 maintenance round. Docs accuracy, code-review polish, and a
single behaviour fix on the docs-site version picker. No public API or
runtime behaviour change in `Wolfgang.TryPattern` itself.

### Fixed

- **docs site** — restored the canonical version-picker `<script>`
  bootstrap inside `docfx_project/docfx.json`'s `_appFooter`. The
  field had been reverted to a bare `"Made with DocFX"` string, so
  `docfx_project/public/version-picker.js` never loaded and the
  deployed docs at `https://chris-wolfgang.github.io/Try-Pattern/`
  showed no version dropdown despite the JS, `versions.json`, and
  `docs/DOCFX-VERSION-PICKER.md` all describing it as wired
  ([#224](https://github.com/Chris-Wolfgang/Try-Pattern/pull/224)).

### Changed

- **docs (README)** — section order standardized to the canonical
  fleet layout (License + Documentation hoisted after Installation;
  new `🔍 Code Quality & Static Analysis` section enumerating the 8
  wired analyzers). All Features-table API references cross-checked
  against `PublicAPI.Shipped.txt` (top-level + per-TFM splits)
  ([#216](https://github.com/Chris-Wolfgang/Try-Pattern/pull/216), closes #135, #138).
- **docs (other Markdown)** — accuracy audit. `CONTRIBUTING.md`
  updated from "7 Analyzers" to 8 (PublicApiAnalyzer entry added);
  `docs/WORKFLOW_SECURITY.md` bumped `actions/checkout@v6 → v7`;
  `docs/RELEASE-WORKFLOW-SETUP.md` "Workflow Architecture" section
  now lists all 6 release-pipeline jobs (`validate-release`,
  `pack-and-validate`, `verify-docs-build`, `publish-nuget`,
  `trigger-docs`, `update-release-artifacts`), where the previous
  text described only 3
  ([#217](https://github.com/Chris-Wolfgang/Try-Pattern/pull/217), closes #139).
- **source XML doc** — `Result` / `Result<T>` class summaries
  reworded so they no longer claim the types are only produced by
  `Try.Run` (they're also useful directly as repository / validation
  return types). `Failure()` factories now show `<returns>` and
  reference `<paramref name="errorMessage"/>` in their `<exception>`
  blocks. `Result<T>.Value`'s summary mentions throw-on-failure
  inline. `Try.RunAsync<T>` summary says "asynchronously" for parity
  with the Action variant
  ([#223](https://github.com/Chris-Wolfgang/Try-Pattern/pull/223), closes #128).

### Tests

- Renamed test methods so the name describes the actual asserted
  exception type (the two `..._throw_InvalidOperationException` cases
  actually assert `ArgumentException`) and to bring `RunFuncTests.cs`
  PascalCase outliers in line with the project's
  `Method_when_condition_expected_result` snake_case convention
  ([#223](https://github.com/Chris-Wolfgang/Try-Pattern/pull/223)).

### Chore

- **maintenance** — verified per-issue and closed: branch pruning
  (#127), PublicApiAnalyzer baseline still matches the shipped
  surface (#146), benchmark project builds clean in Release under
  the curated `benchmarks/.editorconfig` and inherited
  `TreatWarningsAsErrors` (#152). All three were already implemented
  at the framework level by the canonical sync; this round was
  verification only — no code change.
- **src csproj** — added `<PackageTags>` per the canonical
  `Directory.Build.props` per-repo expectation; normalized mixed
  tabs/spaces indent.
- **tests csproj** — dropped a dead `netcoreapp3.1 <ItemGroup
  Condition>` block (TFM is not in `<TargetFrameworks>`, so the
  group never matched) and a stale `<Version>0.3.0</Version>`
  (project is `IsPackable=false`, so the field was unused)
  ([#223](https://github.com/Chris-Wolfgang/Try-Pattern/pull/223)).
- **examples (F#)** — `FSharp.DotNet8.Example/Program.fs` module
  name was a copy-paste leftover from the net462 sibling
  (`FSharp.DotNet462.Example`); renamed to `FSharp.DotNet8.Example`
  ([#223](https://github.com/Chris-Wolfgang/Try-Pattern/pull/223)).

### Skipped / deferred

- **#208** (CI: add ReSharper InspectCode as parallel required
  check) — explicit fleet initiative requiring repo-template-first +
  `bulk-repo-pr` fan-out. Not a Try-Pattern-only pilot item; revisit
  once the canonical workflow lands in `repo-template`.
- Five additional Tier-2 sub-issues were filed under #128 covering
  larger / opinion-dependent items (xunit/runner mismatch on net8+
  TFMs, per-TFM `PublicAPI.Shipped.txt` `Value.get -> T?` entry,
  benchmarks TFM bump to net10, README database example sync-vs-
  async style, test csproj/dir naming alignment): #218, #219, #220,
  #221, #222.

## [0.3.2] - 2026-06-01

Canonical maintenance round + binding-stability fix. No public API or
runtime behavior change vs v0.3.1.

### Added

- **D8** — `verify-docs-build` job in `release.yaml` runs DocFX during
  the release pipeline before the NuGet push, so a docs build failure
  now blocks the package from shipping.
- **D8** — docs site version picker assets
  (`docfx_project/public/version-picker.js`,
  `docfx_project/versions.json`) and `docs/DOCFX-VERSION-PICKER.md`.
- **A1** — `PublicApiAnalyzers` scaffolding (analyzers activate when
  `PublicAPI.Shipped.txt` / `PublicAPI.Unshipped.txt` are present
  alongside the csproj).
- **CI3** — canonical NuGet package metadata: `Authors`, `Copyright`,
  `RepositoryType`, SourceLink, snupkg symbol packages, deterministic
  CI build flag, and `EmbedUntrackedSources` hoisted to
  `Directory.Build.props`.
- **T3** — Stryker mutation-testing workflow (`stryker.yaml`).
- **T1** — coverage report published to docs site.
- **S1** — CodeQL `security-extended` query pack.
- **D6** — versions.json preservation guard on the docs deploy.

### Changed

- **C1** — fleet-wide template-drift sync: workflow files (`pr.yaml`,
  `release.yaml`, `docfx.yaml`, `codeql.yaml`,
  `build-all-versions.yaml`, `stryker.yaml`), `.editorconfig`,
  `BannedSymbols.txt`, `Directory.Build.props`, and per-context
  `tests/Directory.Build.props` consolidated to the canonical baseline.
- **Nullable** — `<Nullable>enable</Nullable>` consolidated into
  `Directory.Build.props` (was per-csproj); per-project opt-out via
  override still supported.
- **CI2** — Dependabot `github-actions` ecosystem added.
- **D3** — repo scripts hardened (`Setup-Labels.ps1`,
  `Fix-BranchRuleset.ps1`).
- `github/codeql-action/init` and `analyze` bumped v3 → v4
  (Node.js 20 → 24 deprecation).

### Fixed

- **C4** — restored explicit `<AssemblyVersion>1.0.0.0</AssemblyVersion>`
  and added a prerelease-safe `<FileVersion>` (regex-strip property
  function) to the src csproj. The original C4 fanout had dropped
  these on the rationale that the hardcoded values were "stale"
  relative to released package versions — but that staleness was the
  correct binding-stability behaviour for libraries that ship a
  `net462` TFM. Without an explicit pin, SDK-derived `AssemblyVersion`
  would change on every minor/patch release, breaking .NET Framework
  consumers without a binding redirect. (See DateTime-Extensions v1.3.1
  for the post-mortem on what happens when this regression reaches a
  release.)
