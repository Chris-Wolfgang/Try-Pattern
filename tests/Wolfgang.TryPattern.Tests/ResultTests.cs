namespace Wolfgang.TryPattern.Tests;

public class ResultTests
{
    private class TestResult(bool succeeded, string? errorMessage) : Result(succeeded, errorMessage);



    [Fact]
    public void Ctor_When_Passed_True_And_Empty_String_Does_Not_Throw_Exception()
    {
        var sut = new TestResult(true, string.Empty);
    }



    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("Test error")]
    public void Ctor_When_Passed_True_And_Non_Empty_String_Throw_InvalidOperationException(string? message)
    {
        var ex = Assert.Throws<ArgumentException>(() => new TestResult(true, message));
        Assert.Equal("errorMessage", ex.ParamName);
    }



    [Fact]
    public void Ctor_When_Passed_False_And_Message_Does_Not_Throw_Exception()
    {
        var sut = new TestResult(false, "Test error");
    }



    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("")]
    public void Ctor_When_Passed_False_And_No_Message_Throw_InvalidOperationException(string? message)
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
    public void Failure_When_Passed_Invalid_Message_Throws_ArgumentException(string? message)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => Result.Failure(message!));
        Assert.Equal("errorMessage", ex.ParamName);

    }



    [Fact]
    public void AllSucceeded_When_Passed_Null_Throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Result.AllSucceeded(null!));
        Assert.Equal("results", ex.ParamName);
    }



    [Fact]
    public void AllSucceeded_When_Passed_Nothing_Returns_True()
    {
        Assert.True(Result.AllSucceeded());
    }



    [Fact]
    public void AllSucceeded_When_Passed_Empty_List_Returns_True()
    {
        Assert.True(Result.AllSucceeded(Array.Empty<Result>()));
    }



    [Fact]
    public void AllSucceeded_When_All_Results_Succeeded_Returns_True()
    {
        var result1 = Result.Success();
        var result2 = Result.Success();
        var result3 = Result.Success();

        Assert.True(Result.AllSucceeded(result1, result2, result3));
    }



    [Fact]
    public void AllSucceeded_When_At_Least_One_Result_Failed_Returns_False()
    {
        var result1 = Result.Success();
        var result2 = Result.Failure("Test");
        var result3 = Result.Success();

        Assert.False(Result.AllSucceeded(result1, result2, result3));

    }



    [Fact]
    public void AnyFailed_When_Passed_Null_Throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Result.AnyFailed(null!));
        Assert.Equal("results", ex.ParamName);
    }



    [Fact]
    public void AnyFailed_When_Nothing_Returns_False()
    {
        Assert.False(Result.AnyFailed());
    }



    [Fact]
    public void AnyFailed_When_Passed_Empty_List_Returns_False()
    {
        Assert.False(Result.AnyFailed(Array.Empty<Result>()));
    }



    [Fact]
    public void AnyFailed_When_All_Results_Succeeded_Returns_False()
    {
        var result1 = Result.Success();
        var result2 = Result.Success();
        var result3 = Result.Success();

        Assert.False(Result.AnyFailed(result1, result2, result3));
    }



    [Fact]
    public void AnyFailed_When_At_Least_One_Result_Failed_Returns_True()
    {
        var result1 = Result.Success();
        var result2 = Result.Failure("Test error");
        var result3 = Result.Success();

        Assert.True(Result.AnyFailed(result1, result2, result3));
    }



    [Fact]
    public void Flatten_When_Passed_Null_Throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Result.Flatten(null!));
        Assert.Equal("results", ex.ParamName);
    }



    [Fact]
    public void Flatten_When_Passed_Nothing_Returns_New_Successful_Result()
    {
        var result = Result.Flatten();

        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.NotNull(result.ErrorMessage);
        Assert.Empty(result.ErrorMessage);
    }



    [Fact]
    public void Flatten_When_Passed_Empty_List_Returns_New_Successful_Result()
    {
        var result = Result.Flatten(Array.Empty<Result>());

        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.NotNull(result.ErrorMessage);
        Assert.Empty(result.ErrorMessage);
    }



    [Fact]
    public void Flatten_When_All_Results_Succeeded_Returns_New_Successful_Result()
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
    public void Flatten_When_At_Least_One_Result_Failed_Returns_First_Failed_Result()
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
