using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.TryPattern;

namespace Wolfgang.TryPattern.Soak;

// Soak harness for #382. Usage:
//
//   dotnet run -c Release --project benchmarks/Wolfgang.TryPattern.Soak -- \
//       --duration 05:00:00 --interval 00:01:00 --output soak-output
//
// Runs one worker per core, each looping over a mix of Try.Run / Try.RunAsync
// (success, failure, cancellation) and Result / Result<T> construction, and
// samples resource counters every --interval. At the end it compares the
// baseline window (the first hour after warm-up, or the first quarter of the run
// when that is shorter) against the last quarter, and exits 1 when any counter
// shows sustained growth (a leak), not on absolute values.
[ExcludeFromCodeCoverage(Justification = "Soak harness; run by the scheduled soak workflow, not the PR gate")]
internal static class Program
{
    private static long _exceptionCount;



    private static async Task<int> Main(string[] args)
    {
        SoakOptions options;
        try
        {
            options = SoakOptions.Parse(args);
        }
        catch (ArgumentException ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            await Console.Error.WriteLineAsync(SoakOptions.Usage);
            return 2;
        }

        Directory.CreateDirectory(options.OutputDirectory);
        AppDomain.CurrentDomain.FirstChanceException += (_, _) => Interlocked.Increment(ref _exceptionCount);

        Console.WriteLine
        (
            $"Soak: duration {options.Duration}, interval {options.Interval}, warm-up {options.Warmup}, " +
            $"workers {options.Workers}, max growth {options.MaxGrowthPercent}%"
        );

        using var cts = new CancellationTokenSource(options.Duration);
        var workers = Enumerable
            .Range(0, options.Workers)
            .Select(id => Task.Run(() => Workload.RunAsync(id, cts.Token)))
            .ToArray();

        var samples = await Sampler.RunAsync(options, () => Interlocked.Read(ref _exceptionCount), cts.Token);
        await Task.WhenAll(workers);

        await File.WriteAllTextAsync
        (
            Path.Combine(options.OutputDirectory, "samples.csv"),
            Sample.ToCsv(samples)
        );

        var report = Analyzer.Analyze(samples, options);
        await File.WriteAllTextAsync(Path.Combine(options.OutputDirectory, "summary.md"), report.Markdown);
        Console.WriteLine(report.Markdown);

        return report.Regressed ? 1 : 0;
    }
}



[ExcludeFromCodeCoverage(Justification = "Soak harness")]
internal sealed record SoakOptions
(
    TimeSpan Duration,
    TimeSpan Interval,
    TimeSpan Warmup,
    int Workers,
    double MaxGrowthPercent,
    string OutputDirectory
)
{
    public const string Usage =
        "Usage: Wolfgang.TryPattern.Soak [--duration hh:mm:ss] [--interval hh:mm:ss] [--warmup hh:mm:ss] " +
        "[--workers n] [--max-growth-percent n] [--output dir]";



    public static SoakOptions Parse(IReadOnlyList<string> args)
    {
        var duration = TimeSpan.FromHours(5);
        var interval = TimeSpan.FromMinutes(1);
        TimeSpan? warmup = null;
        var workers = Environment.ProcessorCount;
        var maxGrowth = 20d;
        var output = "soak-output";

        for (var i = 0; i < args.Count; i++)
        {
            var value = i + 1 < args.Count
                ? args[i + 1]
                : throw new ArgumentException($"Missing value for {args[i]}.");
            switch (args[i])
            {
                case "--duration": duration = ParseSpan(value); break;
                case "--interval": interval = ParseSpan(value); break;
                case "--warmup": warmup = ParseSpan(value); break;
                case "--workers": workers = int.Parse(value, CultureInfo.InvariantCulture); break;
                case "--max-growth-percent": maxGrowth = double.Parse(value, CultureInfo.InvariantCulture); break;
                case "--output": output = value; break;
                default: throw new ArgumentException($"Unknown option {args[i]}.");
            }

            i++;
        }

        if (duration <= TimeSpan.Zero || interval <= TimeSpan.Zero || workers < 1 || maxGrowth <= 0)
        {
            throw new ArgumentException("duration, interval, workers and max-growth-percent must be positive.");
        }

        // Default warm-up: 10% of the run, at least one interval, at most 30 minutes.
        var effectiveWarmup = warmup ?? TimeSpan.FromTicks
        (
            Math.Min
            (
                TimeSpan.FromMinutes(30).Ticks,
                Math.Max(interval.Ticks, duration.Ticks / 10)
            )
        );

        return new SoakOptions(duration, interval, effectiveWarmup, workers, maxGrowth, output);
    }



    private static TimeSpan ParseSpan(string value) =>
        TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var span)
            ? span
            : throw new ArgumentException($"'{value}' is not a TimeSpan (hh:mm:ss).");
}



[ExcludeFromCodeCoverage(Justification = "Soak harness")]
internal static class Workload
{
    public static async Task RunAsync(int workerId, CancellationToken stop)
    {
        var iteration = 0L;
        using var canceled = new CancellationTokenSource();
        await canceled.CancelAsync();

        while (!stop.IsCancellationRequested)
        {
            iteration++;

            // Success and failure paths of every overload.
            _ = Try.Run(static () => { });
            _ = Try.Run(static () => throw new InvalidOperationException("soak failure"));
            _ = Try.Run(() => workerId + iteration);
            _ = Try.Run<int>(static () => throw new FormatException("soak failure"));
            _ = await Try.RunAsync(static () => { }, CancellationToken.None);
            _ = await Try.RunAsync(static () => throw new InvalidOperationException("soak failure"), CancellationToken.None);
            _ = await Try.RunAsync(() => Task.FromResult(iteration), CancellationToken.None);
            _ = await Try.RunAsync<long>(static () => throw new FormatException("soak failure"), CancellationToken.None);

            // Cancellation: an already-canceled token propagates OperationCanceledException.
            try
            {
                _ = await Try.RunAsync(() => Task.FromResult(iteration), canceled.Token);
            }
            catch (OperationCanceledException)
            {
                // Expected: Try.RunAsync rethrows cancellation rather than wrapping it.
            }

            // Result / Result<T> construction and aggregation.
            _ = Result.Flatten
            (
                Result.Success(),
                Result.Failure("a"),
                Result<long>.Success(iteration),
                Result<string>.Failure("b")
            );
        }
    }
}



[ExcludeFromCodeCoverage(Justification = "Soak harness")]
internal sealed record Sample
(
    TimeSpan Elapsed,
    long HeapBytes,
    int Gen2Collections,
    int ThreadPoolThreads,
    int HandleCount,
    long Exceptions
)
{
    public static string ToCsv(IEnumerable<Sample> samples)
    {
        var sb = new StringBuilder("elapsed_seconds,heap_bytes,gen2_collections,threadpool_threads,handles,exceptions\n");
        foreach (var s in samples)
        {
            sb.Append(CultureInfo.InvariantCulture, $"{s.Elapsed.TotalSeconds:F0},{s.HeapBytes},{s.Gen2Collections},{s.ThreadPoolThreads},{s.HandleCount},{s.Exceptions}\n");
        }

        return sb.ToString();
    }
}



[ExcludeFromCodeCoverage(Justification = "Soak harness")]
internal static class Sampler
{
    public static async Task<List<Sample>> RunAsync(SoakOptions options, Func<long> exceptions, CancellationToken stop)
    {
        var samples = new List<Sample>();
        var clock = Stopwatch.StartNew();
        using var process = Process.GetCurrentProcess();

        while (true)
        {
            try
            {
                await Task.Delay(options.Interval, stop);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            // A full blocking collection first, so the heap figure is what is still
            // reachable (a leak), not garbage that has not been collected yet.
            var heap = GC.GetTotalMemory(forceFullCollection: true);
            process.Refresh();
            var sample = new Sample
            (
                clock.Elapsed,
                heap,
                GC.CollectionCount(2),
                ThreadPool.ThreadCount,
                process.HandleCount,
                exceptions()
            );
            samples.Add(sample);
            Console.WriteLine
            (
                FormattableString.Invariant
                (
                    $"{sample.Elapsed:hh\\:mm\\:ss} heap={sample.HeapBytes / 1024}KiB gen2={sample.Gen2Collections} threads={sample.ThreadPoolThreads} handles={sample.HandleCount} exceptions={sample.Exceptions}"
                )
            );
        }

        return samples;
    }
}



[ExcludeFromCodeCoverage(Justification = "Soak harness")]
internal static class Analyzer
{
    // Absolute floors so scheduler and allocator noise on a small, stable process
    // cannot trip the percentage on its own.
    private const long HeapFloorBytes = 2 * 1024 * 1024;
    private const int HandleFloor = 50;
    private const int ThreadFloor = 4;



    public static (bool Regressed, string Markdown) Analyze(IReadOnlyList<Sample> samples, SoakOptions options)
    {
        var steady = samples.Where(s => s.Elapsed >= options.Warmup).ToList();
        var sb = new StringBuilder();
        sb.AppendLine("## Soak summary");
        sb.AppendLine();
        sb.AppendLine(CultureInfo.InvariantCulture, $"- Duration: {options.Duration}, samples: {samples.Count} ({steady.Count} after the {options.Warmup} warm-up)");
        if (samples.Count > 0)
        {
            var last = samples[samples.Count - 1];
            sb.AppendLine(CultureInfo.InvariantCulture, $"- Gen-2 collections: {last.Gen2Collections}; first-chance exceptions: {last.Exceptions} ({last.Exceptions / Math.Max(1, last.Elapsed.TotalSeconds):F0}/s)");
        }

        if (steady.Count < 8)
        {
            sb.AppendLine();
            sb.AppendLine("Not enough post-warm-up samples (need 8) to judge a trend; treating as a failure so a misconfigured run is not read as clean.");
            return (true, sb.ToString());
        }

        var quarter = Math.Max(2, steady.Count / 4);
        var baselineEnd = options.Warmup + TimeSpan.FromHours(1);
        var baseline = steady.Where(s => s.Elapsed < baselineEnd).Take(quarter).ToList();
        if (baseline.Count < 2)
        {
            baseline = steady.Take(quarter).ToList();
        }

        var tail = steady.Skip(steady.Count - quarter).ToList();

        sb.AppendLine();
        sb.AppendLine("| Counter | Baseline median | Last-quarter median | Growth | Verdict |");
        sb.AppendLine("|---|---|---|---|---|");
        var regressed = false;
        regressed |= Check(sb, "GC heap (bytes)", baseline.Select(s => (double)s.HeapBytes), tail.Select(s => (double)s.HeapBytes), HeapFloorBytes, options.MaxGrowthPercent);
        regressed |= Check(sb, "Handles", baseline.Select(s => (double)s.HandleCount), tail.Select(s => (double)s.HandleCount), HandleFloor, options.MaxGrowthPercent);
        regressed |= Check(sb, "Thread-pool threads", baseline.Select(s => (double)s.ThreadPoolThreads), tail.Select(s => (double)s.ThreadPoolThreads), ThreadFloor, options.MaxGrowthPercent);

        sb.AppendLine();
        sb.AppendLine(regressed ? "**Result: sustained growth detected.**" : "**Result: no sustained growth.**");
        return (regressed, sb.ToString());
    }



    private static bool Check(StringBuilder sb, string name, IEnumerable<double> baseline, IEnumerable<double> tail, double floor, double maxGrowthPercent)
    {
        var before = Median(baseline);
        var after = Median(tail);
        var growth = before > 0 ? (after - before) / before * 100 : 0;
        var bad = after - before > floor && growth > maxGrowthPercent;
        sb.AppendLine(CultureInfo.InvariantCulture, $"| {name} | {before:F0} | {after:F0} | {growth:F1}% | {(bad ? "GROWTH" : "ok")} |");
        return bad;
    }



    private static double Median(IEnumerable<double> values)
    {
        var sorted = values.OrderBy(v => v).ToArray();
        var mid = sorted.Length / 2;
        return sorted.Length % 2 == 0
            ? (sorted[mid - 1] + sorted[mid]) / 2
            : sorted[mid];
    }
}
