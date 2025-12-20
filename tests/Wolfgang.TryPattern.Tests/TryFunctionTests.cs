namespace Wolfgang.TryPattern.Tests;

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
    public void Function_WithSuccessfulFunctionOfInt_ReturnsResult()
    {
        // Arrange
        const int expectedValue = 42;
        var function = () => expectedValue;

        // Act
        var result = Try.Function(function);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.Exception);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public void Function_WithSuccessfulFunctionOfNullableInt_ReturnsResult()
    {
        // Arrange
        var expectedValue = 42;
        var function = () => expectedValue;

        // Act
        var result = Try.Function(function);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.Exception);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public void Function_WithStringFunction_ReturnsResult()
    {
        // Arrange
        const string expectedValue = "Hello, World!";
        var function = () => expectedValue;

        // Act
        var result = Try.Function(function);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.Exception);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public void Function_WithNullableStringFunction_ReturnsResult()
    {
        // Arrange
        const string expectedValue = "Hello, World!";
        var function = () => expectedValue;

        // Act
        var result = Try.Function(function);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.Exception);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public void Function_WithObjectFunction_ReturnsResult()
    {
        // Arrange
        var expectedValue = new object();
        var function = () => expectedValue;

        // Act
        var result = Try.Function(function);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.Exception);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public void FunctionInt_WithExceptionThrowingFunction_ReturnsDefault()
    {
        // Arrange
        Func<int> function = () => throw new InvalidOperationException("Test exception");

        // Act
        var result = Try.Function(function);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.NotNull(result.Exception);
        Assert.Equal("Test exception", result.Exception.Message);
        Assert.Equal(0, result.Value);
    }



    [Fact]
    public void FunctionNullableInt_WithExceptionThrowingFunction_ReturnsDefault()
    {
        // Arrange
        Func<int?> function = () => throw new InvalidOperationException("Test exception");

        // Act
        var result = Try.Function(function);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.NotNull(result.Exception);
        Assert.Equal("Test exception", result.Exception.Message);
        Assert.Null(result.Value);
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
        Assert.Equal(expectedObject, result.Value);
    }



    [Fact]
    public void Function_WithMultipleCalls_HandlesEachIndependently()
    {
        // Arrange
        var callCount = 0;
        var successFunction = () => ++callCount;
        var failFunction = new Func<int>
        (
            () =>
                {
                    callCount++;
                    throw new Exception();
                }
            );

        // Act
        var result1 = Try.Function(successFunction);
        var result2 = Try.Function(failFunction);
        var result3 = Try.Function(successFunction);

        // Assert
        Assert.Equal(1, result1.Value);
        Assert.Equal(0, result2.Value); // Default int value
        Assert.Equal(3, result3.Value);
        Assert.Equal(3, callCount);
    }
}
