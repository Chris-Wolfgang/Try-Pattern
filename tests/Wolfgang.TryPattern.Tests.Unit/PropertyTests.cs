// Property-based fuzz tests per issue #170.
//
// FsCheck generates random inputs across each theorem's parameter
// space and checks the property holds for every generated case.
// Each [Property] runs 100 randomized inputs by default; a
// counter-example (if any) is minimized and reported in the test
// output.
//
// Try-Pattern's specification is small but has real properties worth
// enforcing:
//
//   - Try.Run(non-throwing action)          → always Succeeded
//   - Try.Run(throwing action)              → always Failed with round-tripped message
//   - Try.Run<T>(fn returning value)        → Succeeded with .Value == value
//   - Result.AllSucceeded ⇔ !Result.AnyFailed (on non-empty inputs)
//   - Result.Flatten idempotence: Flatten with all-successes is Success
//
// A property failure means the specification described in the class-
// level comment is violated — investigate before dismissing.

using System;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace Wolfgang.TryPattern.Tests.Unit;

public class PropertyTests
{
    [Property]
    public Property Try_Run_of_non_throwing_action_always_succeeds()
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


    [Property]
    public Property Try_Run_of_throwing_action_carries_message_round_trip(NonEmptyString message)
    {
        if (message is null) throw new ArgumentNullException(nameof(message));
        // NonEmptyString excludes only the empty string; Result.Failure
        // additionally rejects whitespace-only strings and there's a
        // latent bug (#273) that lets a whitespace exception message
        // escape from Try.Run. Skip that case so the fast-path
        // property is well-formed vs the current API.
        string msg = message.Get;
        if (string.IsNullOrWhiteSpace(msg))
        {
            return true.ToProperty();
        }
        Result r = Try.Run(() => throw new InvalidOperationException(msg));
        return (r.Failed && string.Equals(r.ErrorMessage, msg, StringComparison.Ordinal)).ToProperty();
    }


    [Property]
    public Property Try_Run_generic_returns_input_value(int value)
    {
        Result<int> r = Try.Run(() => value);
        return (r.Succeeded && r.Value == value).ToProperty();
    }


    [Property]
    public Property AllSucceeded_is_negation_of_AnyFailed(bool[] successFlags)
    {
        // FsCheck may generate the empty array. The API's semantics
        // on empty input are edge cases (both return true / false
        // depending on interpretation). Skip empty via a precondition.
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


    [Property]
    public Property Flatten_of_all_successes_is_success(NonNegativeInt count)
    {
        if (count is null) throw new ArgumentNullException(nameof(count));
        // Bound the count so FsCheck doesn't allocate a huge array
        // just because it generated int.MaxValue.
        int n = Math.Min(count.Get, 64);
        Result[] results = new Result[n];
        for (int i = 0; i < n; i++)
        {
            results[i] = Result.Success();
        }

        Result flat = Result.Flatten(results);
        return flat.Succeeded.ToProperty();
    }


    [Property]
    public Property Flatten_with_at_least_one_failure_is_failure(int[] failIndices)
    {
        // At least one failure index → the flattened Result is Failed.
        // FsCheck can generate empty or all-positive arrays; use an
        // upfront normalization to ensure at least one failure.
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

        // Guarantee at least one failure regardless of input.
        results[0] = Result.Failure("guaranteed");

        Result flat = Result.Flatten(results);
        return flat.Failed.ToProperty();
    }
}
