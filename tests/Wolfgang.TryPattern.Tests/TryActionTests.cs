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
        void Action() => executed = true;

        // Act
        var result = Try.Action(Action);

        // Assert
        Assert.True(executed);
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.NotNull(result.ErrorMessage);
        Assert.Empty(result.ErrorMessage);
    }



    [Fact]
    public void Action_WithExceptionThrowingAction_SwallowsException()
    {
        // Arrange
        static void Action() => throw new InvalidOperationException("Test exception");

        // Act
        var result = Try.Action(Action);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.NotNull(result.ErrorMessage);
        Assert.NotEmpty(result.ErrorMessage);
    }



    [Fact]
    public void Action_WithMultipleExceptions_SwallowsAllExceptions()
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
        Try.Action(Action1);
        Try.Action(Action2);
        Try.Action(Action3);

        // Assert
        Assert.Equal(3, callCount);
    }
}
