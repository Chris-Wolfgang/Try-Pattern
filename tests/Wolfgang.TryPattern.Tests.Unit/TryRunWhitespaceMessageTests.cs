// Regression tests for #273 — Try.Run must not throw when the caught
// exception's Message is null / empty / whitespace. Before the fix,
// these calls raised ArgumentException from inside Result.Failure
// (which rejects whitespace) and defeated the whole point of the Try
// wrapper. After the fix, Try.Run falls back to the exception's type
// name so callers always get a Failed Result.

using System;
using System.Threading.Tasks;

namespace Wolfgang.TryPattern.Tests.Unit;

public class TryRunWhitespaceMessageTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Run_Action_when_exception_message_is_whitespace_returns_Failed(string message)
    {
        Result r = Try.Run(() => throw new InvalidOperationException(message));

        Assert.True(r.Failed);
        Assert.Equal(nameof(InvalidOperationException), r.ErrorMessage);
    }


    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Run_Generic_when_exception_message_is_whitespace_returns_Failed(string message)
    {
        Result<int> r = Try.Run<int>(() => throw new InvalidOperationException(message));

        Assert.True(r.Failed);
        Assert.Equal(nameof(InvalidOperationException), r.ErrorMessage);
    }


    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task RunAsync_Action_when_exception_message_is_whitespace_returns_Failed(string message)
    {
        Result r = await Try.RunAsync(() => throw new InvalidOperationException(message));

        Assert.True(r.Failed);
        Assert.Equal(nameof(InvalidOperationException), r.ErrorMessage);
    }


    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task RunAsync_Generic_when_exception_message_is_whitespace_returns_Failed(string message)
    {
        Result<int> r = await Try.RunAsync<int>(() => throw new InvalidOperationException(message));

        Assert.True(r.Failed);
        Assert.Equal(nameof(InvalidOperationException), r.ErrorMessage);
    }


    [Fact]
    public void Run_Action_when_exception_message_is_non_whitespace_preserves_it()
    {
        // Regression guard: the fallback path must not kick in when
        // the exception carries a real message.
        Result r = Try.Run(() => throw new InvalidOperationException("real message"));

        Assert.True(r.Failed);
        Assert.Equal("real message", r.ErrorMessage);
    }
}
