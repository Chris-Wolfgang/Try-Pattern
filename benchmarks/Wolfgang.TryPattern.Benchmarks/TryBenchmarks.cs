using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Wolfgang.TryPattern.Benchmarks;

[ExcludeFromCodeCoverage(Justification = "This is for benchmarking the code")]
[SimpleJob(RuntimeMoniker.Net80)]
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
            await Try.RunAsync(async () => await Task.CompletedTask);
        }
    }

    [Benchmark]
    public async Task RunAsync_Action_WithException()
    {
        for (var i = 0; i < OperationCount; i++)
        {
            await Try.RunAsync(async () =>
            {
                await Task.CompletedTask;
                throw new InvalidOperationException();
            });
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
