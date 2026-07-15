# Wolfgang.TryPattern v<NEW-MAJOR> migration guide

> **Upgrading from v<PREV-MAJOR>.x?** This is the place. If you are
> upgrading between minor or patch versions within v<NEW-MAJOR>, see
> [CHANGELOG.md](../../CHANGELOG.md) instead — MINOR and PATCH
> releases carry no breaking changes.

- **Release**: `v<NEW-MAJOR>.0.0` (YYYY-MM-DD)
- **Previous major**: `v<PREV-MAJOR>.x`
- **Announcement / rationale**: [link to release notes or blog post]

## Breaking-change inventory

Every breaking change ships as one row. If it isn't in this table, it
isn't breaking; consumers who don't hit any listed row can upgrade
without code changes.

| # | Kind | Before (v<PREV-MAJOR>) | After (v<NEW-MAJOR>) | Fix path |
|---|---|---|---|---|
| 1 | Removed API | `Result.Foo(...)` | `Result.Bar(...)` | See section 1 below |
| 2 | Renamed API | `Try.OldName(...)` | `Try.NewName(...)` | See section 2 |
| 3 | Signature change | `Result.Baz(string s)` | `Result.Baz(ReadOnlySpan<char> s)` | See section 3 |
| 4 | Behaviour change | `Result.Qux()` returned `null` on empty | `Result.Qux()` throws on empty | See section 4 |
| 5 | Target framework removed | shipped `net462` | dropped `net462`, min is `netstandard2.0` | See section 5 |

Delete rows that don't apply to this release. Add rows for every
breaking change; do not merge multiple breakages into one row.

## Per-change migration

### 1. `Result.Foo(...)` removed — use `Result.Bar(...)` instead

**Why**: [one-paragraph rationale — reference an ADR if applicable]

**Before**:

```csharp
var r = Result.Foo(42);
```

**After**:

```csharp
var r = Result.Bar(42, options);
```

**Notes**: [any subtleties — e.g. `Bar` accepts an extra parameter but
defaults preserve v<PREV-MAJOR> behaviour, so passing only the first
argument is a mechanical migration]

### 2. `Try.OldName` renamed to `Try.NewName`

[same shape as above]

### 3. ...

## Deprecation timeline (looking back)

If v<NEW-MAJOR> removed APIs that were `[Obsolete]` in an earlier
release, record the timeline so consumers who skipped a version see
the deprecation warnings they missed.

| Version | What happened |
|---|---|
| v<PREV-MAJOR - 1>.Y | Deprecated `Result.Foo` via `[Obsolete("Use Bar instead", error: false)]` |
| v<PREV-MAJOR>.0 | Escalated to `[Obsolete(..., error: true)]` — build error |
| v<NEW-MAJOR>.0 | Removed |

If v<NEW-MAJOR> introduces a break with **no** prior deprecation, say
so explicitly here — a shipped breaking change without a deprecation
runway is unusual and warrants a note explaining why.

## Deprecation timeline (looking forward)

Anything v<NEW-MAJOR> ships as `[Obsolete]` but does not remove
belongs here, so consumers know what to expect in v<NEW-MAJOR + 1>.

| API | Status in v<NEW-MAJOR> | Planned removal |
|---|---|---|
| `Result.Baz(string)` | `[Obsolete(...)]` warning | v<NEW-MAJOR + 1>.0 |

## Compatibility notes

- **Target framework surface**: which TFMs v<NEW-MAJOR> supports vs
  v<PREV-MAJOR>. Call out any drops (e.g. "`net462` support removed;
  the minimum is now `netstandard2.0`") and additions.
- **NuGet package identity**: unchanged unless the package was renamed.
- **AssemblyVersion**: bumped from `<PREV-MAJOR>.0.0.0` to
  `<NEW-MAJOR>.0.0.0` per
  [ADR-0001](../adr/0001-pinned-assembly-version-for-binding-stability.md).
  .NET Framework consumers WILL need a one-time binding redirect at
  this boundary (this is the correct signal for a MAJOR).
- **Public API diff**: attached as `docs/migrations/v<NEW-MAJOR>-api-diff.txt`
  or similar (generated from `PublicAPI.Shipped.txt`), so tooling
  consumers can programmatically enumerate the surface change.

## FAQ

**Q: I'm on v<PREV-MAJOR>.x and don't hit any listed breaking change.
Do I need to do anything?**

Update the NuGet reference and run tests. If nothing breaks, you're
done.

**Q: The migration is bigger than I expected. Can I stay on
v<PREV-MAJOR> for another year?**

v<PREV-MAJOR> continues to receive PATCH releases for security /
critical-bug fixes for [duration policy]. Feature additions land on
v<NEW-MAJOR> only. Plan the upgrade before the PATCH support window
closes.

**Q: How do I report a v<NEW-MAJOR> migration problem this guide
doesn't cover?**

File an issue at
https://github.com/Chris-Wolfgang/Try-Pattern/issues/new with the
label `migration-help` and enough code context to reproduce.
