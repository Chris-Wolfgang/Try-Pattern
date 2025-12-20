namespace Wolfgang.TryPattern.Tests;

public class TryFuncResultTests
{


    [Fact]
    public void Constructor_WithSucceededAndValue_SetsPropertiesCorrectly()
    {
        // Arrange
        const int expectedValue = 42;

        // Act
        var result = new TryFuncResult<int>(true, expectedValue, null);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Equal(expectedValue, result.Value);
        Assert.Null(result.Exception);
    }



    [Fact]
    public void Constructor_WithFailedAndException_SetsPropertiesCorrectly()
    {
        // Arrange
        var exception = new Exception("Test exception");

        // Act
        var result = new TryFuncResult<int>(false, default, exception);

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.Equal(0, result.Value);
        Assert.Equal(exception, result.Exception);
    }



    [Fact]
    public void Constructor_WithSucceededOnly_SetsPropertiesCorrectly()
    {
        // Arrange
        const int expectedValue = 100;
        // Act
        var result = new TryFuncResult<int>(expectedValue);
        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Equal(expectedValue, result.Value);
        Assert.Null(result.Exception);
    }



    [Fact]
    public void Constructor_for_int_WithFailedOnly_SetsPropertiesCorrectly()
    {
        // Arrange
        var exception = new Exception("Test exception");
        // Act
        var result = new TryFuncResult<int>(exception);
        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.Equal(0, result.Value);
        Assert.Equal(exception, result.Exception);
    }



    [Fact]
    public void Constructor_for_string_WithFailedOnly_SetsPropertiesCorrectly()
    {
        // Arrange
        var exception = new Exception("Test exception");
        // Act
        var result = new TryFuncResult<DateTime>(exception);
        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.Equal(new DateTime(), result.Value);
        Assert.Equal(exception, result.Exception);
    }



    [Fact]
    public void Constructor_when_passed_object_sets_properties_correctly()
    {
        // Arrange

        var value = new object();

        // Act
        var result = new TryFuncResult<object?>(value);
        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Equal(value, result.Value);
        Assert.Null(result.Exception);
    }



    [Fact]
    public void Constructor_when_passed_int_sets_properties_correctly()
    {
        // Arrange

        var value = 3;

        // Act
        var result = new TryFuncResult<int>(value);
        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Equal(value, result.Value);
        Assert.Null(result.Exception);
    }



    [Theory]
    [InlineData(null)]
    [InlineData(3)]
    public void Constructor_when_passed_Nullable_int_sets_properties_correctly(int? value)
    {
        // Arrange

        // Act
        var result = new TryFuncResult<int?>(value);
        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Equal(value, result.Value);
        Assert.Null(result.Exception);
    }



    [Fact]
    public void Constructor_when_passed_string_sets_properties_correctly()
    {
        // Arrange

        var value = "Hello World";

        // Act
        var result = new TryFuncResult<string>(value);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Equal(value, result.Value);
        Assert.Null(result.Exception);
    }



    [Theory]
    [InlineData(null)]
    [InlineData("Hello World")]
    public void Constructor_when_passed_Nullable_int_sets_properties_correctly(string? value)
    {
        // Arrange

        // Act
        var result = new TryFuncResult<string?>(value);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Equal(value, result.Value);
        Assert.Null(result.Exception);
    }




}
