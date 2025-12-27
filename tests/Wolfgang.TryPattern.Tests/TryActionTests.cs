namespace Wolfgang.TryPattern.Tests;

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
        var result = Try.Action(action);

        // Assert
        Assert.True(executed);
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.Exception);
    }



    [Fact]
    public void Action_WithExceptionThrowingAction_SwallowsException()
    {
        // Arrange
        Action action = () => throw new InvalidOperationException("Test exception");

        // Act
        var result = Try.Action(action);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.NotNull(result.Exception);
    }



    [Fact]
    public void Action_WithMultipleExceptions_SwallowsAllExceptions()
    {
        // Arrange
        var callCount = 0;
        var action1 = () =>
        {
            callCount++;
            throw new ArgumentException();
        };
        var action2 = () =>
        {
            callCount++;
            throw new InvalidOperationException();
        };
        var action3 = () =>
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
