# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

Thorough-review batch and repo hardening round. No runtime behaviour
change in `Wolfgang.TryPattern` itself except for #273 (fix
`Try.Run*` throwing on whitespace exception messages); everything
else is CI / test infrastructure that raises the release-readiness
bar. MINOR bump anticipated at release time (0.4.0) — new test
surface, new CI signals, but no public API changes.

### Added

- **CI (SLSA build-provenance)** — `release.yaml`'s
  `pack-and-validate` job attests every shipped `.nupkg` / `.snupkg`
  / `.bom.json` via `actions/attest-build-provenance@v2`. Consumers
  verify with
  `gh attestation verify Wolfgang.TryPattern.<v>.nupkg --repo Chris-Wolfgang/Try-Pattern`.
  Closes #176.
- **CI (Semgrep OSS SAST)** — new `semgrep-sast.yaml` runs
  `p/csharp` + `p/security-audit` rulesets on PR / push-to-main /
  weekly. SARIF ingest to Code Scanning; findings triaged in
  `docs/SEMGREP-AUDIT.md`. Closes #172.
- **CI (license audit)** — new `license-audit.yaml` using
  `dotnet-project-licenses` 2.7.1 fails on any transitive licence
  outside `.github/license/allowed-licenses.json`. Attaches
  `THIRD-PARTY-NOTICES.md` per run. Closes #185.
- **CI (ABI compat)** — new `api-compat.yaml` matrix over
  net8.0/net10.0 runs `apicompat` against the latest published
  version. Suppressions in `.github/api-compat/suppressions.xml`.
  Closes #174.
- **CI (Native AOT smoke)** — new `aot-smoke.yaml` publishes the
  `tests/Wolfgang.TryPattern.AotSmoke` project with `PublishAot=true`
  and runs the produced binary. Closes #180.
- **CI (reproducible-build verification)** — new
  `reproducible-build.yaml` builds on ubuntu + windows weekly and
  diffs the produced `.dll` hashes. Advisory warning on divergence.
  Closes #183.
- **CI (reproducible-build manifest)** — `release.yaml` now attaches
  a `reproducible-build-manifest.json` per release with SHA-256 of
  every shipped `.nupkg` / `.snupkg` / `lib/<tfm>/*.dll`, SDK
  version, and commit. Third-party verification procedure in
  `docs/REPRODUCIBLE-BUILD.md`. Closes #193.
- **CI (Stryker gate)** — `stryker.yaml` runs on PR (main + vNext)
  in addition to weekly; `stryker-config.json` `thresholds.break`
  bumped 0 → 60 as a conservative starting floor. Policy in
  `docs/MUTATION-TESTING.md`. Closes #173.
- **CI (PR benchmark regression gate)** — new `pr-benchmarks.yaml`
  runs BDN on PR HEAD + base, posts an upserted delta comment,
  fails on regressions beyond thresholds unless PR carries the
  `perf-impact-acknowledged` label. Closes #192.
- **CI (continuous fuzz)** — new `fuzz.yaml` runs FsCheck properties
  at MaxTest = 100_000 weekly. Failure auto-files a `bug`-labelled
  issue with the shrunk counter-example. Closes #170.
- **CI (cross-platform × ARM64 differential)** — new
  `cross-platform-differential.yaml` matrix over 6 (os, arch) rows,
  diffs test outcomes. Closes #177.
- **Tests (fuzz + property-based)** — new
  `tests/Wolfgang.TryPattern.Tests.Unit/PropertyTests.cs` and
  `FuzzTests.cs` cover the Try / Result surface with FsCheck.Xunit
  2.16.6.
- **Tests (Native AOT smoke)** — new
  `tests/Wolfgang.TryPattern.AotSmoke/` compile+publish+run smoke
  targeting every public API.
- **Tests (globalization invariance)** — new
  `GlobalizationInvarianceTests.cs` re-runs a representative slice
  under en-US, tr-TR, de-DE, zh-CN, ar-SA, ja-JP. Closes #182.
- **Tests (zero-alloc hot path)** — new `ZeroAllocationTests.cs`
  enforces zero allocations on `Result.Success()` and
  `Try.Run(Action)` success path via
  `GC.GetAllocatedBytesForCurrentThread()`. Closes #184.
- **Tests (XML doc example rot detection)** — new
  `DocExampleCompilationTests.cs` compiles every `<example><code>`
  block from the produced `Wolfgang.TryPattern.xml` through Roslyn.
  Three example blocks added to `Try.cs`. Closes #179.
- **docs** — new `SECURITY.md` "Release path & compromise scope"
  appendix (#188), `docs/SUPPLY-CHAIN.md` (#176),
  `docs/REPRODUCIBLE-BUILD.md` (#183, #193),
  `docs/MUTATION-TESTING.md` (#173), `docs/SEMGREP-AUDIT.md` (#172).

### Fixed

- **Try.Run\* throw on whitespace exception message** — every
  `Try.Run*` overload now coerces a null / empty / whitespace
  exception message to `ex.GetType().Name` via a private
  `SafeErrorMessage` helper before handing it to `Result.Failure`.
  Previously an exception whose `Message` was whitespace-only made
  `Try.Run` itself throw `ArgumentException` from inside its own
  catch. Discovered by the fuzz property added in this batch.
  Closes #273.

### Chore

- **benchmarks** — dropped `[SimpleJob(RuntimeMoniker.Net80)]` from
  `TryBenchmarks`. BDN now uses the compile target's runtime
  (net10.0 per the csproj) which is what most consumers actually
  deploy. The pinned moniker had gone stale after the v0.3.4
  net8 → net10 bump.

### Notes

- Bug #273 exists in every released version 0.3.5 and earlier; the
  fix in this release restores the intended
  never-throws-from-catch invariant.
- The Stryker `break: 60` threshold is a conservative starting
  floor; ratchet-up policy in `docs/MUTATION-TESTING.md`.
- Two thorough-review issues closed as not-applicable:
  #178 (Verify snapshot — no format-drift surface) and
  #171 (production-traffic shadow — no observable side effects).
  #175 (Coyote / concurrency stress) left open for future
  consideration.

## [0.3.5] - 2026-07-14

CI + tooling round. No public API or runtime behaviour change in
`Wolfgang.TryPattern` itself; PATCH bump per SemVer. Notable in this
release: the deployed docs site's version-picker dropdown now works
on `/versions/latest/`, and the fleet-wide ReSharper InspectCode CI
job is now live.

### Fixed

- **docs site (deployed)** — the version-picker `<select>` on
  `https://chris-wolfgang.github.io/Try-Pattern/versions/latest/`
  now shows a selected option. The old code had a dead URL-match
  block trying to resolve `latest` against a concrete `vX.Y.Z`
  URL — the URLs are structurally different by construction so the
  match never succeeded, leaving the dropdown with no selected
  option and no way to navigate. Kept `latest` as a first-class
  option on `/versions/latest/`; still hidden on concrete-version
  pages ([#247](https://github.com/Chris-Wolfgang/Try-Pattern/pull/247),
  closes #238).
- **CI (InspectCode SLN detection)** — the `Run InspectCode` step's
  `ls *.slnx 2>/dev/null | head -n1` idiom exited the whole step with
  code 2 on Git Bash for Windows (the workflow's `pipefail` shell
  propagated `ls`'s non-zero exit when no `*.slnx` files existed,
  killing the step before the `*.sln` fallback ran). Replaced with a
  direct `for candidate in *.slnx *.sln` glob loop — no pipe, no
  pipefail interaction ([#250](https://github.com/Chris-Wolfgang/Try-Pattern/pull/250)).

### Changed

- **CI (InspectCode)** — added a JetBrains ReSharper InspectCode
  job to `pr.yaml` running parallel to the test stages. Runs on
  `windows-latest` so the .NET Framework 4.x reference assemblies
  needed by the net462 examples resolve natively. SARIF uploads to
  GitHub Code Scanning; job fails on any `level=error` finding
  ([#245](https://github.com/Chris-Wolfgang/Try-Pattern/pull/245),
  [#248](https://github.com/Chris-Wolfgang/Try-Pattern/pull/248)).
  `"ReSharper InspectCode"` is now a required status check on the
  `main`-branch ruleset.
- **CI (docfx)** — synced `docfx.yaml` to the canonical repo-template
  version ([#235](https://github.com/Chris-Wolfgang/Try-Pattern/pull/235)).

### Chore

- **hygiene for InspectCode noise floor**
  ([#246](https://github.com/Chris-Wolfgang/Try-Pattern/pull/246)):
  - Dropped `<ImplicitUsings>enable</ImplicitUsings>` from the src,
    tests, and benchmarks csproj files. Every `.cs` file now
    declares its `using` directives explicitly. Files that only had
    conditional `#if !NET6_0_OR_GREATER using System; #endif`
    guards now use unconditional file-scoped usings.
  - Added folder-scoped `.editorconfig` suppressions for Roslyn
    analyzer rules that only apply to `tests/` (S1481 / S2190 /
    S2930 / S3928 / MA0012 / MA0015 / VSTHRD003) and `benchmarks/`
    (RS0016 / RS0037), plus R#-native rules like
    `access_to_disposed_closure`.
  - Added a top-level `TryPattern.sln.DotSettings` for the one
    solution-wide entry (`CheckNamespace` on the polyfill file).
  - Scoped `RS0030` (banned sync IO) suppression to the
    `VB.DotNet462.Example` vbproj — net462 has no async File API.
  - Shortened `[System.Diagnostics.CodeAnalysis.SuppressMessage(...)]`
    to `[SuppressMessage(...)]` in `Result.cs` (the using at the top
    already imports the type).

### Dependencies

- `Meziantou.Analyzer` bumped from **3.0.115** → **3.0.122**
  ([#239](https://github.com/Chris-Wolfgang/Try-Pattern/pull/239),
  [#243](https://github.com/Chris-Wolfgang/Try-Pattern/pull/243)).
- `Microsoft.CodeAnalysis.BannedApiAnalyzers` bumped from **4.14.0** →
  **5.6.0**
  ([#240](https://github.com/Chris-Wolfgang/Try-Pattern/pull/240)).
- `Microsoft.CodeAnalysis.PublicApiAnalyzers` bumped from **3.3.4** →
  **5.6.0**
  ([#241](https://github.com/Chris-Wolfgang/Try-Pattern/pull/241)).
- `SonarAnalyzer.CSharp` bumped from **10.27.0.140913** →
  **10.29.0.143774**
  ([#242](https://github.com/Chris-Wolfgang/Try-Pattern/pull/242),
  [#244](https://github.com/Chris-Wolfgang/Try-Pattern/pull/244)).

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
