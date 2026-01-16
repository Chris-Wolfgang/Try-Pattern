namespace Wolfgang.TryPattern.Tests;

public class TryFunctionTests  // TODO Rename class and file
{
    [Fact]
    public void Run_Func_WithNullFunction_ThrowsArgumentNullException()
    {
        // Arrange
        Func<int>? nullFunction = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Try.Run(nullFunction!));
    }



    [Fact]
    public void Run_Func_WithSuccessfulFunctionOfInt_ReturnsResult()
    {
        // Arrange
        const int expectedValue = 42;
        static int Function() => expectedValue;

        // Act
        var result = Try.Run(Function);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Empty(result.ErrorMessage!);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public void Run_Func_WithSuccessfulFunctionOfNullableInt_ReturnsResult()
    {
        // Arrange
        var expectedValue = 42;
        int Function() => expectedValue;

        // Act
        var result = Try.Run((Func<int>?)Function);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Empty(result.ErrorMessage!);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public void Run_Func_WithStringFunction_ReturnsResult()
    {
        // Arrange
        const string expectedValue = "Hello, World!";
        static string Function() => expectedValue;

        // Act
        var result = Try.Run((Func<string>?)Function);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Empty(result.ErrorMessage!);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public void Run_Func_WithNullableStringFunction_ReturnsResult()
    {
        // Arrange
        const string expectedValue = "Hello, World!";
        static string Function() => expectedValue;

        // Act
        var result = Try.Run((Func<string>?)Function);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Empty(result.ErrorMessage!);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public void Run_Func_WithObjectFunction_ReturnsResult()
    {
        // Arrange
        var expectedValue = new object();
        object Function() => expectedValue;

        // Act
        var result = Try.Run((Func<object>?)Function);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Empty(result.ErrorMessage!);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public void FunctionNullableInt_WithExceptionThrowingFunction_ReturnsDefault()
    {
        // Arrange
        static int? Function() => throw new InvalidOperationException("Test exception");

        // Act
        var result = Try.Run((Func<int?>)Function);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.NotNull(result.ErrorMessage);
        Assert.Equal("Test exception", result.ErrorMessage);
        var ex = Assert.Throws<InvalidOperationException>(() => result.Value);
        Assert.Equal("Cannot access the Value of a failed Result.", ex.Message);
    }



    [Fact]
    public void Run_Func_reference_type_returns_Result_with_correct_properties()
    {
        // Arrange
        var expectedObject = new { Name = "Test", Value = 100 };
        object Function() => expectedObject;

        // Act
        var result = Try.Run(Function);

        // Assert
        Assert.Equal(expectedObject, result.Value);
    }



    [Fact]
    public void Run_Func_WithMultipleCalls_HandlesEachIndependently()
    {
        // Arrange
        var callCount = 0;
        var successFunction = () => ++callCount;
        var failFunction = new Func<int>
        (
            () =>
                {
                    callCount++;
                    throw new Exception("Test error");
                }
            );

        // Act
        var result1 = Try.Run(successFunction);
        var result2 = Try.Run(failFunction);
        var result3 = Try.Run(successFunction);

        // Assert
        Assert.True(result1.Succeeded);
        Assert.Equal(1, result1.Value);

        Assert.True(result2.Failed);
        Assert.Equal("Test error", result2.ErrorMessage); // Default int value

        Assert.True(result3.Succeeded);
        Assert.Equal(3, result3.Value);

        Assert.Equal(3, callCount);
    }
}
