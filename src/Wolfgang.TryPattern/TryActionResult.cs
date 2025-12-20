using System;

namespace Wolfgang.TryPattern;



/// <summary>
/// The result of a Try Action execution.
/// </summary>
public record TryActionResult
{
    /// <summary>
    /// Initializes a new instance of the TryActionResult class with the specified success status and
    /// exception information.
    /// </summary>
    /// <param name="succeeded">A value indicating whether the operation succeeded. Set to <see langword="true"/> if the operation was
    /// successful; otherwise, <see langword="false"/>.</param>
    /// <param name="exception">The exception that was thrown during the operation if it failed; otherwise, <see langword="null"/>.</param>
    public TryActionResult
    (
        bool succeeded,
        Exception? exception
    )
    {
        Succeeded = succeeded;
        Exception = exception;
    }


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
    public bool Succeeded { get; }



    /// <summary>
    /// Gets a value indicating whether the operation did not succeed.
    /// </summary>
    public bool Failed  => !Succeeded;



    /// <summary>
    /// If the action failed, gets the exception that caused the current operation to fail. Otherwise, this value is null.
    /// </summary>
    public Exception? Exception { get; }
}