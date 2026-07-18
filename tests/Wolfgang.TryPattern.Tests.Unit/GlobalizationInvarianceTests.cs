#if NET6_0_OR_GREATER
// Globalization / CultureInfo invariance per issue #182.
//
// Try-Pattern does not itself format numbers, dates, or use collation.
// The public surface is culture-invariant by contract: no method mutates
// its behaviour under tr-TR, de-DE, zh-CN, ar-SA, or ja-JP relative to
// en-US. This suite ASSERTS that by re-running a representative slice
// of the Try / Result surface under every hostile culture the fleet
// standard covers.
//
// Culture-sensitivity allowlist: EMPTY. If a future method introduces
// culture-dependent behaviour (e.g. formatting a number in an error
// message), it MUST be added to this doc comment before merging, and
// the corresponding test either updated to exclude it or explicitly
// asserted against invariant output.
//
// Each test swaps BOTH `CurrentCulture` and `CurrentUICulture` and
// restores them in `finally` so a xunit parallelism split doesn't
// leak culture across unrelated tests.

using System;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;

namespace Wolfgang.TryPattern.Tests.Unit;

public class GlobalizationInvarianceTests
{
    public static TheoryData<string> HostileCultures =>
        new()
        {
            "en-US",   // Baseline.
            "tr-TR",   // Dotted-I / dotless-I case bugs.
            "de-DE",   // Decimal-comma; ',' as group separator.
            "zh-CN",   // Simplified Chinese; collation.
            "ar-SA",   // RTL; Hindi-Arabic digit shapes.
            "ja-JP",   // Full-width digit shapes.
        };


    [Theory]
    [MemberData(nameof(HostileCultures))]
    public void Result_Success_and_Failure_are_culture_invariant(string cultureName)
    {
        WithCulture
        (
            cultureName,
            () =>
            {
                Result ok = Result.Success();
                Assert.True(ok.Succeeded);
                Assert.False(ok.Failed);

                Result bad = Result.Failure("boom");
                Assert.True(bad.Failed);
                Assert.Equal("boom", bad.ErrorMessage);
            }
        );
    }


    [Theory]
    [MemberData(nameof(HostileCultures))]
    public void Try_Run_Action_carries_exception_message_verbatim(string cultureName)
    {
        WithCulture
        (
            cultureName,
            () =>
            {
                Result r = Try.Run(() => throw new InvalidOperationException("boom"));

                Assert.True(r.Failed);
                Assert.Equal("boom", r.ErrorMessage);
            }
        );
    }


    [Theory]
    [MemberData(nameof(HostileCultures))]
    public void Try_Run_Generic_returns_value_unchanged(string cultureName)
    {
        WithCulture
        (
            cultureName,
            () =>
            {
                Result<int> r = Try.Run(() => 42);

                Assert.True(r.Succeeded);
                Assert.Equal(42, r.Value);
            }
        );
    }


    [Theory]
    [MemberData(nameof(HostileCultures))]
    public async Task Try_RunAsync_returns_success_under_hostile_culture(string cultureName)
    {
        await WithCultureAsync
        (
            cultureName,
            async () =>
            {
                Result r = await Try.RunAsync(() => { });
                Assert.True(r.Succeeded);
            }
        );
    }


    [Theory]
    [MemberData(nameof(HostileCultures))]
    public void Result_combinators_behave_identically_across_cultures(string cultureName)
    {
        WithCulture
        (
            cultureName,
            () =>
            {
                Assert.True(Result.AllSucceeded(Result.Success(), Result.Success()));
                Assert.True(Result.AnyFailed(Result.Success(), Result.Failure("f")));
                Assert.True(Result.Flatten(Result.Success(), Result.Success()).Succeeded);
                Assert.True(Result.Flatten(Result.Success(), Result.Failure("f")).Failed);
            }
        );
    }


    private static void WithCulture(string cultureName, Action body)
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        CultureInfo previousUI = CultureInfo.CurrentUICulture;
        try
        {
            var target = CultureInfo.GetCultureInfo(cultureName);
            CultureInfo.CurrentCulture = target;
            CultureInfo.CurrentUICulture = target;
            body();
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
            CultureInfo.CurrentUICulture = previousUI;
        }
    }


    private static async Task WithCultureAsync(string cultureName, Func<Task> body)
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        CultureInfo previousUI = CultureInfo.CurrentUICulture;
        try
        {
            var target = CultureInfo.GetCultureInfo(cultureName);
            CultureInfo.CurrentCulture = target;
            CultureInfo.CurrentUICulture = target;
            await body().ConfigureAwait(false);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
            CultureInfo.CurrentUICulture = previousUI;
        }
    }
}

#endif
