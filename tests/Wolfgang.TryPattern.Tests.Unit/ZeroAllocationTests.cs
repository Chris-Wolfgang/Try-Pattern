#if NET6_0_OR_GREATER
// Zero-allocation hot-path enforcement per issue #184.
//
// Try-Pattern is not a high-throughput library — it wraps user
// delegates with try/catch overhead. Only a small subset of the surface
// is intended to be zero-alloc:
//
//   1. `Result.Success()` — returns a cached singleton (`_successInstance`)
//      per the source; no `new` on the call path.
//   2. `Try.Run(Action)` on the success path — success returns
//      `Result.Success()` (singleton) and try/catch itself is
//      zero-heap-alloc.
//
// Everything else allocates by design: `Result.Failure(msg)` news up
// a Result instance, `Result<T>.Success/Failure` news up a Result<T>,
// `Try.Run<T>` news up a Result<T>, and every `RunAsync` overload
// allocates at least a Task. There is deliberately no attempt to
// enforce zero-alloc on those — the doc list above IS the enforced
// scope.
//
// Measurement notes:
//   - GC.GetAllocatedBytesForCurrentThread() requires net5+ / netcoreapp3+.
//     TFM-guarded to net6+ for stability.
//   - The test warms the method up first (1 pre-call) so JIT tiering
//     and any first-call bookkeeping don't pollute the sample.
//   - We measure a batch of N calls, not one, so any per-invocation
//     jitter averages out and small allocations (a few bytes from the
//     framework's own bookkeeping if any) would show up as N-multiplied.

using System;
using Xunit;

namespace Wolfgang.TryPattern.Tests.Unit;

public class ZeroAllocationTests
{
    private const int BatchSize = 100;

    private static readonly Action NoOp = () => { };


    [Fact]
    public void Result_Success_static_call_allocates_zero_bytes()
    {
        // Warm-up.
        _ = Result.Success();

        long before = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < BatchSize; i++)
        {
            _ = Result.Success();
        }
        long after = GC.GetAllocatedBytesForCurrentThread();

        long delta = after - before;
        Assert.True
        (
            delta == 0,
            $"Result.Success() static should be zero-alloc — observed {delta} bytes across {BatchSize} calls."
        );
    }


    [Fact]
    public void Try_Run_Action_success_path_allocates_zero_bytes()
    {
        // Warm-up.
        _ = Try.Run(NoOp);

        long before = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < BatchSize; i++)
        {
            _ = Try.Run(NoOp);
        }
        long after = GC.GetAllocatedBytesForCurrentThread();

        long delta = after - before;
        Assert.True
        (
            delta == 0,
            $"Try.Run(Action) success path should be zero-alloc — observed {delta} bytes across {BatchSize} calls (the wrapped Action itself must also be zero-alloc for this claim to hold)."
        );
    }
}

#endif
