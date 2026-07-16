using System;

namespace Wolfgang.TryPattern.Tests.Unit;

public class ResultTests
{
    private class TestResult(bool succeeded, string? errorMessage) : Result(succeeded, errorMessage);



    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Ctor_when_passed_true_and_null_or_empty_does_not_throw_Exception(string? message)
    {
        // Both null and "" are valid inputs on success — the ctor
        // canonicalizes them to null-valued ErrorMessage.
        var result = new TestResult(succeeded: true, message);

        Assert.True(result.Succeeded);
        Assert.Null(result.ErrorMessage);
    }



    [Theory]
    [InlineData(" ")]
    [InlineData("Test error")]
    public void Ctor_when_passed_true_and_non_empty_string_throws_ArgumentException(string message)
    {
        var ex = Assert.Throws<ArgumentException>(() => new TestResult(succeeded: true, message));
        Assert.Equal("errorMessage", ex.ParamName);
    }



    [Fact]
    public void Ctor_when_passed_false_and_message_does_not_throw_Exception()
    {
        var unused = new TestResult(succeeded: false, "Test error");
    }



    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("")]
    public void Ctor_when_passed_false_and_no_message_throws_ArgumentException(string? message)
    {
        var ex = Assert.Throws<ArgumentException>(() => new TestResult(succeeded: false, message));
        Assert.Equal("errorMessage", ex.ParamName);
    }





    [Fact]
    public void Success_sets_properties_correctly()
    {
        // Act
        var result = Result.Success();
        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.ErrorMessage);
    }



    [Fact]
    public void Success_returns_the_same_cached_instance_on_every_call()
    {
        // Locks in the singleton optimization: Result is immutable, so
        // Success() intentionally returns a shared instance to avoid
        // per-call allocations. If this assertion ever needs to be
        // relaxed, it is a deliberate behavioral change that consumers
        // relying on reference identity should be notified about.
        var first = Result.Success();
        var second = Result.Success();

        Assert.Same(first, second);
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
    public void Flatten_when_array_contains_null_element_throws_ArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            Result.Flatten(Result.Success(), null!, Result.Failure("x")));
        Assert.Equal("results", ex.ParamName);
        Assert.Contains("index 1", ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void AllSucceeded_when_array_contains_null_element_throws_ArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            Result.AllSucceeded(Result.Success(), null!));
        Assert.Equal("results", ex.ParamName);
        Assert.Contains("index 1", ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void AnyFailed_when_array_contains_null_element_throws_ArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            Result.AnyFailed(null!, Result.Success()));
        Assert.Equal("results", ex.ParamName);
        Assert.Contains("index 0", ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void AnyFailed_does_not_inspect_elements_after_first_failure()
    {
        // Documents the short-circuit behavior: AnyFailed walks the array
        // until it can decide the answer. Elements after the first failure
        // (including nulls) are intentionally not inspected.
        var result = Result.AnyFailed(Result.Failure("decisive"), null!);

        Assert.True(result);
    }



    [Fact]
    public void AllSucceeded_does_not_inspect_elements_after_first_failure()
    {
        // Documents the short-circuit behavior: AllSucceeded walks the array
        // until it can decide the answer. Elements after the first failure
        // (including nulls) are intentionally not inspected.
        var result = Result.AllSucceeded(Result.Failure("decisive"), null!);

        Assert.False(result);
    }



    [Fact]
    public void Flatten_when_passed_nothing_returns_new_successful_Result()
    {
        var result = Result.Flatten();

        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.ErrorMessage);
    }



    [Fact]
    public void Flatten_when_passed_empty_list_returns_new_successful_Result()
    {
        var result = Result.Flatten(Array.Empty<Result>());

        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.ErrorMessage);
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
        Assert.Null(result.ErrorMessage);
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



    [Fact]
    public void Flatten_when_multiple_results_failed_returns_Result_with_all_messages()
    {
        var result1 = Result.Success();
        var result2 = Result.Failure("test error 1");
        var result3 = Result.Failure("test error 2");
        var result4 = Result.Success();

        var result = Result.Flatten(result1, result2, result3, result4);

        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.Equal("test error 1\ntest error 2", result.ErrorMessage);
    }






}
