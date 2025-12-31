using System;
using System.Linq;

namespace Wolfgang.TryPattern;



/// <summary>
/// The result of a Try Action execution.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the Result class with the specified success status and
    /// exception information.
    /// </summary>
    /// <param name="succeeded">A value indicating whether the operation succeeded. Set to <see langword="true"/> if the operation was
    /// successful; otherwise, <see langword="false"/>.</param>
    /// <param name="errorMessage">Error errorMessage associated with the result.</param>
    protected Result
    (
        bool succeeded,
        string? errorMessage
    )
    {
        switch (succeeded)
        {
            case true when errorMessage != string.Empty:
                throw new ArgumentException("A successful result cannot have an error errorMessage.", nameof(errorMessage));

            case false when string.IsNullOrWhiteSpace(errorMessage):
                throw new ArgumentException("A failed result must have an error errorMessage.", nameof(errorMessage));

            default:
                Succeeded = succeeded;
                ErrorMessage = errorMessage;
                break;
        }
    }



    /// <summary>
    /// Creates a successful Result.
    /// </summary>
    public static Result Success() => new(true, string.Empty);



    /// <summary>
    /// Creates a failed Result with the specified error message.
    /// </summary>
    /// <param name="message">The error message or empty string.</param>
    /// <exception cref="ArgumentException">ErrorMessage is null</exception>
    public static Result Failure(string message) =>  string.IsNullOrWhiteSpace(message)
            ? throw new ArgumentException("ErrorMessage cannot be empty", nameof(message))
            : new Result(false, message);



    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    /// <remarks>Succeeded and Failed are mutually exclusive</remarks>
    public bool Succeeded { get; }



    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    /// <remarks>Succeeded and Failed are mutually exclusive</remarks>
    public bool Failed => !Succeeded;



    /// <summary>
    /// The error message associated with the result if the operation failed. Otherwise, empty string.
    /// </summary>
    public string? ErrorMessage { get; }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="results"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">results is null</exception>
    public static Result Flatten(params Result[] results)
    {
        if (results == null)
        {
            throw new ArgumentNullException(nameof(results));
        }

        var failures = results
            .Where(r => r.Failed)
            .Select(r => r.ErrorMessage);

        var message = string.Join("\n", failures);

        return message.Length == 0
            ? Success()
            : Failure(message);
    }



    /// <summary>
    /// Returns true if any of the provided Result instances indicate a failure.
    /// </summary>
    /// <param name="results">The array of <see cref="Result"/>> to review</param>
    /// <returns>true if any of the results Failed. Otherwise, false</returns>
    /// <exception cref="ArgumentNullException">results is null</exception>
    public static bool AnyFailed(params Result[]? results) =>
        results?.Any(r => r.Failed) ?? throw new ArgumentNullException(nameof(results));



    /// <summary>
    /// Returns true if all of provided Result instances indicate success.
    /// </summary>
    /// <param name="results">The array of <see cref="Result"/>> to review</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">results is null</exception>
    public static bool AllSucceeded(params Result[]? results) =>
        results?.All(r => r.Succeeded) ?? throw new ArgumentNullException(nameof(results));
}



/// <summary>
/// The result of a Try Action execution that returns a value of type T.
/// </summary>
/// <typeparam name="T"></typeparam>


public class Result<T> : Result
{
#if NET5_0_OR_GREATER
    private readonly T? _value;
#else
    private readonly T _value;
#endif



/// <summary>
/// Creates a new instance of Result&lt;T&gt;.
/// </summary>
/// <param name="succeeded">A value indicating if the operation succeeded.</param>
/// <param name="errorMessage">The error errorMessage if the operation failed.</param>
/// <param name="value">The return value of the operation if it succeeded</param>
#if NET5_0_OR_GREATER
    private Result(bool succeeded, string? errorMessage, T value) : base(succeeded, errorMessage) => _value = value;
#else
    private Result(bool succeeded, string? errorMessage, T value) : base(succeeded, errorMessage) => _value = value;
#endif



    /// <summary>
    /// The value produced by the operation if it succeeded.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
#if NET5_0_OR_GREATER
    public T? Value => Failed
        ? throw new InvalidOperationException("Cannot access the Value of a failed Result.")
        : _value;
#else
    public T Value => Failed
        ? throw new InvalidOperationException("Cannot access the Value of a failed Result.")
        : _value;
#endif



    /// <summary>
    /// Creates a successful Result.
    /// </summary>
#if NET5_0_OR_GREATER
    public static Result<T?> Success(T? value) => new Result<T?>(true, string.Empty, value);
#else
    public static Result<T> Success(T value) => new(true, string.Empty, value);
#endif



    /// <summary>
    /// Creates a failed Result with the specified error message.
    /// </summary>
    /// <param name="message">The error message or empty string.</param>
    /// <exception cref="ArgumentException">ErrorMessage is null</exception>
#if NET5_0_OR_GREATER
    public static new Result<T?> Failure(string message) => message == null
        ? throw new ArgumentException("ErrorMessage cannot be null", nameof(message))
        : new Result<T?>(false, message, default(T)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              );
#else
    public static new Result<T> Failure(string message) => message == null
        ? throw new ArgumentException("ErrorMessage cannot be null", nameof(message))
        : new Result<T>(false, message, default(T));
#endif

}