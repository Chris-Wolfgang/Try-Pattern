#pragma warning disable IDE0022
namespace Wolfgang.TryPattern.Tests;

public class ResultTests
{
    private class TestResult(bool succeeded, string? errorMessage) : Result(succeeded, errorMessage);



    [Fact]
    public void Ctor_when_passed_true_and_empty_string_does_not_throw_Exception()
    {
        var unused = new TestResult(true, string.Empty);
    }



    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("Test error")]
    public void Ctor_when_passed_true_and_non_empty_string_throw_InvalidOperationException(string? message)
    {
        var ex = Assert.Throws<ArgumentException>(() => new TestResult(true, message));
        Assert.Equal("errorMessage", ex.ParamName);
    }



    [Fact]
    public void Ctor_when_passed_false_and_message_does_not_throw_Exception()
    {
        var unused = new TestResult(false, "Test error");
    }



    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("")]
    public void Ctor_when_passed_false_and_no_message_throw_InvalidOperationException(string? message)
    {
        var ex = Assert.Throws<ArgumentException>(() => new TestResult(false, message));
        Assert.Equal("errorMessage", ex.ParamName);
    }





    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Success_sets_properties_correctly(bool succeeded)
    {
        // Act
        var result = Result.Success();
        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Empty(result.ErrorMessage!);
    }



    [Fact]
    public void Failure_sets_properties_correctly()
    {
        const string message = "Test Error";

        // Act
        var result = Result.Failure(message);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.Equal(message, result.ErrorMessage!);
    }



    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Failure_when_passed_invalid_message_throws_ArgumentException(string? message)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => Result.Failure(message!));
        Assert.Equal("errorMessage", ex.ParamName);

    }



    [Fact]
    public void AllSucceeded_when_passed_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Result.AllSucceeded(null!));
        Assert.Equal("results", ex.ParamName);
    }



    [Fact]
    public void AllSucceeded_when_passed_nothing_returns_true()
    {
        Assert.True(Result.AllSucceeded());
    }



    [Fact]
    public void AllSucceeded_when_passed_empty_list_returns_true()
    {
        Assert.True(Result.AllSucceeded(Array.Empty<Result>()));
    }



    [Fact]
    public void AllSucceeded_when_all_results_succeeded_returns_true()
    {
        var result1 = Result.Success();
        var result2 = Result.Success();
        var result3 = Result.Success();

        Assert.True(Result.AllSucceeded(result1, result2, result3));
    }



    [Fact]
    public void AllSucceeded_when_at_least_one_result_failed_returns_false()
    {
        var result1 = Result.Success();
        var result2 = Result.Failure("Test");
        var result3 = Result.Success();

        Assert.False(Result.AllSucceeded(result1, result2, result3));

    }



    [Fact]
    public void AnyFailed_when_passed_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Result.AnyFailed(null!));
        Assert.Equal("results", ex.ParamName);
    }



    [Fact]
    public void AnyFailed_when_nothing_returns_false()
    {
        Assert.False(Result.AnyFailed());
    }



    [Fact]
    public void AnyFailed_when_passed_empty_list_returns_false()
    {
        Assert.False(Result.AnyFailed(Array.Empty<Result>()));
    }



    [Fact]
    public void AnyFailed_when_all_results_succeeded_returns_false()
    {
        var result1 = Result.Success();
        var result2 = Result.Success();
        var result3 = Result.Success();

        Assert.False(Result.AnyFailed(result1, result2, result3));
    }



    [Fact]
    public void AnyFailed_when_at_least_one_result_failed_returns_true()
    {
        var result1 = Result.Success();
        var result2 = Result.Failure("Test error");
        var result3 = Result.Success();

        Assert.True(Result.AnyFailed(result1, result2, result3));
    }



    [Fact]
    public void Flatten_when_passed_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Result.Flatten(null!));
        Assert.Equal("results", ex.ParamName);
    }



    [Fact]
    public void Flatten_when_passed_nothing_returns_new_successful_Result()
    {
        var result = Result.Flatten();

        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.NotNull(result.ErrorMessage);
        Assert.Empty(result.ErrorMessage);
    }



    [Fact]
    public void Flatten_when_passed_empty_list_returns_new_successful_Result()
    {
        var result = Result.Flatten(Array.Empty<Result>());

        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.NotNull(result.ErrorMessage);
        Assert.Empty(result.ErrorMessage);
    }



    [Fact]
    public void Flatten_when_all_results_succeeded_returns_new_successful_Result()
    {
        var result1 = Result.Success();
        var result2 = Result.Success();
        var result3 = Result.Success();

        var result = Result.Flatten(result1, result2, result3);

        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.NotNull(result.ErrorMessage);
        Assert.Empty(result.ErrorMessage);
    }



    [Fact]
    public void Flatten_when_at_least_one_result_failed_returns_first_failed_Result()
    {
        var result1 = Result.Success();
        var result2 = Result.Failure("test error");
        var result3 = Result.Success();

        var result = Result.Flatten(result1, result2, result3);

        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.Equal("test error", result.ErrorMessage);
    }






}
