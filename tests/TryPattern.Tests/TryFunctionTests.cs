namespace TryPattern.Tests;

public class TryFunctionTests
{
    [Fact]
    public void Function_WithNullFunction_ThrowsArgumentNullException()
    {
        // Arrange
        Func<int>? nullFunction = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Try.Function(nullFunction!));
    }

    [Fact]
    public void Function_WithSuccessfulFunction_ReturnsResult()
    {
        // Arrange
        const int expectedValue = 42;
        Func<int> function = () => expectedValue;

        // Act
        var result = Try.Function(function);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Function_WithStringFunction_ReturnsResult()
    {
        // Arrange
        const string expectedValue = "Hello, World!";
        Func<string> function = () => expectedValue;

        // Act
        var result = Try.Function(function);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Function_WithExceptionThrowingFunction_ReturnsDefault()
    {
        // Arrange
        Func<int> function = () => throw new InvalidOperationException("Test exception");

        // Act
        var result = Try.Function(function);

        // Assert
        Assert.Equal(default(int), result);
    }

    [Fact]
    public void Function_WithExceptionThrowingStringFunction_ReturnsNull()
    {
        // Arrange
        Func<string> function = () => throw new InvalidOperationException("Test exception");

        // Act
        var result = Try.Function(function);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Function_WithComplexObject_ReturnsResult()
    {
        // Arrange
        var expectedObject = new { Name = "Test", Value = 100 };
        Func<object> function = () => expectedObject;

        // Act
        var result = Try.Function(function);

        // Assert
        Assert.Equal(expectedObject, result);
    }

    [Fact]
    public void Function_WithMultipleCalls_HandlesEachIndependently()
    {
        // Arrange
        var callCount = 0;
        Func<int> successFunction = () => ++callCount;
        Func<int> failFunction = () =>
        {
            callCount++;
            throw new Exception();
        };

        // Act
        var result1 = Try.Function(successFunction);
        var result2 = Try.Function(failFunction);
        var result3 = Try.Function(successFunction);

        // Assert
        Assert.Equal(1, result1);
        Assert.Equal(0, result2); // Default int value
        Assert.Equal(3, result3);
        Assert.Equal(3, callCount);
    }
}
