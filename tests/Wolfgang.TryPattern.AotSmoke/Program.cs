// Native AOT / trimming smoke test for the AOT-safe public surface of
// Wolfgang.TryPattern. Published with PublishAot + PublishTrimmed (see
// the .csproj) and run by CI: a trim/AOT-unsafe regression makes the
// analyzers warn (TreatWarningsAsErrors → build fails), and a runtime
// break (MissingMethodException / NotSupportedException / silent no-op)
// makes this program exit non-zero.
//
// Try-Pattern has no reflection, no dynamic-code paths, no serialization —
// the entire public surface is expected to be AOT-safe. Every public
// method is exercised at least once below, both success and failure
// paths, so a future refactor that inadvertently pulls in a
// reflection-dependent code path is caught here.

using System.Globalization;
using Wolfgang.TryPattern;

int expected = 0;
int actual = 0;

// Try.Run(Action) — success + failure.
Result r1 = Try.Run(() => { });
expected++; if (r1.Succeeded) actual++;

Result r2 = Try.Run(() => throw new InvalidOperationException("boom"));
expected++; if (r2.Failed && string.Equals(r2.ErrorMessage, "boom", StringComparison.Ordinal)) actual++;

// Try.Run<T>(Func<T>) — success + failure. T=int under nullable-enable and
// unconstrained T maps `Result<T?>` back to Result<int> (int? in a
// value-type context with no `where T : struct`).
Result<int> r3 = Try.Run(() => 42);
expected++; if (r3.Succeeded && r3.Value == 42) actual++;

Result<int> r4 = Try.Run<int>(() => throw new InvalidOperationException("boom-t"));
expected++; if (r4.Failed && string.Equals(r4.ErrorMessage, "boom-t", StringComparison.Ordinal)) actual++;

// Try.RunAsync(Action, CancellationToken) — success + failure.
Result r5 = await Try.RunAsync(() => { });
expected++; if (r5.Succeeded) actual++;

Result r6 = await Try.RunAsync(() => throw new InvalidOperationException("async-boom"));
expected++; if (r6.Failed && string.Equals(r6.ErrorMessage, "async-boom", StringComparison.Ordinal)) actual++;

// Try.RunAsync<T>(Func<Task<T>>, CancellationToken) — success + failure.
Result<string?> r7 = await Try.RunAsync<string>(() => Task.FromResult<string?>("ok"));
expected++; if (r7.Succeeded && string.Equals(r7.Value, "ok", StringComparison.Ordinal)) actual++;

Result<string?> r8 = await Try.RunAsync<string>(() => throw new InvalidOperationException("async-boom-t"));
expected++; if (r8.Failed && string.Equals(r8.ErrorMessage, "async-boom-t", StringComparison.Ordinal)) actual++;

// Result static factories + combinators.
Result r9 = Result.Success();
expected++; if (r9.Succeeded) actual++;

Result r10 = Result.Failure("nope");
expected++; if (r10.Failed && string.Equals(r10.ErrorMessage, "nope", StringComparison.Ordinal)) actual++;

Result r11 = Result.Flatten(Result.Success(), Result.Success());
expected++; if (r11.Succeeded) actual++;

Result r12 = Result.Flatten(Result.Success(), Result.Failure("nope-flat"));
expected++; if (r12.Failed) actual++;

expected++; if (Result.AnyFailed(Result.Success(), Result.Failure("f"))) actual++;
expected++; if (Result.AllSucceeded(Result.Success(), Result.Success())) actual++;

// Result<T> factories.
Result<int> r13 = Result<int>.Success(7);
expected++; if (r13.Succeeded && r13.Value == 7) actual++;

Result<int> r14 = Result<int>.Failure("nope-t");
expected++; if (r14.Failed && string.Equals(r14.ErrorMessage, "nope-t", StringComparison.Ordinal)) actual++;

if (actual != expected)
{
    System.Console.Error.WriteLine(string.Create(CultureInfo.InvariantCulture, $"FAIL: expected {expected} AOT-safe assertions, got {actual}."));
    return 1;
}

System.Console.WriteLine(string.Create(CultureInfo.InvariantCulture, $"OK: AOT-safe surface passed {actual} assertions under Native AOT."));
return 0;
