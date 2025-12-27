namespace Wolfgang.TryPattern.Tests;

public class TryActionResultTests
{


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Constructor_WithSucceededOnly_SetsPropertiesCorrectly(bool succeeded)
    {
        // Act
        var result = new TryActionResult(succeeded, null);
        // Assert
        Assert.Equal(succeeded, result.Succeeded);
        Assert.Null(result.Exception);
    }




    [Fact]

    public void Constructor_WithSucceededAndException_SetsPropertiesCorrectly()
    {
        // Arrange
        var exception = new Exception("Test exception");
        // Act
        var result = new TryActionResult(true, exception);
        // Assert
        Assert.Equal(exception, result.Exception);
    }



    [Fact]
    public void Constructor_WithFailedOnly_SetsPropertiesCorrectly()
    {
        // Act
        var result = new TryActionResult(false, null);
        // Assert
        Assert.False(result.Succeeded);
        Assert.Null(result.Exception);
    }



    [Fact]
    public void Constructor_with_no_parameters_sets_properties_correctly()
    {
        // Arrange

        // Act
        var result = new TryActionResult();

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Null(result.Exception);
    }



    [Fact]
    public void Constructor_with_Exception_sets_properties_correctly()
    {
        // Arrange

        // Act
        var result = new TryActionResult(new Exception());

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.NotNull(result.Exception);
    }

}
