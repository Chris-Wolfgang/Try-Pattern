using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Wolfgang.TryPattern;



/// <summary>
/// Represents the outcome of an operation. Contains properties indicating whether the operation
/// succeeded or failed. If the operation failed the <see cref="Result.ErrorMessage"/> property will
/// contain a message as to why.
/// </summary>
/// <remarks>
/// Commonly produced by <see cref="Try.Run(Action)"/>, but also useful directly as a return type
/// from validation helpers, repository methods, and other service-layer code.
/// </remarks>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the Result class with the specified success status and
    /// exception information.
    /// </summary>
    /// <param name="succeeded">A value indicating whether the operation succeeded. Set to <see langword="true"/>
    /// if the operation was successful; otherwise, <see langword="false"/>.</param>
    /// <param name="errorMessage">Error message associated with the result.</param>
    /// <remarks>
    /// If the operation was successful, <paramref name="errorMessage"/> must be an empty string. If the operation failed,
    /// <paramref name="errorMessage"/> must not be null, empty, or whitespace.
    /// </remarks>
    protected Result
    (
        bool succeeded,
        string? errorMessage
    )
    {
        if (succeeded && errorMessage != string.Empty)
        {
            throw new ArgumentException
            (
                "A successful result cannot have an error message.",
                nameof(errorMessage)
            );
        }

        if (!succeeded && string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new ArgumentException
            (
                "A failed result must have an error message.",
                nameof(errorMessage)
            );
        }

        Succeeded = succeeded;
        ErrorMessage = errorMessage;
    }



    /// <summary>
    /// Creates a failed <see cref="Result"/> with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message indicating the reason for failure.</param>
    /// <returns>A failed <see cref="Result"/> whose <see cref="ErrorMessage"/> is set to <paramref name="errorMessage"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="errorMessage"/> is null, empty, or whitespace.</exception>
    public static Result Failure(string errorMessage) =>
        string.IsNullOrWhiteSpace(errorMessage)
            ? throw new ArgumentException("errorMessage cannot be null, empty, or whitespace.", nameof(errorMessage))
            : new Result(succeeded: false, errorMessage);



    private static readonly Result _successInstance = new(succeeded: true, string.Empty);



    /// <summary>
    /// Creates a successful <see cref="Result"/>.
    /// </summary>
    /// <returns>A successful <see cref="Result"/>.</returns>
    /// <remarks>
    /// Returns a cached singleton instance. <see cref="Result"/> is immutable, so reusing
    /// the same instance is safe and avoids per-call allocations on hot paths. Callers must
    /// not rely on reference identity to distinguish results: every call to <see cref="Success"/>
    /// returns the same object, so two successful results will be reference-equal.
    /// </remarks>
    public static Result Success() => _successInstance;



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
    /// Gets the error message describing why the operation failed, or an empty string if the operation succeeded.
    /// </summary>
    public string? ErrorMessage { get; }



    /// <summary>
    /// Takes zero or more <see cref="Result"/>s and flattens them into a single <see cref="Result"/>.
    /// </summary>
    /// <param name="results">Zero or more <see cref="Result"/>s to flatten</param>
    /// <returns>
    /// If all the <see cref="Result"/>s were successful (or the array is empty) the return value
    /// is a successful <see cref="Result"/>. If one or more failed, the return value is a failed
    /// <see cref="Result"/> and the ErrorMessage property will contain the errors from each failed
    /// <see cref="Result"/> separated by a newline character.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="results"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="results"/> contains a null element.
    /// </exception>
    public static Result Flatten([NotNull] params Result[]? results)
    {
        if (results == null)
        {
            throw new ArgumentNullException(nameof(results));
        }

        // Single-pass scan: walk the array once, validating element non-nullness
        // inline and remembering the index of the first failure (if any). Because
        // we visit every element across the head loop and the tail loop below,
        // no element escapes null validation even though we short-circuit the
        // head loop on the first failure.
        var firstFailureIndex = -1;
        for (var i = 0; i < results.Length; i++)
        {
            if (results[i] == null)
            {
                throw new ArgumentException($"Element at index {i} is null. The results array must not contain null elements.", nameof(results));
            }

            if (results[i].Failed)
            {
                firstFailureIndex = i;
                break;
            }
        }

        if (firstFailureIndex == -1)
        {
            return Success();
        }

        // Scan forward from the first failure to collect any additional failure
        // messages. Every remaining index is visited, so null validation continues
        // to cover the tail of the array.
        var firstMessage = results[firstFailureIndex].ErrorMessage!;
        StringBuilder? builder = null;
        for (var i = firstFailureIndex + 1; i < results.Length; i++)
        {
            if (results[i] == null)
            {
                throw new ArgumentException($"Element at index {i} is null. The results array must not contain null elements.", nameof(results));
            }

            if (!results[i].Failed)
            {
                continue;
            }

            if (builder == null)
            {
                builder = new StringBuilder(firstMessage);
            }

            builder.Append('\n').Append(results[i].ErrorMessage);
        }

        return Failure(builder?.ToString() ?? firstMessage);
    }



    /// <summary>
    /// Returns true if any of the specified <see cref="Result"/>s indicate a failure.
    /// Otherwise, false.
    /// </summary>
    /// <param name="results">The array of <see cref="Result"/> to review.</param>
    /// <returns>
    /// <see langword="true"/> if any of the specified <see cref="Result"/>s failed, otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="results"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// A null element is encountered before the first failure. Note that elements after the first
    /// failure are not inspected, so a trailing null may go undetected when at least one earlier
    /// element has already failed.
    /// </exception>
    public static bool AnyFailed([NotNull] params Result[]? results)
    {
        if (results == null)
        {
            throw new ArgumentNullException(nameof(results));
        }

        for (var i = 0; i < results.Length; i++)
        {
            if (results[i] == null)
            {
                throw new ArgumentException($"Element at index {i} is null. The results array must not contain null elements.", nameof(results));
            }

            if (results[i].Failed)
            {
                return true;
            }
        }

        return false;
    }



    /// <summary>
    /// Returns true if all the specified <see cref="Result"/>s indicate success.
    /// </summary>
    /// <param name="results">The array of <see cref="Result"/> to review.</param>
    /// <returns>
    /// <see langword="true"/> if all the specified <see cref="Result"/>s succeeded, otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="results"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// A null element is encountered before the first failure. Note that elements after the first
    /// failure are not inspected, so a trailing null may go undetected when at least one earlier
    /// element has already failed.
    /// </exception>
    public static bool AllSucceeded([NotNull] params Result[]? results)
    {
        if (results == null)
        {
            throw new ArgumentNullException(nameof(results));
        }

        for (var i = 0; i < results.Length; i++)
        {
            if (results[i] == null)
            {
                throw new ArgumentException($"Element at index {i} is null. The results array must not contain null elements.", nameof(results));
            }

            if (results[i].Failed)
            {
                return false;
            }
        }

        return true;
    }
}



/// <summary>
/// Represents the outcome of an operation that produces a value of type <typeparamref name="T"/>.
/// Contains properties indicating whether the operation <see cref="Result.Succeeded"/> or
/// <see cref="Result.Failed"/>. If the operation failed the <see cref="Result.ErrorMessage"/> property
/// will contain a message as to why. If the operation succeeded the <see cref="Result{T}.Value"/>
/// property will contain the returned value.
/// </summary>
/// <typeparam name="T">The type of value returned on success.</typeparam>
/// <remarks>
/// Commonly produced by <see cref="Try.Run{T}(Func{T})"/>, but also useful directly as a return type
/// from repository, service, or validation code that wants to surface a value-or-error outcome.
/// </remarks>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1000:Do not declare static members on generic types",
    Justification = "Result<T>.Failure / Result<T>.Success are factory methods central to the public API; consumers explicitly specify T at the call site by design.")]
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
    /// <param name="errorMessage">Error message associated with the result.</param>
    /// <param name="value">The return value of the function if it succeeded, otherwise the default value for <typeparamref name="T"/>.</param>
    /// <remarks>
    /// If the operation was successful, <paramref name="errorMessage"/> must be an empty string and
    /// <paramref name="value"/> should be the return value from the function.
    /// If the operation failed, <paramref name="errorMessage"/> must not be null, empty, or whitespace and
    /// <paramref name="value"/> should be <c>default(T)</c>.
    /// </remarks>
#if NET5_0_OR_GREATER
    private Result(bool succeeded, string? errorMessage, T? value) : base(succeeded, errorMessage) => _value = value;
#else
    private Result(bool succeeded, string? errorMessage, T value) : base(succeeded, errorMessage) => _value = value;
#endif



    /// <summary>
    /// Creates a failed <see cref="Result{T}"/> with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message indicating the reason for failure.</param>
    /// <returns>A failed <see cref="Result{T}"/> whose <see cref="Result.ErrorMessage"/> is set to <paramref name="errorMessage"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="errorMessage"/> is null, empty, or whitespace.</exception>
#if NET5_0_OR_GREATER
    public static new Result<T?> Failure(string errorMessage) =>
        string.IsNullOrWhiteSpace(errorMessage)
            ? throw new ArgumentException("errorMessage cannot be null, empty, or whitespace.", nameof(errorMessage))
            : new Result<T?>(succeeded: false, errorMessage, default!);
#else
    public static new Result<T> Failure(string errorMessage) =>
        string.IsNullOrWhiteSpace(errorMessage)
            ? throw new ArgumentException("errorMessage cannot be null, empty, or whitespace.", nameof(errorMessage))
            : new Result<T>(succeeded: false, errorMessage, default!);
#endif



    /// <summary>
    /// Creates a successful <see cref="Result{T}"/> with the specified value.
    /// </summary>
    /// <param name="value">The value produced by the operation.</param>
    /// <returns>A successful <see cref="Result{T}"/> whose <see cref="Value"/> is <paramref name="value"/>.</returns>
#if NET5_0_OR_GREATER
    public static Result<T?> Success(T? value) => new(succeeded: true, string.Empty, value);
#else
    public static Result<T> Success(T value) => new(succeeded: true, string.Empty, value);
#endif



    /// <summary>
    /// Gets the value produced by the operation if it succeeded; throws if the operation failed.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when this property is accessed after the operation has failed.</exception>
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
