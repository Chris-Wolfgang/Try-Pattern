// Long-run fuzz variant of PropertyTests per issue #170.
//
// Same properties as PropertyTests, but with `MaxTest = 100_000`
// (vs the default 100) so the scheduled fuzz workflow explores far
// more of the input space. Every theorem carries
// `[Trait("Category", "Fuzz")]` so PR-gate `dotnet test` runs can
// exclude this class via `--filter "Category!=Fuzz"` and keep the
// per-PR budget under a second.
//
// The scheduled workflow (.github/workflows/fuzz.yaml) runs
// `--filter "Category=Fuzz"` and, on any failure, opens a
// `kind:fuzz-finding` issue with the failing seed + counter-example
// pulled from the test output.

using System;
using System.Diagnostics.CodeAnalysis;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

namespace Wolfgang.TryPattern.Tests.Unit;

// Never executed by the PR gate (`--filter "Category!=Fuzz"`), only by the
// scheduled fuzz.yaml run, so the instrumented test assembly would score it
// at 0% on every PR. The same properties ARE measured through PropertyTests.
[ExcludeFromCodeCoverage]
[Trait("Category", "Fuzz")]
public class FuzzTests
{
    private const int LongRunMaxTest = 100_000;


    [Property(MaxTest = LongRunMaxTest)]
    public Property Fuzz_Try_Run_of_non_throwing_action_always_succeeds()
    {
        return Prop.ForAll<int>
        (
            x =>
            {
                int sink = 0;
                Result r = Try.Run(() => { sink = x; });
                return r.Succeeded && sink == x;
            }
        );
    }


    [Property(MaxTest = LongRunMaxTest)]
    public Property Fuzz_Try_Run_of_throwing_action_carries_message(NonEmptyString message)
    {
        if (message is null) throw new ArgumentNullException(nameof(message));
        // NonEmptyString can still be whitespace-only. Since #273 Try.Run
        // reports such a message as the exception's type name, because
        // Result.Failure rejects whitespace (same oracle as PropertyTests).
        string msg = message.Get;
        string expected = string.IsNullOrWhiteSpace(msg) ? nameof(InvalidOperationException) : msg;
        Result r = Try.Run(() => throw new InvalidOperationException(msg));
        return (r.Failed && string.Equals(r.ErrorMessage, expected, StringComparison.Ordinal)).ToProperty();
    }


    [Property(MaxTest = LongRunMaxTest)]
    public Property Fuzz_Try_Run_generic_returns_input_value(int value)
    {
        Result<int> r = Try.Run(() => value);
        return (r.Succeeded && r.Value == value).ToProperty();
    }


    [Property(MaxTest = LongRunMaxTest)]
    public Property Fuzz_AllSucceeded_is_negation_of_AnyFailed(bool[] successFlags)
    {
        if (successFlags is null || successFlags.Length == 0)
        {
            return true.ToProperty();
        }

        Result[] results = new Result[successFlags.Length];
        for (int i = 0; i < successFlags.Length; i++)
        {
            results[i] = successFlags[i]
                ? Result.Success()
                : Result.Failure($"f{i}");
        }

        bool allOk = Result.AllSucceeded(results);
        bool anyFail = Result.AnyFailed(results);
        return (allOk == !anyFail).ToProperty();
    }


    [Property(MaxTest = LongRunMaxTest)]
    public Property Fuzz_Flatten_of_all_successes_is_success(NonNegativeInt count)
    {
        if (count is null) throw new ArgumentNullException(nameof(count));
        int n = Math.Min(count.Get, 64);
        Result[] results = new Result[n];
        for (int i = 0; i < n; i++)
        {
            results[i] = Result.Success();
        }

        Result flat = Result.Flatten(results);
        return flat.Succeeded.ToProperty();
    }


    [Property(MaxTest = LongRunMaxTest)]
    public Property Fuzz_Flatten_with_at_least_one_failure_is_failure(int[] failIndices)
    {
        if (failIndices is null || failIndices.Length == 0)
        {
            return true.ToProperty();
        }

        int n = failIndices.Length;
        Result[] results = new Result[n];
        for (int i = 0; i < n; i++)
        {
            results[i] = failIndices[i] % 2 == 0
                ? Result.Failure($"f{i}")
                : Result.Success();
        }

        results[0] = Result.Failure("guaranteed");

        Result flat = Result.Flatten(results);
        return flat.Failed.ToProperty();
    }
}
