using System;

namespace Wolfgang.TryPattern.Tests.Unit;

public class RunFuncTests
{
    [Fact]
    public void Run_Func_when_function_is_null_throws_ArgumentNullException()
    {
        // Arrange
        Func<int>? nullFunction = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Try.Run(nullFunction!));
    }



    [Fact]
    public void Run_Func_when_int_function_succeeds_returns_successful_Result()
    {
        // Arrange
        const int expectedValue = 42;
        static int Function() => expectedValue;

        // Act
        var result = Try.Run(Function);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.ErrorMessage);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public void Run_Func_when_nullable_int_function_succeeds_returns_successful_Result()
    {
        // Arrange
        var expectedValue = 42;
        int Function() => expectedValue;

        // Act
        var result = Try.Run(Function);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.ErrorMessage);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public void Run_Func_when_string_function_succeeds_returns_successful_Result()
    {
        // Arrange
        const string expectedValue = "Hello, World!";
        static string Function() => expectedValue;

        // Act
        var result = Try.Run(Function);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.ErrorMessage);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public void Run_Func_when_nullable_string_function_succeeds_returns_successful_Result()
    {
        // Arrange
        const string expectedValue = "Hello, World!";
        static string? Function() => expectedValue;

        // Act
        var result = Try.Run(Function);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.ErrorMessage);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public void Run_Func_when_object_function_succeeds_returns_successful_Result()
    {
        // Arrange
        var expectedValue = new object();
        object Function() => expectedValue;

        // Act
        var result = Try.Run(Function);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.ErrorMessage);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public void Run_Func_when_nullable_int_function_throws_returns_failed_Result_whose_Value_access_throws()
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
    public void Run_Func_when_called_multiple_times_handles_each_independently()
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
