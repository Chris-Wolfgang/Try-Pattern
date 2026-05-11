namespace Wolfgang.TryPattern.Tests;

public class ResultOfTTests
{


    [Fact]
    public void Success_int_sets_properties_correctly()
    {
        // Arrange
        const int expectedValue = 42;

        // Act
        var result = Result<int>.Success(expectedValue);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Equal(expectedValue, result.Value);
        Assert.Empty(result.ErrorMessage!);
    }



    [Fact]
    public void Failure_int_sets_properties_correctly()
    {
        // Arrange
        const string message = "Test exception";

        // Act
        var result = Result<int>.Failure(message);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.Equal(message, result.ErrorMessage);
        var ex = Assert.Throws<InvalidOperationException>(() => result.Value);
        Assert.Equal("Cannot access the Value of a failed Result.", ex.Message);
    }



    [Fact]
    public void Success_DateTime_sets_properties_correctly()
    {
        // Arrange
        var expectedValue = DateTime.Now;

        // Act
        var result = Result<DateTime>.Success(expectedValue);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Equal(expectedValue, result.Value);
        Assert.Empty(result.ErrorMessage!);
    }



    [Fact]
    public void Failure_DateTime_sets_properties_correctly()
    {
        // Arrange
        const string message = "Test exception";

        // Act
        var result = Result<DateTime>.Failure(message);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.Equal(message, result.ErrorMessage);
        var ex = Assert.Throws<InvalidOperationException>(() => result.Value);
        Assert.Equal("Cannot access the Value of a failed Result.", ex.Message);
    }



    [Fact]
    public void Success_Person_sets_properties_correctly()
    {
        // Arrange
        var value = new Person {FirstName = "John", LastName = "Doe"};

        // Act
        var result = Result<Person>.Success(value);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Equal(value, result.Value);
        Assert.Equal("John", result.Value!.FirstName);
        Assert.Equal("Doe", result.Value!.LastName);
        Assert.Empty(result.ErrorMessage!);
    }



    [Fact]
    public void Success_Person_when_passed_null_sets_properties_correctly()
    {
        // Arrange

        // Act
        var result = Result<Person>.Success(null!);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null( result.Value);
        Assert.Empty(result.ErrorMessage!);
    }



    [Theory]
    [InlineData(null)]
    [InlineData(3)]
    public void Success_Nullable_int_sets_properties_correctly(int? value)
    {
        // Arrange

        // Act
        var result = Result<int?>.Success(value);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Equal(value, result.Value);
        Assert.Empty(result.ErrorMessage!);
    }



    [Fact]
    public void Success_string_sets_properties_correctly()
    {
        // Arrange

        var value = "Hello World";

        // Act
        var result = Result<string>.Success(value);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Equal(value, result.Value);
        Assert.Empty(result.ErrorMessage!);
    }



    [Theory]
    [InlineData(null)]
    [InlineData("Hello World")]
    public void Success_Nullable_string_sets_properties_correctly(string? value)
    {
        // Arrange

        // Act
        var result = Result<string?>.Success(value);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Equal(value, result.Value);
        Assert.Empty(result.ErrorMessage!);
    }


    // Note: AllSucceeded / AnyFailed / Flatten are inherited from Result and
    // are already covered by ResultTests. Tests that invoked them via the
    // derived type `Result<int>.` were just verifying inheritance, which is a
    // language feature — they have been removed.
}