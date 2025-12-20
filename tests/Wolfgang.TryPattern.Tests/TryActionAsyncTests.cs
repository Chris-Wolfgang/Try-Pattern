namespace Wolfgang.TryPattern.Tests;

public class TryActionAsyncTests
{
    [Fact]
    public async Task ActionAsync_WithNullAction_ThrowsArgumentNullException()
    {
        // Arrange
        Func<Task>? nullAction = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => Try.ActionAsync(nullAction!));
    }



    [Fact]
    public async Task ActionAsync_WithSuccessfulAction_ExecutesAction()
    {
        // Arrange
        var executed = false;
        var action = async () =>
        {
            await Task.Delay(10);
            executed = true;
        };

        // Act
        var result = await Try.ActionAsync(action);

        // Assert
        Assert.True(executed);
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.Exception);

    }

    

    [Fact]
    public async Task ActionAsync_WithExceptionThrowingAction_SwallowsException()
    {
        // Arrange
        var action = async () =>
        {
            await Task.Delay(10);
            throw new InvalidOperationException("Test exception");
        };

        // Act
        var result = await Try.ActionAsync(action);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.NotNull(result.Exception);
    }




    [Fact]
    public async Task ActionAsync_WithSynchronousException_SwallowsException()
    {
        // Arrange
        Func<Task> action = () => throw new InvalidOperationException("Synchronous exception");

        // Act
        var result = await Try.ActionAsync(action);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.NotNull(result.Exception);
    }



    [Fact]
    public async Task ActionAsync_WithMultipleAsyncExceptions_SwallowsAllExceptions()
    {
        // Arrange
        var callCount = 0;

        var action1 = async () =>
        {
            await Task.Delay(10);
            callCount++;
            throw new ArgumentException();
        };
        var action2 = async () =>
        {
            await Task.Delay(10);
            callCount++;
            throw new InvalidOperationException();
        };
        var action3 = async () =>
        {
            await Task.Delay(10);
            callCount++;
            throw new NullReferenceException();
        };

        // Act
        await Try.ActionAsync(action1);
        await Try.ActionAsync(action2);
        await Try.ActionAsync(action3);
        Assert.Equal(3 ,callCount);
    }
}
