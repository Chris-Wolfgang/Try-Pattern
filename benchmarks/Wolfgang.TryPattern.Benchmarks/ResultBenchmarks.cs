using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;

namespace Wolfgang.TryPattern.Benchmarks;

// Covers the Result aggregation helpers (#442). Flatten's lazy StringBuilder
// path is the only non-trivial allocation logic in the library: all successes
// return the cached singleton, one failure re-wraps the first message without a
// builder, and two or more failures build the joined message.
[ExcludeFromCodeCoverage(Justification = "This is for benchmarking the code")]
[MemoryDiagnoser]
public class ResultBenchmarks
{
    private const int ResultCount = 16;

    private Result[] _allSucceeded = [];
    private Result[] _oneFailed = [];
    private Result[] _allFailed = [];



    [GlobalSetup]
    public void Setup()
    {
        _allSucceeded = new Result[ResultCount];
        _oneFailed = new Result[ResultCount];
        _allFailed = new Result[ResultCount];

        for (var i = 0; i < ResultCount; i++)
        {
            _allSucceeded[i] = Result.Success();
            _oneFailed[i] = i == ResultCount / 2
                ? Result.Failure("failure " + i)
                : Result.Success();
            _allFailed[i] = Result.Failure("failure " + i);
        }
    }



    [Benchmark]
    public Result Flatten_AllSucceeded() => Result.Flatten(_allSucceeded);



    [Benchmark]
    public Result Flatten_OneFailed() => Result.Flatten(_oneFailed);



    [Benchmark]
    public Result Flatten_AllFailed() => Result.Flatten(_allFailed);



    [Benchmark]
    public bool AnyFailed_AllSucceeded() => Result.AnyFailed(_allSucceeded);



    [Benchmark]
    public bool AllSucceeded_AllSucceeded() => Result.AllSucceeded(_allSucceeded);
}
