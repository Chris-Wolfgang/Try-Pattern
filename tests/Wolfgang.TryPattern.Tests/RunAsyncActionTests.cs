namespace Wolfgang.TryPattern.Tests;

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
        Assert.NotNull(result.ErrorMessage);
        Assert.Empty(result.ErrorMessage);

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
        Assert.NotNull(result.ErrorMessage);
        Assert.Empty(result.ErrorMessage);

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

        void Action()
        {
            actionStarted.TrySetResult(true);

            try
            {
                while (true)
                {
                    cts.Token.ThrowIfCancellationRequested();
                    Task.Delay(10).Wait();
                }
            }
            catch (OperationCanceledException)
            {
                cancellationObserved.TrySetResult(true);
                throw;
            }
        }

        var task = Try.RunAsync(Action, cts.Token);

        await actionStarted.Task;

        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => task);
        var completedTask = await Task.WhenAny(cancellationObserved.Task, Task.Delay(1000));
        Assert.Same(cancellationObserved.Task, completedTask);
    }
}
