using System.Threading;

namespace Wolfgang.TryPattern.Tests.Unit;

/// <summary>
/// The cancellation token tests pass to calls that accept one.
/// </summary>
/// <remarks>
/// These files compile in two projects: Wolfgang.TryPattern.Tests.Unit (xUnit v2, which has no
/// <c>TestContext</c>) and Wolfgang.TryPattern.Tests.Aot (xUnit v3 AOT, which defines
/// <c>XUNIT_V3</c>). Under v3 the token is the running test's, so a canceled or timed-out run
/// stops the call (and analyzer rule xUnit1051 is met); under v2 it is
/// <see cref="CancellationToken.None"/>, the same as passing no token.
/// </remarks>
internal static class TestCancellation
{
    /// <summary>
    /// Gets the current test's cancellation token under xUnit v3, otherwise
    /// <see cref="CancellationToken.None"/>.
    /// </summary>
#if XUNIT_V3
    internal static CancellationToken Token => TestContext.Current.CancellationToken;
#else
    internal static CancellationToken Token => CancellationToken.None;
#endif
}
