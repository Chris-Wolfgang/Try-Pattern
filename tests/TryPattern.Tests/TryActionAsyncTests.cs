namespace TryPattern.Tests;

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
        Func<Task> action = async () =>
        {
            await Task.Delay(10);
            executed = true;
        };

        // Act
        await Try.ActionAsync(action);

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public async Task ActionAsync_WithExceptionThrowingAction_SwallowsException()
    {
        // Arrange
        Func<Task> action = async () =>
        {
            await Task.Delay(10);
            throw new InvalidOperationException("Test exception");
        };

        // Act & Assert - Should not throw
        var exception = await Record.ExceptionAsync(() => Try.ActionAsync(action));
        Assert.Null(exception);
    }

    [Fact]
    public async Task ActionAsync_WithSynchronousException_SwallowsException()
    {
        // Arrange
        Func<Task> action = () => throw new InvalidOperationException("Synchronous exception");

        // Act & Assert - Should not throw
        var exception = await Record.ExceptionAsync(() => Try.ActionAsync(action));
        Assert.Null(exception);
    }

    [Fact]
    public async Task ActionAsync_WithMultipleAsyncExceptions_SwallowsAllExceptions()
    {
        // Arrange
        var callCount = 0;
        Func<Task> action1 = async () =>
        {
            await Task.Delay(10);
            callCount++;
            throw new ArgumentException();
        };
        Func<Task> action2 = async () =>
        {
            await Task.Delay(10);
            callCount++;
            throw new InvalidOperationException();
        };
        Func<Task> action3 = async () =>
        {
            await Task.Delay(10);
            callCount++;
            throw new NullReferenceException();
        };

        // Act
        await Try.ActionAsync(action1);
        await Try.ActionAsync(action2);
        await Try.ActionAsync(action3);

        // Assert
        Assert.Equal(3, callCount);
    }
}
