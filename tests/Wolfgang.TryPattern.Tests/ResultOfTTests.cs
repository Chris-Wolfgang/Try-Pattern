
// ReSharper disable AccessToStaticMemberViaDerivedType

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
        Assert.Equal("John", result.Value.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
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


    
    [Fact]
    public void AllSucceeded_when_passed_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Result<int>.AllSucceeded(null!));
        Assert.Equal("results", ex.ParamName);
    }



    [Fact]
    public void AllSucceeded_when_passed_nothing_returns_true()
    {
        Assert.True(Result<int>.AllSucceeded());
    }



    [Fact]
    public void AllSucceeded_when_passed_empty_list_returns_true()
    {
        Assert.True(Result<int>.AllSucceeded(Array.Empty<Result>()));
    }



    [Fact]
    public void AllSucceeded_when_all_results_succeeded_returns_true()
    {
        var result1 = Result<int>.Success();
        var result2 = Result<int>.Success();
        var result3 = Result<int>.Success();

        Assert.True(Result<int>.AllSucceeded(result1, result2, result3));
    }



    [Fact]
    public void AllSucceeded_when_at_least_one_result_failed_returns_false()
    {
        var result1 = Result<int>.Success();
        var result2 = Result<int>.Failure("Test");
        var result3 = Result<int>.Success();

        Assert.False(Result<int>.AllSucceeded(result1, result2, result3));

    }



    [Fact]
    public void AnyFailed_when_passed_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Result<int>.AnyFailed(null!));
        Assert.Equal("results", ex.ParamName);
    }



    [Fact]
    public void AnyFailed_when_nothing_returns_false()
    {
        Assert.False(Result<int>.AnyFailed());
    }



    [Fact]
    public void AnyFailed_when_passed_empty_list_returns_false()
    {
        Assert.False(Result<int>.AnyFailed(Array.Empty<Result>()));
    }



    [Fact]
    public void AnyFailed_when_all_results_succeeded_returns_false()
    {
        var result1 = Result<int>.Success();
        var result2 = Result<int>.Success();
        var result3 = Result<int>.Success();

        Assert.False(Result<int>.AnyFailed(result1, result2, result3));
    }



    [Fact]
    public void AnyFailed_when_at_least_one_result_failed_returns_true()
    {
        var result1 = Result<int>.Success();
        var result2 = Result<int>.Failure("Test error");
        var result3 = Result<int>.Success();

        Assert.True(Result<int>.AnyFailed(result1, result2, result3));
    }



    [Fact]
    public void Flatten_when_passed_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Result<int>.Flatten(null!));
        Assert.Equal("results", ex.ParamName);
    }



    [Fact]
    public void Flatten_when_passed_nothing_returns_new_successful_Result()
    {
        var result = Result<int>.Flatten();

        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.NotNull(result.ErrorMessage);
        Assert.Empty(result.ErrorMessage);
    }



    [Fact]
    public void Flatten_when_passed_empty_list_returns_new_successful_Result()
    {
        var result = Result<int>.Flatten(Array.Empty<Result>());

        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.NotNull(result.ErrorMessage);
        Assert.Empty(result.ErrorMessage);
    }



    [Fact]
    public void Flatten_when_all_results_succeeded_returns_new_successful_Result()
    {
        var result1 = Result<int>.Success();
        var result2 = Result<int>.Success();
        var result3 = Result<int>.Success();

        var result = Result<int>.Flatten(result1, result2, result3);

        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.NotNull(result.ErrorMessage);
        Assert.Empty(result.ErrorMessage);
    }



    [Fact]
    public void Flatten_when_at_least_one_result_failed_returns_first_failed_Result()
    {
        var result1 = Result<int>.Success();
        var result2 = Result<int>.Failure("test error");
        var result3 = Result<int>.Success();

        var result = Result<int>.Flatten(result1, result2, result3);

        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.Equal("test error", result.ErrorMessage);
    }




    [Fact]
    public void Flatten_when_multiple_results_failed_returns_Result_with_all_message()
    {
        var result1 = Result<int>.Success();
        var result2 = Result<int>.Failure("test error 1");
        var result3 = Result<int>.Failure("test error 2");
        var result4 = Result<int>.Success();

        var result = Result<int>.Flatten(result1, result2, result3, result4);

        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.Equal("test error 1\ntest error 2", result.ErrorMessage);
    }

}