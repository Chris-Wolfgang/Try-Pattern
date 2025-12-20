using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Wolfgang.TryPattern.Benchmarks;

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
            Try.Action(() => { });
        }
    }

    [Benchmark]
    public void Action_WithException()
    {
        for (var i = 0; i < OperationCount; i++)
        {
            Try.Action(() => throw new InvalidOperationException());
        }
    }

    [Benchmark]
    public async Task ActionAsync_Success()
    {
        for (var i = 0; i < OperationCount; i++)
        {
            await Try.ActionAsync(async () => await Task.CompletedTask);
        }
    }

    [Benchmark]
    public async Task ActionAsync_WithException()
    {
        for (var i = 0; i < OperationCount; i++)
        {
            await Try.ActionAsync(async () =>
            {
                await Task.CompletedTask;
                throw new InvalidOperationException();
            });
        }
    }

    [Benchmark]
    public void Function_Success()
    {
        for (var i = 0; i < OperationCount; i++)
        {
            _ = Try.Function(() => 42);
        }
    }

    [Benchmark]
    public void Function_WithException()
    {
        for (var i = 0; i < OperationCount; i++)
        {
            _ = Try.Function<int>(() => throw new InvalidOperationException());
        }
    }

    [Benchmark]
    public async Task FunctionAsync_Success()
    {
        for (var i = 0; i < OperationCount; i++)
        {
            _ = await Try.FunctionAsync(async () =>
            {
                await Task.CompletedTask;
                return 42;
            });
        }
    }

    [Benchmark]
    public async Task FunctionAsync_WithException()
    {
        for (var i = 0; i < OperationCount; i++)
        {
            _ = await Try.FunctionAsync<int>(async () =>
            {
                await Task.CompletedTask;
                throw new InvalidOperationException();
            });
        }
    }
}
