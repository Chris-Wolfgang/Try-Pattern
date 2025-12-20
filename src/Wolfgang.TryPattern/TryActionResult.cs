using System;

namespace Wolfgang.TryPattern;



/// <summary>
/// The result of a Try Action execution.
/// </summary>
/// <param name="Succeeded">
/// Indicates whether the action succeeded or failed.
/// </param>
/// <param name="Exception">
/// The exception that caused the action to fail, if any. Otherwise, null.
/// </param>
public record TryActionResult
(
    bool Succeeded,
    Exception? Exception
)
{
    /// Used when action succeeded
    public TryActionResult()
    : this(true, null)
    {
    }



    /// Used when action failed
    public TryActionResult
    (
        Exception exception
    )
        : this(false, exception)
    {
    }



    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    public bool Succeeded { get; } = Succeeded;



    /// <summary>
    /// Gets a value indicating whether the operation did not succeed.
    /// </summary>
    public bool Failed  => !Succeeded;



    /// <summary>
    /// If the action failed, gets the exception that caused the current operation to fail. Otherwise, this value is null.
    /// </summary>
    public Exception? Exception { get; } = Exception;
}