# Reproducible builds

`Wolfgang.TryPattern` aims for **deterministic** builds — the same source at
the same commit, built with the same SDK and MSBuild inputs, produces the
same managed assembly bytes.

Reproducibility (byte-identical output across different environments) is
verified on a best-effort basis, but is not a hard guarantee — see the
[Limits](#limits) section.

## The knobs

`Directory.Build.props` sets, for all projects in the repo:

- `<ContinuousIntegrationBuild Condition="'$(CI)' == 'true'">true</ContinuousIntegrationBuild>` —
  normalises embedded source paths so a check-out to `/home/runner/work/…`
  vs `C:\a\…` doesn't leak into the compiled `.pdb`.
- `<PublishRepositoryUrl>true</PublishRepositoryUrl>` +
  `<EmbedUntrackedSources>true</EmbedUntrackedSources>` +
  `<IncludeSymbols>true</IncludeSymbols>` +
  `<SymbolPackageFormat>snupkg</SymbolPackageFormat>` — embed the source-
  control commit into the produced package, embed any generated source that
  isn't checked into git, and emit a symbol package that points at the
  resolved commit URLs (see also
  [SourceLink verification](../.github/workflows/sourcelink-verify.yaml)).
- The C# compiler is deterministic by default on modern SDKs — the
  `<Deterministic>true</Deterministic>` MSBuild property is implicit and
  does not need to be set explicitly.

## Verifying yourself

Any third party can rebuild `Wolfgang.TryPattern.dll` at a given tag from a
clean checkout and compare it to what NuGet.org serves.

### 1. Tooling versions

- `dotnet --info` should report the same major/minor SDK the release was
  cut with. The reference is `dotnet --info` from the release run — visible
  in the `publish-nuget` job's log in the [Actions
  tab](https://github.com/Chris-Wolfgang/Try-Pattern/actions/workflows/release.yaml).
  Patch-version drift is usually fine; major/minor drift is not.
- `sha256sum` (any POSIX / GNU implementation) or `Get-FileHash -Algorithm
  SHA256` on Windows PowerShell.
- `unzip` / `Expand-Archive` for extracting the `.nupkg`.

### 2. Rebuild + hash locally

```bash
git clone https://github.com/Chris-Wolfgang/Try-Pattern.git
cd Try-Pattern
git checkout <tag>                         # e.g. v0.3.5
CI=true dotnet build src/Wolfgang.TryPattern \
  -c Release \
  -f net8.0 \
  -p:ContinuousIntegrationBuild=true
sha256sum src/Wolfgang.TryPattern/bin/Release/net8.0/Wolfgang.TryPattern.dll
```

### 3. Compare against NuGet.org

```bash
curl -LO https://www.nuget.org/api/v2/package/Wolfgang.TryPattern/<version>
unzip -p wolfgang.trypattern.<version>.nupkg lib/net8.0/Wolfgang.TryPattern.dll \
  | sha256sum
```

### 4. Compare against the published manifest

Every GitHub release attaches a `reproducible-build-manifest.json` file
(alongside the `.nupkg` / `.snupkg` / `.bom.json`) listing the SHA-256 of
each shipped `.nupkg` and every `lib/<tfm>/*.dll` inside it, plus the SDK
version + commit SHA the release was built from. This is the canonical
reference — any hash you compute yourself should be verifiable against
this file.

```bash
gh release download <tag> -R Chris-Wolfgang/Try-Pattern \
  -p 'reproducible-build-manifest.json'
cat reproducible-build-manifest.json | jq .
```

The three SHAs (local rebuild → NuGet.org `.nupkg` extract → published
manifest) should all match for the `.dll`. If they diverge, please [file a
discrepancy issue](#reporting-a-discrepancy) so we can investigate.

## Reporting a discrepancy

Open a
[reproducibility-discrepancy issue](https://github.com/Chris-Wolfgang/Try-Pattern/issues/new?title=Reproducibility+discrepancy+for+v%3CVERSION%3E&labels=reproducibility)
with, at minimum:

1. The release tag you tried to verify (e.g. `v0.3.5`).
2. Your OS + `dotnet --info` output.
3. The three SHAs (your local build, the extract from NuGet.org, and the
   value from `reproducible-build-manifest.json`).
4. Any deviation from the exact commands above.

Divergence between the published manifest and the extract from NuGet.org
is treated as a supply-chain incident. Divergence between a local rebuild
and both of the other two is usually an environment issue (SDK version,
`CI` variable not set, timezone-affected file, etc.) and is investigated
as a bug in the reproducibility guarantee — not as an incident.

## Third-party verification attestations

We do not currently accept unsolicited verification attestations. If you
have independently verified a release and want that on record, open a
discussion referencing the release tag and your verification methodology;
we will link it from the release notes.

The [Reproducible Builds project](https://reproducible-builds.org/) has
conventions for cross-organisation verification which we may adopt in a
future iteration. Feedback welcome via issue #193.

## Automated verification

`.github/workflows/reproducible-build.yaml` builds the current commit on
both `ubuntu-latest` and `windows-latest` (weekly on Mondays, and on manual
dispatch), then diffs the produced `.dll` hashes across the two runners.
The `compare` job posts a `::warning::` (not a hard failure) if the hashes
diverge — cross-OS byte-equality is aspirational, not gate-blocking.

## Limits

- **TFM slice**: only `net8.0` is verified. Framework TFMs (`net462`,
  `netstandard2.0`) require Windows-only reference assemblies via a
  targeting pack, so the same-inputs-across-OS guarantee cannot be met for
  them.
- **Cross-OS byte-equality is best-effort**. `<ContinuousIntegrationBuild>`
  normalises source paths but does not guarantee identical `.pdb` /
  `.snupkg` bytes between runners. Managed IL should match; embedded
  metadata may differ. The reproducible-build workflow surfaces any
  divergence as a warning.
- **Third-party rebuilds** are best done with the same major/minor
  `dotnet` SDK as the release used. `dotnet --info` on the release run
  (visible in the release-workflow logs) is the reference.
