# 0002 — Split `PublicAPI.Shipped.txt` per target framework

- **Status**: Accepted
- **Date**: 2026-06-27
- **PR / Issue**: [#211](https://github.com/Chris-Wolfgang/Try-Pattern/pull/211)
  (original split), [#228](https://github.com/Chris-Wolfgang/Try-Pattern/pull/228)
  (`Value.get` migration).

## Context

`Microsoft.CodeAnalysis.PublicApiAnalyzers` tracks the public API
surface via a `PublicAPI.Shipped.txt` manifest that lives next to the
csproj. The analyzer emits RS0016 ("symbol not declared") and RS0017
("declared symbol not found") to enforce that every public symbol in
the compilation exactly matches one line in the manifest.

Try-Pattern targets four TFMs: `net462`, `netstandard2.0`, `net8.0`,
`net10.0`. On the modern TFMs (`net5.0+` and `netstandard2.1+`), C# 8
nullable reference types are enabled and several methods return
nullable-annotated types:

```csharp
#if NET5_0_OR_GREATER
    public T? Value => Failed ? throw new … : _value;
#else
    public T  Value => Failed ? throw new … : _value;
#endif
```

The public surface **differs by TFM**. A single flat `PublicAPI.Shipped.txt`
cannot describe both signatures simultaneously — whichever one is in
the manifest, the analyzer emits RS0017 on the other TFM's compilation
because the "declared" symbol does not match what the compiler sees.

The alternative — declaring the whole surface as non-nullable and
letting `#nullable disable` in the source paper over it — sacrifices
the nullable-annotation contract that modern consumers rely on. The
`Result<T>.Value` return is genuinely a `T?` on modern TFMs and
consumers should get compile-time warnings when they assume otherwise.

## Decision

Store the TFM-invariant public surface in the standard top-level
`src/Wolfgang.TryPattern/PublicAPI.Shipped.txt`, and store the
per-TFM divergent surface under
`src/Wolfgang.TryPattern/PublicApi/{modern,legacy}/PublicAPI.Shipped.txt`.
Wire the analyzer to pick the right file via csproj conditions:

```xml
<ItemGroup>
    <AdditionalFiles Include="PublicApi\modern\PublicAPI.Shipped.txt"
        Condition="$([MSBuild]::IsTargetFrameworkCompatible('$(TargetFramework)', 'net5.0'))" />
    <AdditionalFiles Include="PublicApi\modern\PublicAPI.Unshipped.txt"
        Condition="$([MSBuild]::IsTargetFrameworkCompatible('$(TargetFramework)', 'net5.0'))" />
    <AdditionalFiles Include="PublicApi\legacy\PublicAPI.Shipped.txt"
        Condition="!$([MSBuild]::IsTargetFrameworkCompatible('$(TargetFramework)', 'net5.0'))" />
    <AdditionalFiles Include="PublicApi\legacy\PublicAPI.Unshipped.txt"
        Condition="!$([MSBuild]::IsTargetFrameworkCompatible('$(TargetFramework)', 'net5.0'))" />
</ItemGroup>
```

Both files are named `PublicAPI.Shipped.txt` because the analyzer
matches by filename, not path — the folder is the disambiguator.

## Consequences

- **The analyzer catches per-TFM regressions.** If a future refactor
  accidentally returns `T` instead of `T?` on modern TFMs (or vice
  versa on legacy), the analyzer emits RS0017 on the affected build,
  before the change lands.
- **Every new public API declared inside a `#if TFM` branch must be
  added to the matching per-TFM manifest**, not the top-level one.
  The top-level manifest is for surface that is TFM-invariant.
  Falling into the top-level file by default means the analyzer
  silently accepts a mismatched signature — as happened in Try-Pattern
  up through v0.3.4, when `Result<T>.Value.get -> T` (non-nullable)
  lived in the top-level file while the modern build actually
  returned `T?`. See [#219](https://github.com/Chris-Wolfgang/Try-Pattern/issues/219)
  and its fix in [#228](https://github.com/Chris-Wolfgang/Try-Pattern/pull/228).
- **Contributors adding a public API need to remember three files**:
  the top-level manifest, and one of the per-TFM manifests IF the
  addition is inside a `#if` branch. Running `dotnet build -c Release`
  will report the discrepancy (RS0016 for missing, RS0017 for extra)
  in the failing TFM slice.
- **The pattern is fleet-wide** — every Wolfgang.* library that
  multi-targets both modern and legacy TFMs uses the same split.
  See the `reference_publicapi_per_tfm_split` memory for the pattern
  validation history and the RS0017 recovery process when the
  manifest text drifts.
