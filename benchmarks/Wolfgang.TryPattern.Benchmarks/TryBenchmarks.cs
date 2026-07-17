using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Wolfgang.TryPattern.Benchmarks;

// No [SimpleJob(RuntimeMoniker.NetXY)] — BDN uses the compile target
// (net10.0 per this csproj's TargetFramework), which is the runtime
// most consumers actually deploy. Pinning to a specific moniker
// silently breaks when the csproj TargetFramework moves without the
// attribute being updated (as happened when v0.3.4 bumped net8→net10
// but the moniker stayed Net80).
[ExcludeFromCodeCoverage(Justification = "This is for benchmarking the code")]
[MemoryDiagnoser]
public class TryBenchmarks
{
    private const int OperationCount = 1000;

    [Benchmark]
    public void Action_Success()
    {
        for (var i = 0; i < OperationCount; i++)
        {
            Try.Run(() => { });
        }
    }

    [Benchmark]
    public void Action_WithException()
    {
        for (var i = 0; i < OperationCount; i++)
        {
            Try.Run(() => throw new InvalidOperationException());
        }
    }

    [Benchmark]
    public async Task RunAsync_Action_Success()
    {
        for (var i = 0; i < OperationCount; i++)
        {
            await Try.RunAsync(() => { });
        }
    }

    [Benchmark]
    public async Task RunAsync_Action_WithException()
    {
        for (var i = 0; i < OperationCount; i++)
        {
            await Try.RunAsync(() => throw new InvalidOperationException());
        }
    }

    [Benchmark]
    public void Run_Func_Success()
    {
        for (var i = 0; i < OperationCount; i++)
        {
            _ = Try.Run(() => 42);
        }
    }

    [Benchmark]
    public void Run_Func_WithException()
    {
        for (var i = 0; i < OperationCount; i++)
        {
            _ = Try.Run<int>(() => throw new InvalidOperationException());
        }
    }

    [Benchmark]
    public async Task RunAsync_Func_Success()
    {
        for (var i = 0; i < OperationCount; i++)
        {
            _ = await Try.RunAsync(async () =>
            {
                await Task.CompletedTask;
                return 42;
            });
        }
    }

    [Benchmark]
    public async Task RunAsync_Func_WithException()
    {
        for (var i = 0; i < OperationCount; i++)
        {
            _ = await Try.RunAsync<int>(async () =>
            {
                await Task.CompletedTask;
                throw new InvalidOperationException();
            });
        }
    }
}
