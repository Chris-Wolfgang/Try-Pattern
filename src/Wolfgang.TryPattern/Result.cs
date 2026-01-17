using System;
using System.Linq;

namespace Wolfgang.TryPattern;



/// <summary>
/// The result of executing an <seealso cref="Action"/>. Contains properties indicating whether the operation
/// succeeded or failed. If the operation failed the <see cref="Result.ErrorMessage"/> property will contain
/// a message as to why.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the Result class with the specified success status and
    /// exception information.
    /// </summary>
    /// <param name="succeeded">A value indicating whether the operation succeeded. Set to <see langword="true"/>
    /// if the operation was successful; otherwise, <see langword="false"/>.</param>
    /// <param name="errorMessage">Error errorMessage associated with the result.</param>
    /// <remarks>
    /// If the operation was successful, errorMessage must be an empty string. If the operation failed
    /// errorMessage must not be null or empty
    /// </remarks>
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
    /// Creates a failed Result with the specified error Message.
    /// </summary>
    /// <param name="errorMessage">The error message indicating the reason for failure.</param>
    /// <exception cref="ArgumentException">errorMessage is null or empty</exception>
    public static Result Failure(string errorMessage) =>
        string.IsNullOrWhiteSpace(errorMessage)
            ? throw new ArgumentException("errorMessage cannot be empty", nameof(errorMessage))
            : new Result(false, errorMessage);



    /// <summary>
    /// Creates a successful <see cref="Result"/>>.
    /// </summary>
    public static Result Success() => new(true, string.Empty);



    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    /// <returns><see langword="true"/> if the operation succeeded, otherwise <see langword="false"/>.</returns>
    /// <remarks><see cref="Succeeded"/> and <see cref="Failed"/> are mutually exclusive</remarks>
    public bool Succeeded { get; }



    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    /// <returns><see langword="true"/> if the operation failed, otherwise <see langword="false"/>.</returns>
    /// <remarks><see cref="Succeeded"/> and <see cref="Failed"/> are mutually exclusive</remarks>
    public bool Failed => !Succeeded;



    /// <summary>
    /// The error message describing why the operation failed. Otherwise, an empty string if the operation succeeded.
    /// </summary>
    public string? ErrorMessage { get; }



    /// <summary>
    /// Takes one or more <see cref="Result"/>s and flattens them into a single <see cref="Result"/>.
    /// </summary>
    /// <param name="results">One or more <see cref="Result"/>s to flatten</param>
    /// <returns>
    /// If all the <see cref="Result"/>s were successful the return value is a successful <see cref="Result"/>.
    /// If one or more failed, the return value is a failed <see cref="Result"/> and the ErrorMessage 
    /// property will contain the errors from each failed <see cref="Result"/> separated by a newline character.
    /// </returns>
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
    /// Returns true if any of the specified <see cref="Result"/>s indicate a failure. 
    /// Otherwise, false.
    /// </summary>
    /// <param name="results">The array of <see cref="Result"/>> to review</param>
    /// <returns>
    /// <see langword="true"/> if any of the specified <see cref="Result"/>s failed, otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">results is null</exception>
    public static bool AnyFailed(params Result[]? results) =>
        results?.Any(r => r.Failed) ?? throw new ArgumentNullException(nameof(results));



    /// <summary>
    /// Returns true if all the specified <see cref="Result"/>s indicate success. 
    /// </summary>
    /// <param name="results">The array of <see cref="Result"/>> to review</param>
    /// <returns>
    /// <see langword="true"/> if all the specified <see cref="Result"/>s succeeded, otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">results is null</exception>
    public static bool AllSucceeded(params Result[]? results) =>
        results?.All(r => r.Succeeded) ?? throw new ArgumentNullException(nameof(results));
}



/// <summary>
/// The result of executing an <seealso cref="Func{T}"/>. Contains properties indicating whether the operation
/// <see cref="Result.Succeeded"/> or <see cref="Result.Failed"/>. If the operation failed the
/// <see cref="Result.ErrorMessage"/> property will contain message as to why. If the operation succeeded the
/// <see cref="Result{T}.Value"/> property will contain the return value from the function.
/// </summary>
public class Result<T> : Result
{


#if NET5_0_OR_GREATER
    private readonly T? _value;
#else
    private readonly T _value;
#endif


    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> class with the specified success status,
    /// exception information, and return value.
    /// </summary>
    /// <param name="succeeded">A value indicating whether the operation succeeded. Set to <see langword="true"/>
    /// if the operation was successful; otherwise, <see langword="false"/>.</param>
    /// <param name="errorMessage">Error errorMessage associated with the result.</param>
    /// <param name="value">The return value of the function if it succeeded, otherwise the default value for {T}</param>
    /// <remarks>
    /// If the operation was successful, errorMessage must be an empty string and value should be the return value from the function.
    /// If the operation failed errorMessage must not be null or empty and the value should be default{T}
    /// </remarks>
#if NET5_0_OR_GREATER
    private Result(bool succeeded, string? errorMessage, T? value) : base(succeeded, errorMessage) => _value = value;
#else
    private Result(bool succeeded, string? errorMessage, T value) : base(succeeded, errorMessage) => _value = value;
#endif



    /// <summary>
    /// Creates a failed Result with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message indicating the reason for failure.</param>
    /// <exception cref="ArgumentException">errorMessage is null or empty</exception>
#if NET5_0_OR_GREATER
    public static new Result<T?> Failure(string errorMessage) =>
        string.IsNullOrWhiteSpace(errorMessage)
            ? throw new ArgumentException("errorMessage cannot be empty", nameof(errorMessage))
            : new Result<T?>(false, errorMessage, default!);
#else
    public static new Result<T> Failure(string errorMessage) =>
        string.IsNullOrWhiteSpace(errorMessage)
            ? throw new ArgumentException("errorMessage cannot be empty", nameof(errorMessage))
            : new Result<T>(false, errorMessage, default!);
#endif



    /// <summary>
    /// Creates a successful <see cref="Result"/> with specified value.
    /// </summary>
#if NET5_0_OR_GREATER
    public static Result<T?> Success(T? value) => new(true, string.Empty, value);
#else
    public static Result<T> Success(T value) => new(true, string.Empty, value);
#endif



    /// <summary>
    /// The value produced by the operation if it succeeded.
    /// </summary>
    /// <exception cref="InvalidOperationException">Retrieving this property if the operation failed</exception>
#if NET5_0_OR_GREATER
    public T? Value => Failed
        ? throw new InvalidOperationException("Cannot access the Value of a failed Result.")
        : _value;
#else
    public T Value => Failed
        ? throw new InvalidOperationException("Cannot access the Value of a failed Result.")
        : _value;
#endif




}