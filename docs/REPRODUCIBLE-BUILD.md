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
clean checkout and compare it to what NuGet.org serves:

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

Then compare against the `.dll` extracted from the tag's `.nupkg` on
NuGet.org:

```bash
curl -LO https://www.nuget.org/api/v2/package/Wolfgang.TryPattern/<version>
unzip -p wolfgang.trypattern.<version>.nupkg lib/net8.0/Wolfgang.TryPattern.dll \
  | sha256sum
```

The two SHAs should match. If they do not, please open an issue with the
tag, your OS, `dotnet --info`, and both hashes so we can investigate.

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
