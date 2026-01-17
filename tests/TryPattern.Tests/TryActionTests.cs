#if !NET6_0_OR_GREATER
using System;
#endif

namespace TryPattern.Tests;

public class TryActionTests
{
    [Fact]
    public void Action_WithNullAction_ThrowsArgumentNullException()
    {
        // Arrange
        Action? nullAction = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Try.Action(nullAction!));
    }

    [Fact]
    public void Action_WithSuccessfulAction_ExecutesAction()
    {
        // Arrange
        var executed = false;
        Action action = () => executed = true;

        // Act
        Try.Action(action);

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public void Action_WithExceptionThrowingAction_SwallowsException()
    {
        // Arrange
        Action action = () => throw new InvalidOperationException("Test exception");

        // Act & Assert - Should not throw
        var exception = Record.Exception(() => Try.Action(action));
        Assert.Null(exception);
    }

    [Fact]
    public void Action_WithMultipleExceptions_SwallowsAllExceptions()
    {
        // Arrange
        var callCount = 0;
        Action action1 = () =>
        {
            callCount++;
            throw new ArgumentException();
        };
        Action action2 = () =>
        {
            callCount++;
            throw new InvalidOperationException();
        };
        Action action3 = () =>
        {
            callCount++;
            throw new NullReferenceException();
        };

        // Act
        Try.Action(action1);
        Try.Action(action2);
        Try.Action(action3);

        // Assert
        Assert.Equal(3, callCount);
    }
}
