namespace Wolfgang.TryPattern.AotSmoke;

/// <summary>A user-defined value type, so the generic members are exercised with a non-BCL struct.</summary>
internal readonly record struct Point(int X, int Y);

/// <summary>Reaches the protected <see cref="Result"/> constructor the way a consumer would.</summary>
internal sealed class CustomResult : Result
{
    public CustomResult(bool succeeded, string? errorMessage)
        : base(succeeded, errorMessage)
    {
    }
}
