# 0003 — `OperationCanceledException` is always rethrown, never captured as a `Result`

- **Status**: Accepted
- **Date**: 2026-05-03
- **PR / Issue**: retroactive; behaviour has been in place since
  `Try.RunAsync` was introduced.

## Context

`Try.RunAsync` catches exceptions from the wrapped action / function
and returns them as `Result.Failure(exception.Message)`. The obvious
first take is that this should apply uniformly to every exception —
including `OperationCanceledException` when the caller-provided
`CancellationToken` fires.

That would be wrong. `OperationCanceledException` has different
semantics from every other exception type in .NET:

- **The .NET runtime uses it as the standard cancellation signal.**
  `Task.WaitAsync`, `WhenAny` on a cancellation-linked task, and the
  entire `IAsyncEnumerable` cancellation contract all rely on
  `OperationCanceledException` propagating up unwrapped so callers can
  distinguish "this operation was cancelled" from "this operation
  failed."
- **`Task.Run(action, token)` throws `OperationCanceledException`
  when the token was already-cancelled at scheduling time**, before
  the action starts. Wrapping that in a `Result.Failure(…)` would
  lie about whether the action ran.
- **The `[EnforceExtendedAnalyzerRules]` async pattern** — which our
  `BannedSymbols.txt` codifies — expects `OperationCanceledException`
  to be *transparent* to intermediate `try`/`catch` layers. If a
  library at the middle of a call stack swallows it into a `Result`,
  the outer caller's cancellation loop breaks.

The alternative — treating `OperationCanceledException` as just
another exception and returning `Result.Failure` — was rejected: it
would make `Try.RunAsync` incompatible with standard .NET cancellation
patterns, forcing every consumer to write

```csharp
var result = await Try.RunAsync(work, token);
if (result.Failed && result.ErrorMessage.Contains("cancel", …))
    throw new OperationCanceledException(token);
```

which is exactly the boilerplate `Try` exists to eliminate.

## Decision

Both `Try.RunAsync` overloads (Action + Func) catch
`OperationCanceledException` in a dedicated `catch` block that
**rethrows** — the exception propagates to the caller unchanged. Only
non-cancellation exceptions become `Result.Failure(ex.Message)`.

```csharp
try
{
    await Task.Run(action, token).ConfigureAwait(false);
    return Result.Success();
}
catch (OperationCanceledException)
{
    throw;
}
catch (Exception ex)
{
    return Result.Failure(ex.Message);
}
```

The order matters: the `OperationCanceledException` catch must come
before the general `Exception` catch, or the specific handler is
unreachable.

For the `RunAsync(Func<Task<T>>)` overload, the token is also checked
BEFORE invoking the function via `token.ThrowIfCancellationRequested()`
— if cancellation is already requested when the caller reaches
`Try.RunAsync`, the function is never invoked and the
`OperationCanceledException` propagates directly.

## Consequences

- **Standard `.WaitAsync(token)` / `WhenAny` cancellation patterns
  work correctly around `Try.RunAsync`.** A caller can wire an outer
  cancellation without the library eating the signal.
- **Consumers cannot distinguish "action threw
  `OperationCanceledException` for unrelated reasons" from "the
  wrapping cancellation fired"** without extra work — but this
  ambiguity exists in *every* async .NET API, not just Try.
- **The XML doc on both `RunAsync` overloads must document this
  explicitly** (and does — see the `<exception cref="OperationCanceledException">`
  block on `Try.cs`). If a contributor rewrites the `RunAsync`
  implementation without preserving the OCE-rethrow, the observable
  behaviour changes even though the return type does not — no
  compiler error would catch that regression.
- **Tests explicitly assert the rethrow.** See
  `RunAsync_Action_CancellationToken_when_cancellation_is_requested_after_action_started_the_action_is_cancelled`
  in `tests/Wolfgang.TryPattern.Tests.Unit/RunAsyncActionTests.cs`
  — if that test starts passing when the rethrow is silently removed,
  the test's assertion pattern is wrong (it should be checking that
  the exception ESCAPED, not merely that it was thrown inside).
