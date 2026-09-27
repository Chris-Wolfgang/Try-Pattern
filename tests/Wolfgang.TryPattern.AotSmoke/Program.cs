// Native AOT / trimming smoke test for the whole public surface of
// Wolfgang.TryPattern. Published with PublishAot + PublishTrimmed (see the
// .csproj) and run by CI (.github/workflows/aot-smoke.yaml): a trim/AOT-unsafe
// regression makes the analyzers warn (TreatWarningsAsErrors -> build fails),
// and a runtime break (MissingMethodException / NotSupportedException / a
// silently wrong result) makes this program exit non-zero.
//
// Every check names the public API member it covers, as the member appears in
// src/Wolfgang.TryPattern/**/PublicAPI.*.txt with the namespace, `static` and
// `.get` removed (e.g. "Result.Flatten", "Result<T>.Value", "Try.RunAsync<T>").
// AotSmokeCoverageTests in the unit suite fails when a public member has no
// Check here, so new API cannot ship without an AOT smoke check.
//
// Beyond one call per member, the paths most likely to break under AOT are
// covered on purpose:
//   - value-type AND reference-type instantiations of every generic member
//     (AOT compiles value-type generics separately; reference types share code),
//   - exception paths (guards, failures, cancellation), which pull in
//     exception-handling and stack-trace machinery,
//   - the whitespace-message fallback, which reads ex.GetType().Name — the one
//     place the library touches type metadata.

using System.Globalization;
using Wolfgang.TryPattern;
using Wolfgang.TryPattern.AotSmoke;

var failures = new List<string>();
var checks = 0;

static bool Same(string? a, string? b) => string.Equals(a, b, StringComparison.Ordinal);

void Check(string api, bool condition, string what)
{
    checks++;
    if (!condition)
    {
        failures.Add($"{api}: {what}");
    }
}

void Throws<TException>(string api, Action action, string what)
    where TException : Exception
{
    Exception? caught = null;
    try
    {
        action();
    }
    catch (Exception ex)
    {
        caught = ex;
    }

    Check(api, caught is TException, $"{what} (threw {caught?.GetType().Name ?? "nothing"})");
}

async Task ThrowsAsync<TException>(string api, Func<Task> action, string what)
    where TException : Exception
{
    Exception? caught = null;
    try
    {
        await action();
    }
    catch (Exception ex)
    {
        caught = ex;
    }

    Check(api, caught is TException, $"{what} (threw {caught?.GetType().Name ?? "nothing"})");
}

using var canceled = new CancellationTokenSource();
await canceled.CancelAsync();

// ---- Try.Run(Action) -------------------------------------------------------
Result run = Try.Run(() => { });
Check("Try.Run", run.Succeeded, "success");
run = Try.Run(() => throw new InvalidOperationException("boom"));
Check("Try.Run", run.Failed && Same(run.ErrorMessage, "boom"), "failure carries the exception message");
run = Try.Run(() => throw new InvalidOperationException("   "));
Check("Try.Run", run.Failed && Same(run.ErrorMessage, nameof(InvalidOperationException)), "whitespace message falls back to the exception type name");
Throws<ArgumentNullException>("Try.Run", () => Try.Run(null!), "null action");

// ---- Try.Run<T>(Func<T>) — value type and reference type -------------------
Result<int> runInt = Try.Run(() => 42);
Check("Try.Run<T>", runInt.Succeeded && runInt.Value == 42, "value-type success");
runInt = Try.Run<int>(() => throw new InvalidOperationException("boom-t"));
Check("Try.Run<T>", runInt.Failed && Same(runInt.ErrorMessage, "boom-t"), "value-type failure");
Result<string?> runString = Try.Run<string?>(() => "text");
Check("Try.Run<T>", runString.Succeeded && Same(runString.Value, "text"), "reference-type success");
Result<Point> runPoint = Try.Run(() => new Point(1, 2));
Check("Try.Run<T>", runPoint.Succeeded && runPoint.Value == new Point(1, 2), "user-defined struct success");
Throws<ArgumentNullException>("Try.Run<T>", () => Try.Run<int>(null!), "null function");

// ---- Try.RunAsync(Action, CancellationToken) --------------------------------
Result runAsync = await Try.RunAsync(() => { });
Check("Try.RunAsync", runAsync.Succeeded, "success");
runAsync = await Try.RunAsync(() => throw new InvalidOperationException("async-boom"), CancellationToken.None);
Check("Try.RunAsync", runAsync.Failed && Same(runAsync.ErrorMessage, "async-boom"), "failure");
await ThrowsAsync<OperationCanceledException>("Try.RunAsync", () => Try.RunAsync(() => { }, canceled.Token), "pre-canceled token propagates cancellation");
await ThrowsAsync<ArgumentNullException>("Try.RunAsync", () => Try.RunAsync(null!), "null action");

// ---- Try.RunAsync<T>(Func<Task<T>>, CancellationToken) — both kinds of T -----
Result<string?> runAsyncString = await Try.RunAsync<string?>(() => Task.FromResult<string?>("ok"));
Check("Try.RunAsync<T>", runAsyncString.Succeeded && Same(runAsyncString.Value, "ok"), "reference-type success");
runAsyncString = await Try.RunAsync<string?>(() => throw new InvalidOperationException("async-boom-t"));
Check("Try.RunAsync<T>", runAsyncString.Failed && Same(runAsyncString.ErrorMessage, "async-boom-t"), "reference-type failure");
Result<int> runAsyncInt = await Try.RunAsync(async () =>
{
    await Task.Yield();
    return 7;
});
Check("Try.RunAsync<T>", runAsyncInt.Succeeded && runAsyncInt.Value == 7, "value-type success across an await");
await ThrowsAsync<OperationCanceledException>("Try.RunAsync<T>", () => Try.RunAsync(() => Task.FromResult(1), canceled.Token), "pre-canceled token propagates cancellation");
await ThrowsAsync<ArgumentNullException>("Try.RunAsync<T>", () => Try.RunAsync<int>(null!), "null function");

// ---- Result: constructor, factories, properties -----------------------------
// The constructor is protected: exercise it the way a consumer can, through a subclass.
Check("Result.Result", new CustomResult(true, null).Succeeded, "succeeded with no message");
Check("Result.Result", Same(new CustomResult(false, "bad").ErrorMessage, "bad"), "failed with a message");
Throws<ArgumentException>("Result.Result", () => _ = new CustomResult(true, "unexpected"), "a success must not carry a message");
Throws<ArgumentException>("Result.Result", () => _ = new CustomResult(false, " "), "a failure needs a non-whitespace message");

Result success = Result.Success();
Check("Result.Success", success.Succeeded, "succeeds");
Check("Result.Succeeded", success.Succeeded && !Result.Failure("x").Succeeded, "true only on success");
Check("Result.Failed", Result.Failure("x").Failed && !success.Failed, "true only on failure");
Check("Result.ErrorMessage", success.ErrorMessage is null && Same(Result.Failure("why").ErrorMessage, "why"), "null on success, the message on failure");
Check("Result.Failure", Result.Failure("nope").Failed, "fails");
Throws<ArgumentException>("Result.Failure", () => Result.Failure(" "), "whitespace message");

// ---- Result combinators ------------------------------------------------------
Check("Result.Flatten", Result.Flatten(Result.Success(), Result.Success()).Succeeded, "all successes");
Result flat = Result.Flatten(Result.Success(), Result.Failure("first"), Result.Failure("second"));
Check("Result.Flatten", flat.Failed && flat.ErrorMessage!.Contains("first", StringComparison.Ordinal) && flat.ErrorMessage.Contains("second", StringComparison.Ordinal), "joins every failure message");
Check("Result.Flatten", Result.Flatten().Succeeded, "empty input succeeds");
Throws<ArgumentNullException>("Result.Flatten", () => Result.Flatten(null), "null array");

Check("Result.AnyFailed", Result.AnyFailed(Result.Success(), Result.Failure("f")), "true with a failure");
Check("Result.AnyFailed", !Result.AnyFailed(Result.Success()) && !Result.AnyFailed(), "false with none, and when empty");
Throws<ArgumentNullException>("Result.AnyFailed", () => Result.AnyFailed(null), "null array");

Check("Result.AllSucceeded", Result.AllSucceeded(Result.Success(), Result.Success()) && Result.AllSucceeded(), "true when all succeed, and when empty");
Check("Result.AllSucceeded", !Result.AllSucceeded(Result.Success(), Result.Failure("f")), "false with a failure");
Throws<ArgumentException>("Result.AllSucceeded", () => Result.AllSucceeded(Result.Success(), null!), "null element");

// ---- Result<T> — value type, reference type, user-defined struct -------------
Result<int> okInt = Result<int>.Success(7);
Check("Result<T>.Success", okInt.Succeeded && okInt.Value == 7, "value type");
Result<string?> okString = Result<string>.Success("s");
Check("Result<T>.Success", okString.Succeeded && Same(okString.Value, "s"), "reference type");
Result<Point> okPoint = Result<Point>.Success(new Point(3, 4));
Check("Result<T>.Success", okPoint.Value == new Point(3, 4), "user-defined struct");
Check("Result<T>.Value", okInt.Value == 7 && Same(okString.Value, "s"), "returns the value on success");
Throws<InvalidOperationException>("Result<T>.Value", () => _ = Result<int>.Failure("no").Value, "throws on a failed result");
Result<int> badInt = Result<int>.Failure("nope-t");
Check("Result<T>.Failure", badInt.Failed && Same(badInt.ErrorMessage, "nope-t"), "value type");
Check("Result<T>.Failure", Result<string>.Failure("nope-s").Failed, "reference type");
Throws<ArgumentException>("Result<T>.Failure", () => Result<int>.Failure(""), "empty message");

if (failures.Count > 0)
{
    Console.Error.WriteLine(string.Create(CultureInfo.InvariantCulture, $"FAIL: {failures.Count} of {checks} AOT smoke checks failed:"));
    foreach (var failure in failures)
    {
        Console.Error.WriteLine($"  {failure}");
    }

    return 1;
}

Console.WriteLine(string.Create(CultureInfo.InvariantCulture, $"OK: {checks} AOT smoke checks passed under Native AOT."));
return 0;
