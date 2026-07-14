# 0004 — `Result.Success()` returns a cached singleton

- **Status**: Accepted
- **Date**: 2026-05-03
- **PR / Issue**: retroactive; behaviour has been in place since
  the initial `Result` implementation.

## Context

`Result` (non-generic) is immutable: it has two publicly-observable
fields (`Succeeded`, `ErrorMessage`) and neither is mutable after
construction. A successful `Result` is completely uniform — there is
no per-call state.

`Try.Run(action)` returns `Result.Success()` on every success. In
tight validation loops (e.g. `Result.Flatten(Try.Run(…), Try.Run(…),
Try.Run(…), …)`), that path can fire thousands of times per second.
The naïve implementation

```csharp
public static Result Success() => new Result(succeeded: true, string.Empty);
```

allocates a fresh `Result` object on every call. For an immutable
type with identical state on every success, that's wasted allocation.

The alternative — leaving the constructor public and having callers
manage sharing themselves — pushes the singleton concern onto every
consumer. Since `Result` has no public constructor (it's protected —
consumers use factory methods), we can hold the singleton privately
and hand it out.

## Decision

`Result.Success()` returns a cached singleton `_successInstance`:

```csharp
private static readonly Result _successInstance =
    new(succeeded: true, string.Empty);

public static Result Success() => _successInstance;
```

`Result` is deliberately kept immutable to preserve the safety of
sharing this instance across threads and unrelated call sites.

## Consequences

- **Zero allocations on the success path** of `Try.Run(action)` and
  any consumer that calls `Result.Success()` directly. This shows up
  in the BDN benchmarks — the success-case Action and Func
  measurements report allocation counts noticeably lower than the
  failure-case counterparts.
- **Consumers MUST NOT rely on reference identity** to distinguish
  results. `Result.Success() == Result.Success()` is `true` by
  reference — two calls return the same object. Any consumer that
  compares two success results with `ReferenceEquals` or `==` and
  expects them to be different (e.g. to attach per-instance state) is
  broken. The XML doc on `Success()` calls this out explicitly.
- **`Result` MUST remain immutable.** If a future change adds any
  mutable field, adds a per-instance identity, or introduces
  disposable state, the singleton pattern breaks silently — one
  consumer's mutation would leak into every other call site's success
  result. If mutability is ever needed, this singleton must be
  removed in the same change.
- **`Result<T>.Success(value)` deliberately does NOT cache** — every
  call takes a per-call `value` argument, so there is no common
  instance to share. Caching would require keying by `value`, which
  is both memory-unbounded and semantics-changing.
