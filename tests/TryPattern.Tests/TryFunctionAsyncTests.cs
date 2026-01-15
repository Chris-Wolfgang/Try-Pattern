#if !NET6_0_OR_GREATER
using System;
using System.Threading.Tasks;
#endif

namespace TryPattern.Tests;

public class TryFunctionAsyncTests
{
    [Fact]
    public async Task FunctionAsync_WithNullFunction_ThrowsArgumentNullException()
    {
        // Arrange
        Func<Task<int>>? nullFunction = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => Try.FunctionAsync(nullFunction!));
    }

    [Fact]
    public async Task FunctionAsync_WithSuccessfulFunction_ReturnsResult()
    {
        // Arrange
        const int expectedValue = 42;
        Func<Task<int>> function = async () =>
        {
            await Task.Delay(10);
            return expectedValue;
        };

        // Act
        var result = await Try.FunctionAsync(function);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public async Task FunctionAsync_WithStringFunction_ReturnsResult()
    {
        // Arrange
        const string expectedValue = "Hello, Async World!";
        Func<Task<string>> function = async () =>
        {
            await Task.Delay(10);
            return expectedValue;
        };

        // Act
        var result = await Try.FunctionAsync(function);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public async Task FunctionAsync_WithExceptionThrowingFunction_ReturnsDefault()
    {
        // Arrange
        Func<Task<int>> function = async () =>
        {
            await Task.Delay(10);
            throw new InvalidOperationException("Test exception");
        };

        // Act
        var result = await Try.FunctionAsync(function);

        // Assert
        Assert.Equal(default(int), result);
    }

    [Fact]
    public async Task FunctionAsync_WithExceptionThrowingStringFunction_ReturnsNull()
    {
        // Arrange
        Func<Task<string>> function = async () =>
        {
            await Task.Delay(10);
            throw new InvalidOperationException("Test exception");
        };

        // Act
        var result = await Try.FunctionAsync(function);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task FunctionAsync_WithSynchronousException_ReturnsDefault()
    {
        // Arrange
        Func<Task<int>> function = () => throw new InvalidOperationException("Synchronous exception");

        // Act
        var result = await Try.FunctionAsync(function);

        // Assert
        Assert.Equal(default(int), result);
    }

    [Fact]
    public async Task FunctionAsync_WithComplexObject_ReturnsResult()
    {
        // Arrange
        var expectedObject = new { Name = "Test", Value = 100 };
        Func<Task<object>> function = async () =>
        {
            await Task.Delay(10);
            return expectedObject;
        };

        // Act
        var result = await Try.FunctionAsync(function);

        // Assert
        Assert.Equal(expectedObject, result);
    }

    [Fact]
    public async Task FunctionAsync_WithMultipleCalls_HandlesEachIndependently()
    {
        // Arrange
        var callCount = 0;
        Func<Task<int>> successFunction = async () =>
        {
            await Task.Delay(10);
            return ++callCount;
        };
        Func<Task<int>> failFunction = async () =>
        {
            await Task.Delay(10);
            callCount++;
            throw new Exception();
        };

        // Act
        var result1 = await Try.FunctionAsync(successFunction);
        var result2 = await Try.FunctionAsync(failFunction);
        var result3 = await Try.FunctionAsync(successFunction);

        // Assert
        Assert.Equal(1, result1);
        Assert.Equal(0, result2); // Default int value
        Assert.Equal(3, result3);
        Assert.Equal(3, callCount);
    }
}
