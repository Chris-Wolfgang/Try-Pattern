using System;

namespace Wolfgang.TryPattern;

/// <summary>
/// The result of a Try Function execution.
/// </summary>
/// <typeparam name="T">
/// The type of the result produced by the function.
/// </typeparam>
public record TryFuncResult<T>
{

	/// <summary>
	/// Initializes a new instance of the TryFuncResult class with the specified success status, value  and
	/// exception information.
	/// </summary>
	/// <param name="succeeded">A value indicating whether the operation succeeded. Set to <see langword="true"/> if the operation was
	/// successful; otherwise, <see langword="false"/>.</param>
	/// <param name="value">The value produced by the operation if it succeeded; otherwise, the default value for the type.</param>
	/// <param name="exception">The exception that was thrown during the operation if it failed; otherwise, <see langword="null"/>.</param>
	public TryFuncResult
	(
		bool succeeded,
#if NET5_0_OR_GREATER
		T? value,
#else
		T value,
#endif
		Exception? exception
	)
	{
		Succeeded = succeeded;
		Value = value;
		Exception = exception;
	}



	/// <summary>
	/// Initializes a new instance of the TryFuncResult class for a successful operation.
	/// </summary>
	/// <param name="value">The value produced by the successful operation.</param>
	public TryFuncResult
	(
#if NET5_0_OR_GREATER
		T? value
#else
		T value
#endif
	)
		: this(true, value, null)
	{
	}



	/// <summary>
	/// Initializes a new instance of the TryFuncResult class for a failed operation.
	/// </summary>
	/// <param name="exception">The exception that caused the operation to fail.</param>
	public TryFuncResult
	(
		Exception? exception
	)
		: this(false, default, exception)
	{

	}



    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    public bool Succeeded { get; }



    /// <summary>
    /// Gets a value indicating whether the operation did not complete successfully.
    /// </summary>
    public bool Failed => !Succeeded;



    /// <summary>
    /// If the function failed, gets the exception that caused the current operation to fail. Otherwise, this value is null.
    /// </summary>
    public Exception? Exception { get; }



    /// <summary>
    /// Gets the result value, if available.
    /// </summary>
    public T? Value { get; }
}