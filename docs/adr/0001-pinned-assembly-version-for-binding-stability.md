# 0001 — Pin `<AssemblyVersion>` to `1.0.0.0` for binding stability

- **Status**: Accepted
- **Date**: 2026-06-01
- **PR / Issue**: retroactive; original C4 restoration in the v0.3.2
  Tier-1 pilot; regression on `AssemblyVersion` drop-then-restore
  documented in DateTime-Extensions v1.3.0 → v1.3.1 post-mortem.

## Context

`Wolfgang.TryPattern` ships four target frameworks:
`net462`, `netstandard2.0`, `net8.0`, `net10.0`. The `net462` slice
still runs on the .NET Framework CLR, where **assembly identity is
part of type resolution**: every reference to a type in an external
assembly binds against a specific `AssemblyName` — `<Name>, Version=X.Y.Z.W,
Culture=…, PublicKeyToken=…`.

Without an explicit `<AssemblyVersion>` in the csproj, the .NET SDK
derives one from the `<Version>` NuGet property:

- `<Version>0.3.4</Version>` → derived `AssemblyVersion = 0.3.4.0`
- `<Version>0.3.5</Version>` → derived `AssemblyVersion = 0.3.5.0`

Every minor/patch release therefore changes the assembly's binding
identity. A `net462` consumer compiled against `0.3.4.0` will throw
`FileLoadException: Could not load file or assembly '<Name>,
Version=0.3.4.0, …'` when it encounters the deployed `0.3.5.0`
assembly at runtime — unless the consumer adds a binding redirect for
every hop:

```xml
<bindingRedirect oldVersion="0.0.0.0-0.3.5.0" newVersion="0.3.5.0"/>
```

Standard library convention (NodaTime, Newtonsoft.Json, AutoMapper) is
to **pin `AssemblyVersion` to the MAJOR-line baseline** and let
`AssemblyFileVersion` / `AssemblyInformationalVersion` carry the
per-release version. NodaTime's 3.x line, for instance, has kept
`AssemblyVersion=3.0.0.0` for years while shipping 3.0 → 3.1 → 3.2 → …
so consumers do not need a binding redirect between minor releases.

The alternative — letting the SDK derive it — is what happened during
the original C4 fleet fanout on the (incorrect) premise that a
hardcoded `1.0.0` value was "stale relative to the released package
versions." DateTime-Extensions v1.3.0 shipped with a derived
`AssemblyVersion=1.3.0.0`, breaking every .NET Framework consumer
compiled against 1.0/1.1/1.2 without a binding redirect. Recovery
required a v1.3.1 hotfix that restored the pin.

## Decision

`src/Wolfgang.TryPattern/Wolfgang.TryPattern.csproj` sets
`<AssemblyVersion>1.0.0.0</AssemblyVersion>` explicitly and leaves it
at that value for every release in the `1.x` line — including PATCH
and MINOR bumps.

`<FileVersion>` is derived from `<Version>` with a regex-strip of any
prerelease/metadata suffix:

```xml
<FileVersion>$([System.Text.RegularExpressions.Regex]::Replace(
    "$(Version)", "[-+].*$", "")).0</FileVersion>
```

`<AssemblyInformationalVersion>` is left to the SDK's default (which
appends the git commit SHA), giving diagnostic tools the full
version + commit story without affecting binding.

## Consequences

- **.NET Framework consumers do not need a binding redirect** when
  upgrading between minor/patch releases in the `1.x` line. This is the
  primary win.
- **Debuggers / crash dumps / assembly-load logs show the release
  version via `FileVersion` / `InformationalVersion`**, not
  `AssemblyVersion`. Tooling that reports "assembly version" often
  means one of the other three — check which field before diagnosing
  a "still on 1.0.0.0" report.
- **A deliberate breaking API change requires bumping `AssemblyVersion`.**
  The next MAJOR (2.0.0) must set `<AssemblyVersion>2.0.0.0</AssemblyVersion>`.
  Consumers will need a one-time binding redirect at the 1.x → 2.x
  boundary, which is the correct signal: a MAJOR release IS a
  binding-breaking release.
- **If a future contributor silently drops the `<AssemblyVersion>` line
  from the csproj** thinking it looks "stale", every subsequent release
  ships a different binding identity and breaks every net462 consumer.
  Both the csproj carries a comment explaining this, and this ADR is
  the durable record.
- **Fleet-wide precedent**: the same pin lives in every downstream
  Wolfgang.* library repo per the canonical `repo-template`.
  Restoring it after the original C4 drop was part of the fleet
  cleanup covered in the DateTime-Extensions v1.3.1 post-mortem;
  Try-Pattern inherited the fixed version.
