namespace Wolfgang.TryPattern.Tests;

public class TryActionTests
{
    [Fact]
    public void Run_Action_WithNullAction_ThrowsArgumentNullException()
    {
        // Arrange
        Action? nullAction = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Try.Run(nullAction!));
    }



    [Fact]
    public void Run_Action_WithSuccessfulAction_ExecutesAction()
    {
        // Arrange
        var executed = false;
        void Action() => executed = true;

        // Act
        var result = Try.Run(Action);

        // Assert
        Assert.True(executed);
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.NotNull(result.ErrorMessage);
        Assert.Empty(result.ErrorMessage);
    }



    [Fact]
    public void Run_Action_WithExceptionThrowingAction_SwallowsException()
    {
        // Arrange
        static void Action() => throw new InvalidOperationException("Test exception");

        // Act
        var result = Try.Run(Action);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.NotNull(result.ErrorMessage);
        Assert.NotEmpty(result.ErrorMessage);
    }



    [Fact]
    public void Run_Action_WithMultipleExceptions_SwallowsAllExceptions()
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
        Try.Run(Action1);
        Try.Run(Action2);
        Try.Run(Action3);

        // Assert
        Assert.Equal(3, callCount);
    }
}
