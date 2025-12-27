namespace Wolfgang.TryPattern.Tests;

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
    public async Task FunctionAsyncInt_WithSuccessfulFunction_ReturnsResult()
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
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.Exception);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public async Task FunctionAsyncNullableInt_WithSuccessfulFunction_ReturnsResult()
    {
        // Arrange
        int? expectedValue = 42;
        Func<Task<int?>> function = async () =>
        {
            await Task.Delay(10);
            return expectedValue;
        };

        // Act
        var result = await Try.FunctionAsync(function);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.Exception);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public async Task FunctionAsyncString_WithStringFunction_ReturnsResult()
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
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.Exception);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public async Task FunctionAsyncStringNullable_WithStringFunction_ReturnsResult()
    {
        // Arrange
        var expectedValue = "Hello, Async World!";
        Func<Task<string?>> function = async () =>
        {
            await Task.Delay(10);
            return expectedValue;
        };

        // Act
        var result = await Try.FunctionAsync(function);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.Exception);
        Assert.Equal(expectedValue, result.Value);
    }



    [Fact]
    public async Task FunctionAsyncInt_WithExceptionThrowingFunction_ReturnsDefault()
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
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.NotNull(result.Exception);
        Assert.Equal(0, result.Value);
    }



    [Fact]
    public async Task FunctionAsyncIntNullable_WithExceptionThrowingFunction_ReturnsDefault()
    {
        // Arrange
        Func<Task<int?>> function = async () =>
        {
            await Task.Delay(10);
            throw new InvalidOperationException("Test exception");
        };

        // Act
        var result = await Try.FunctionAsync(function);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.NotNull(result.Exception);
        Assert.Null(result.Value);
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
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.NotNull(result.Exception);
        Assert.Null(result.Value);
    }



    [Fact]
    public async Task FunctionAsyncInt_WithSynchronousException_ReturnsDefault()
    {
        // Arrange
        Func<Task<int>> function = () => throw new InvalidOperationException("Synchronous exception");

        // Act
        var result = await Try.FunctionAsync(function);

        // Assert
        Assert.Equal(0, result.Value);
    }



    [Fact]
    public async Task FunctionAsyncIntNullable_WithSynchronousException_ReturnsDefault()
    {
        // Arrange
        Func<Task<int?>> function = () => throw new InvalidOperationException("Synchronous exception");

        // Act
        var result = await Try.FunctionAsync(function);

        // Assert
        Assert.Null(result.Value);
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
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.Exception);
        Assert.Equal(expectedObject, result.Value);
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
        Assert.Equal(1, result1.Value);
        Assert.Equal(0, result2.Value); // Default int value
        Assert.Equal(3, result3.Value);
        Assert.Equal(3, callCount);
    }
}
