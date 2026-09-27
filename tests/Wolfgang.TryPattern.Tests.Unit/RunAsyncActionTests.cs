using System;
using System.Threading;
using System.Threading.Tasks;

namespace Wolfgang.TryPattern.Tests.Unit;

public class RunAsyncActionTests
{
    [Fact]
    public async Task RunAsync_Action_when_passed_null_throws_ArgumentNullException()
    {
        // Arrange
        Action? nullAction = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => Try.RunAsync(nullAction!));
    }



    [Fact]
    public async Task RunAsync_Action_executes_specified_action()
    {
        // Arrange
        var executed = false;

        void Action()
        {
            executed = true;
        }

        // Act
        var result = await Try.RunAsync(Action);

        // Assert
        Assert.True(executed);
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.ErrorMessage);

    }

    

    [Fact]
    public async Task RunAsync_Action_when_action_throws_exception_swallows_the_exception()
    {
        // Arrange
        static void Action ()
        {
            throw new InvalidOperationException("Test exception");
        }

        // Act
        var result = await Try.RunAsync(Action);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.NotNull(result.ErrorMessage);
        Assert.NotEmpty(result.ErrorMessage);
    }


    
    [Fact]
    public async Task RunAsync_Action_when_passed_synchronous_action_that_throws_exception_swallows_the_exception()
    {
        // Arrange
        static void Action() => throw new InvalidOperationException("Synchronous exception");

        // Act
        var result = await Try.RunAsync(Action);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.NotNull(result.ErrorMessage);
        Assert.NotEmpty(result.ErrorMessage);
    }



    [Fact]
    public async Task RunAsync_Action_when_called_multiple_times_with_action_that_throws_exception_swallows_all_exceptions()
    {
        // Arrange
        var callCount = 0;

        void Action1()
        {
            callCount++;
            throw new ArgumentException();
        }

        void Action2()
        {
            callCount++;
            throw new InvalidOperationException();
        }

        void Action3()
        {
            callCount++;
            throw new NullReferenceException();
        }

        // Act
        await Try.RunAsync(Action1);
        await Try.RunAsync(Action2);
        await Try.RunAsync(Action3);

        // Assert
        Assert.Equal(3, callCount);
    }
    


    [Fact]
    public async Task RunAsync_Action_CancellationToken_when_passed_null_throws_ArgumentNullException()
    {
        // Arrange
        Action? nullAction = null;

        var cts = new CancellationTokenSource();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => Try.RunAsync(nullAction!, cts.Token));
    }



    [Fact]
    public async Task RunAsync_Action_CancellationToken_executes_specified_action()
    {
        // Arrange
        var executed = false;

        void Action()
        {
            executed = true;
        }

        var cts = new CancellationTokenSource();

        // Act
        var result = await Try.RunAsync(Action, cts.Token);

        // Assert
        Assert.True(executed);
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.ErrorMessage);

    }



    [Fact]
    public async Task RunAsync_Action_CancellationToken_when_action_throws_exception_swallows_the_exception()
    {
        // Arrange
        static void Action()
        {
            throw new InvalidOperationException("Test exception");
        }

        var cts = new CancellationTokenSource();


        // Act
        var result = await Try.RunAsync(Action, cts.Token);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.NotNull(result.ErrorMessage);
        Assert.NotEmpty(result.ErrorMessage);
    }



    [Fact]
    public async Task RunAsync_Action_CancellationToken_when_passed_synchronous_action_that_throws_exception_swallows_the_exception()
    {
        // Arrange
        static void Action() => throw new InvalidOperationException("Synchronous exception");

        var cts = new CancellationTokenSource();


        // Act
        var result = await Try.RunAsync(Action, cts.Token);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.NotNull(result.ErrorMessage);
        Assert.NotEmpty(result.ErrorMessage);
    }



    [Fact]
    public async Task RunAsync_Action_CancellationToken_when_called_multiple_times_with_action_that_throws_exception_swallows_all_exceptions()
    {
        // Arrange
        var callCount = 0;

        void Action1()
        {
            callCount++;
            throw new ArgumentException();
        }

        void Action2()
        {
            callCount++;
            throw new InvalidOperationException();
        }

        void Action3()
        {
            callCount++;
            throw new NullReferenceException();
        }

        var cts = new CancellationTokenSource();

        // Act
        await Try.RunAsync(Action1, cts.Token);
        await Try.RunAsync(Action2, cts.Token);
        await Try.RunAsync(Action3, cts.Token);

        // Assert
        Assert.Equal(3, callCount);
    }



    [Fact]
    public async Task RunAsync_Action_CancellationToken_when_cancellation_is_requested_after_action_started_the_action_is_cancelled()
    {
        using var cts = new CancellationTokenSource();
        var actionStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var cancellationObserved = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        // Straight-line, ending in an unconditional `throw`, so every line runs in
        // every interleaving and the compiler emits no normal-exit sequence point.
        // A polling loop left a line (the first delay, later the loop-back)
        // uncovered whenever a fast runner cancelled early, and a trailing
        // ThrowIfCancellationRequested left the closing brace uncovered; either
        // fails the 100% test-assembly gate. WaitOne returns true only once the
        // token is cancelled; the timeout turns a cancellation that never
        // arrives into a failed assertion instead of a hang.
        void Action()
        {
            actionStarted.TrySetResult(true);
            cancellationObserved.TrySetResult(cts.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(10)));
            throw new OperationCanceledException(cts.Token);
        }

        var task = Try.RunAsync(Action, cts.Token);

        await actionStarted.Task;

        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => task);
        Assert.True(await cancellationObserved.Task, "the action never observed the cancellation");
    }
}
