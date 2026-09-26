using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Wolfgang.TryPattern.Tests.Concurrency;

/// <summary>
/// Concurrency stress tests (#175). <see cref="Try"/> is a static class with no
/// shared mutable state and <see cref="Result"/> / <see cref="Result{T}"/> are
/// immutable, so the job here is to confirm under real thread-pool contention
/// that concurrent callers never see each other's outcome, and that a
/// cancellation racing a call never runs the delegate AND reports it as
/// canceled. Coyote was considered and skipped for the same reason as in
/// IAsyncEnumerable-Extensions: there is no shared state for a schedule
/// explorer to find, and the xunit stress suite runs everywhere `dotnet test` does.
/// </summary>
/// <remarks>
/// Every test is written without failure-only branches: outcomes are recorded
/// and checked as a whole, so each line executes on a passing run (the test
/// assembly is held to the coverage gate like the others).
/// </remarks>
public sealed class ConcurrencyStressTests
{
    // STRESS_ITERATIONS scales the round count: modest by default so the suite
    // runs in every PR's `dotnet test`, generous when the weekly
    // concurrency.yaml run sets it.
    private static readonly int StressIterations = GetStressIterations();

    private static readonly int ConcurrentCallers = Math.Clamp(Environment.ProcessorCount * 2, 4, 16);

    private readonly ITestOutputHelper _output;



    public ConcurrencyStressTests(ITestOutputHelper output)
    {
        _output = output;
    }



    private static int GetStressIterations()
    {
        var raw = Environment.GetEnvironmentVariable("STRESS_ITERATIONS");
        return int.TryParse(raw, out var parsed) && parsed > 0
            ? parsed
            : 100;
    }



    [Fact]
    public async Task Run_Func_when_called_concurrently_each_caller_gets_its_own_value()
    {
        for (var round = 0; round < StressIterations; round++)
        {
            var results = await Task.WhenAll
            (
                Enumerable
                    .Range(0, ConcurrentCallers)
                    .Select(id => Task.Run(() => (Id: id, Result: Try.Run(() => id * 3))))
            );

            Assert.All
            (
                results,
                r => Assert.True(r.Result.Succeeded && r.Result.Value == r.Id * 3)
            );
        }
    }



    [Fact]
    public async Task Run_Action_when_failing_concurrently_each_caller_gets_its_own_message()
    {
        for (var round = 0; round < StressIterations; round++)
        {
            var results = await Task.WhenAll
            (
                Enumerable
                    .Range(0, ConcurrentCallers)
                    .Select(id => Task.Run(() => (Id: id, Result: Try.Run(() => throw new InvalidOperationException($"caller {id}")))))
            );

            Assert.All
            (
                results,
                r => Assert.True(r.Result.Failed && string.Equals(r.Result.ErrorMessage, $"caller {r.Id}", StringComparison.Ordinal))
            );
        }
    }



    [Fact]
    public async Task RunAsync_Func_when_success_and_failure_interleave_outcomes_stay_with_their_caller()
    {
        for (var round = 0; round < StressIterations; round++)
        {
            var results = await Task.WhenAll
            (
                Enumerable
                    .Range(0, ConcurrentCallers)
                    .Select(async id => (Id: id, Result: await Try.RunAsync(() => SucceedOrThrowAsync(id))))
            );

            // Even ids succeed with their own id, odd ids fail with their own message.
            Assert.All
            (
                results,
                r => Assert.True
                (
                    r.Id % 2 == 0
                        ? r.Result.Succeeded && r.Result.Value == r.Id
                        : r.Result.Failed && string.Equals(r.Result.ErrorMessage, $"caller {r.Id}", StringComparison.Ordinal)
                )
            );
        }
    }



    [Fact]
    public async Task RunAsync_Action_when_cancellation_races_scheduling_a_canceled_call_never_ran_the_action()
    {
        var canceled = 0;
        for (var round = 0; round < StressIterations; round++)
        {
            using var cts = new CancellationTokenSource();
            var runs = 0;
            var cancel = Task.Run(cts.Cancel);

            var call = Try.RunAsync(() => Interlocked.Increment(ref runs), cts.Token);
            var thrown = await Record.ExceptionAsync(() => call);
            await cancel;

            // Exactly two legal outcomes: canceled before Task.Run started the
            // action (OperationCanceledException, action never ran), or the
            // action ran to completion once and the call succeeded.
            Assert.True
            (
                thrown is OperationCanceledException
                    ? Volatile.Read(ref runs) == 0
                    : thrown is null && call.Result.Succeeded && Volatile.Read(ref runs) == 1,
                $"round {round}: thrown={thrown?.GetType().Name ?? "none"}, runs={runs}"
            );
            canceled += thrown is null ? 0 : 1;
        }

        _output.WriteLine($"{canceled} of {StressIterations} rounds observed the cancellation before the action started.");
    }



    [Fact]
    public async Task RunAsync_Func_when_cancellation_races_the_token_check_a_canceled_call_never_invoked_the_function()
    {
        var canceled = 0;
        for (var round = 0; round < StressIterations; round++)
        {
            using var cts = new CancellationTokenSource();
            var invocations = 0;
            var cancel = Task.Run(cts.Cancel);

            var call = Try.RunAsync
            (
                () =>
                {
                    Interlocked.Increment(ref invocations);
                    return Task.FromResult(42);
                },
                cts.Token
            );
            var thrown = await Record.ExceptionAsync(() => call);
            await cancel;

            // Canceled before the token check: OperationCanceledException and the
            // function was never invoked. Otherwise it was invoked exactly once and
            // its value came back, even if the token was canceled a moment later.
            Assert.True
            (
                thrown is OperationCanceledException
                    ? Volatile.Read(ref invocations) == 0
                    : thrown is null && call.Result.Succeeded && call.Result.Value == 42 && Volatile.Read(ref invocations) == 1,
                $"round {round}: thrown={thrown?.GetType().Name ?? "none"}, invocations={invocations}"
            );
            canceled += thrown is null ? 0 : 1;
        }

        _output.WriteLine($"{canceled} of {StressIterations} rounds observed the cancellation before the token check.");
    }



    private static async Task<int> SucceedOrThrowAsync(int id)
    {
        // Yield so the continuation can resume on a different pool thread than
        // the one that started the call, which is where cross-talk would show.
        await Task.Yield();
        return id % 2 == 0
            ? id
            : throw new InvalidOperationException($"caller {id}");
    }
}
