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
}
