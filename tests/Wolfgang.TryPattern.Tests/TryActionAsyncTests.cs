namespace Wolfgang.TryPattern.Tests;

public class TryActionAsyncTests
{
    [Fact]
    public async Task RunAsync_Action_WithNullAction_ThrowsArgumentNullException()
    {
        // Arrange
        Action? nullAction = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => Try.RunAsync(nullAction!));
    }



    [Fact]
    public async Task RunAsync_Action_WithSuccessfulAction_ExecutesAction()
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
    public async Task RunAsync_Action_WithExceptionThrowingAction_SwallowsException()
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
    public async Task RunAsync_Action_WithSynchronousException_SwallowsException()
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
    public async Task RunAsync_Action__WithMultipleAsyncExceptions_SwallowsAllExceptions()
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
        Assert.Equal(3 ,callCount);
    }
}
