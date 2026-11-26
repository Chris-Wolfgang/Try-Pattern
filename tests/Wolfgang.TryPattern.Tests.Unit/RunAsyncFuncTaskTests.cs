using System;
using System.Threading;
using System.Threading.Tasks;

namespace Wolfgang.TryPattern.Tests.Unit;

public class RunAsyncFuncTaskTests
{
    [Fact]
    public async Task RunAsync_FuncTask_when_passed_null_throws_ArgumentNullException()
    {
        // Arrange
        Func<Task>? nullFunction = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => Try.RunAsync(nullFunction!, TestCancellation.Token));
    }



    [Fact]
    public async Task RunAsync_FuncTask_when_successful_returns_successful_Result_after_the_work_completes()
    {
        // Arrange
        var completed = false;

        async Task Function()
        {
            await Task.Yield();
            completed = true;
        }

        // Act
        var result = await Try.RunAsync((Func<Task>)Function, TestCancellation.Token);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Null(result.ErrorMessage);
        Assert.True(completed, "the returned task must be awaited before the Result is produced");
    }



    [Fact]
    public async Task RunAsync_FuncTask_when_function_throws_after_an_await_returns_failed_Result()
    {
        // Arrange
        static async Task Function()
        {
            await Task.Yield();
            throw new InvalidOperationException("Test exception");
        }

        // Act
        var result = await Try.RunAsync((Func<Task>)Function, TestCancellation.Token);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("Test exception", result.ErrorMessage);
    }



    [Fact]
    public async Task RunAsync_FuncTask_when_function_throws_synchronously_returns_failed_Result()
    {
        // Arrange
        static Task Function() => throw new InvalidOperationException("Test exception");

        // Act
        var result = await Try.RunAsync((Func<Task>)Function, TestCancellation.Token);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("Test exception", result.ErrorMessage);
    }



    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task RunAsync_FuncTask_when_exception_message_is_whitespace_returns_type_name(string message)
    {
        // Arrange
        Task Function() => Task.FromException(new InvalidOperationException(message));

        // Act
        var result = await Try.RunAsync((Func<Task>)Function, TestCancellation.Token);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal(nameof(InvalidOperationException), result.ErrorMessage);
    }



    [Fact]
    public async Task RunAsync_FuncTask_when_token_is_already_canceled_throws_OperationCanceledException_without_invoking_function()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var wasInvoked = false;

        Task Function()
        {
            wasInvoked = true;
            return Task.CompletedTask;
        }

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => Try.RunAsync((Func<Task>)Function, cts.Token));
        Assert.False(wasInvoked, "function should not be invoked when the token is already canceled");

        // Positive control: the probe does flip when the function runs.
        await Function();
        Assert.True(wasInvoked);
    }



    [Fact]
    public async Task RunAsync_FuncTask_when_function_observes_cancellation_rethrows_OperationCanceledException()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var functionStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var pending = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var registration = cts.Token.Register(() => pending.TrySetCanceled(cts.Token));

        Task Function()
        {
            functionStarted.TrySetResult(true);
            return pending.Task;
        }

        var task = Try.RunAsync((Func<Task>)Function, cts.Token);

        // Act
        await functionStarted.Task;
        cts.Cancel();

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => task);
    }



    [Fact]
    public async Task RunAsync_when_passed_async_lambda_without_result_returns_failed_Result_instead_of_async_void()
    {
        // Act: the async lambda must bind to RunAsync(Func<Task>), not RunAsync(Action).
        var result = await Try.RunAsync(async () =>
        {
            await Task.Yield();
            throw new InvalidOperationException("Test exception");
        }, TestCancellation.Token);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("Test exception", result.ErrorMessage);
    }



    [Fact]
    public async Task RunAsync_when_passed_throw_expression_lambda_returns_failed_Result()
    {
        // Act: a throw-expression lambda prefers the Task-returning overload over Action;
        // either way the outcome must be a failed Result.
        var result = await Try.RunAsync(() => throw new InvalidOperationException("Test exception"), TestCancellation.Token);

        // Assert
        Assert.True(result.Failed);
        Assert.Equal("Test exception", result.ErrorMessage);
    }



    [Fact]
    public async Task RunAsync_when_passed_non_async_lambda_with_unreachable_end_runs_it_synchronously_on_the_caller()
    {
        // Arrange: a worker-loop lambda (no async, end not reachable) binds to RunAsync(Func<Task>),
        // not RunAsync(Action), so it is invoked on the caller's thread instead of through Task.Run.
        var callerThread = Environment.CurrentManagedThreadId;
        var delegateThread = -1;
        var iterations = 0;

        // Act
        var task = Try.RunAsync(() =>
        {
            delegateThread = Environment.CurrentManagedThreadId;
            while (true)
            {
                if (++iterations == 3)
                {
                    throw new InvalidOperationException("Loop ended");
                }
            }
        }, TestCancellation.Token);

        // Assert
        Assert.True(task.IsCompleted, "the loop must have run to its end before RunAsync returned");
        Assert.Equal(callerThread, delegateThread);
        var result = await task;
        Assert.True(result.Failed);
        Assert.Equal("Loop ended", result.ErrorMessage);
    }
}
