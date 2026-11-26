using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Wolfgang.TryPattern;

/// <summary>
/// Provides methods to execute actions and functions with automatic exception handling returning
/// a <see cref="Result"/> representing the outcome of the action/function and the return value
/// of the function if successful or an error message if the action/function failed.
/// </summary>
/// <remarks>
/// When an exception is caught, the failed <see cref="Result"/>'s <see cref="Result.ErrorMessage"/>
/// is the exception's <see cref="Exception.Message"/>. If that message is null, empty, or
/// whitespace (which <see cref="Result.Failure(string)"/> would reject), the exception's type
/// name (for example <c>InvalidOperationException</c>) is used instead, so these methods never
/// throw from inside their own <c>catch</c> block.
/// <para>
/// The synchronous <c>Run</c> overloads capture every exception, including
/// <see cref="OperationCanceledException"/>, as a failed <see cref="Result"/>. The
/// <c>RunAsync</c> overloads rethrow <see cref="OperationCanceledException"/> instead, so
/// cancellation reaches the caller.
/// </para>
/// </remarks>
public static class Try
{
    /// <summary>
    /// Executes the specified action, catching any exception that may occur.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>
    /// A successful <see cref="Result"/>, or a failed <see cref="Result"/> whose
    /// <see cref="Result.ErrorMessage"/> is the caught exception's message (or its type name when
    /// the message is null, empty, or whitespace; see <see cref="Try"/>).
    /// </returns>
    /// <remarks>
    /// Every exception, including <see cref="OperationCanceledException"/>, becomes a failed
    /// <see cref="Result"/>. Use <see cref="RunAsync(Action, CancellationToken)"/> when cancellation
    /// must propagate to the caller.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result result = Try.Run(() => File.Delete("temp.txt"));
    /// if (result.Failed)
    /// {
    ///     Console.WriteLine("Delete failed: " + result.ErrorMessage);
    /// }
    /// </code>
    /// </example>
    public static Result Run([NotNull] Action? action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        try
        {
            action();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(SafeErrorMessage(ex));
        }
    }



    /// <summary>
    /// Executes the specified function and returns its result, catching any exception that may occur.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <returns>
    /// A successful <see cref="Result{T}"/> wrapping the function's return value, or a failed
    /// <see cref="Result{T}"/> whose <see cref="Result.ErrorMessage"/> is the caught exception's message
    /// (or its type name when the message is null, empty, or whitespace; see <see cref="Try"/>).
    /// </returns>
    /// <remarks>
    /// An <c>async</c> lambda passed here is not awaited: <typeparamref name="T"/> is inferred as
    /// <see cref="Task"/>, so the result is a successful <see cref="Result{T}"/> wrapping the
    /// still-running task and any exception it later throws is not captured. Use
    /// <see cref="RunAsync(Func{Task}, CancellationToken)"/> for asynchronous work.
    /// <para>
    /// Every exception, including <see cref="OperationCanceledException"/>, becomes a failed
    /// <see cref="Result{T}"/>; the <c>RunAsync</c> overloads rethrow cancellation instead.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; result = Try.Run(() => int.Parse("42"));
    /// if (result.Succeeded)
    /// {
    ///     Console.WriteLine("Parsed: " + result.Value);
    /// }
    /// </code>
    /// </example>
#if NET5_0_OR_GREATER
    public static Result<T?> Run<T>([NotNull] Func<T>? function)
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        try
        {
            return Result<T?>.Success(function());
        }
        catch (Exception ex)
        {
            return Result<T?>.Failure(SafeErrorMessage(ex));
        }
    }
#else
    public static Result<T> Run<T>([NotNull] Func<T>? function)
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        try
        {
            return Result<T>.Success(function());
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(SafeErrorMessage(ex));
        }
    }
#endif



    /// <summary>
    /// Executes the specified action asynchronously, catching any exception that may occur.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="token">
    /// A <see cref="CancellationToken"/> that is passed to <see cref="Task.Run(Action, CancellationToken)"/>.
    /// If cancellation is requested before <paramref name="action"/> begins executing, the task is
    /// canceled and an <see cref="OperationCanceledException"/> is propagated to the caller rather
    /// than wrapped in a failed <see cref="Result"/>. Once <paramref name="action"/> has started,
    /// <see cref="Task.Run(Action, CancellationToken)"/> cannot interrupt it; cancellation during
    /// execution is only observed if <paramref name="action"/> itself cooperatively checks the
    /// token (e.g. via <see cref="CancellationToken.ThrowIfCancellationRequested"/>) and throws.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> of <see cref="Result"/> representing the asynchronous operation. On a
    /// caught exception the failed <see cref="Result"/>'s <see cref="Result.ErrorMessage"/> is the
    /// exception's message (or its type name when the message is null, empty, or whitespace; see
    /// <see cref="Try"/>).
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
    /// <exception cref="OperationCanceledException">
    /// Cancellation was observed via <paramref name="token"/> (either before
    /// <paramref name="action"/> started, or because <paramref name="action"/> itself observed
    /// the token and threw).</exception>
    /// <remarks>
    /// A non-<c>async</c> lambda whose end cannot be reached (an endless loop, a bare
    /// <c>throw</c>) binds to <see cref="RunAsync(Func{Task}, CancellationToken)"/> instead,
    /// which runs it on the caller's thread. Cast such a lambda to <see cref="Action"/> to run it
    /// here, on the thread pool.
    /// </remarks>
    /// <example>
    /// <code>
    /// using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
    /// Result result = await Try.RunAsync(
    ///     () =>
    ///     {
    ///         foreach (int item in largeDataSet)
    ///         {
    ///             cts.Token.ThrowIfCancellationRequested();
    ///             Process(item);
    ///         }
    ///     },
    ///     cts.Token);
    /// </code>
    /// </example>
    public static async Task<Result> RunAsync([NotNull] Action? action, CancellationToken token = default)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        try
        {
            await Task.Run(action, token).ConfigureAwait(false);
            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return Result.Failure(SafeErrorMessage(ex));
        }
    }



    /// <summary>
    /// Executes the specified asynchronous function that produces no value, catching any exception
    /// that may occur.
    /// </summary>
    /// <param name="function">The asynchronous function to execute.</param>
    /// <param name="token">
    /// A <see cref="CancellationToken"/> that is checked before <paramref name="function"/> is
    /// invoked. If cancellation is already requested, an <see cref="OperationCanceledException"/>
    /// is thrown without invoking <paramref name="function"/>. The token is not threaded into
    /// <paramref name="function"/> itself; if you need cancellation inside the function, capture
    /// the token in the lambda's closure. If <paramref name="function"/> observes the token during
    /// execution and the resulting <see cref="OperationCanceledException"/> escapes, it is also
    /// propagated to the caller rather than wrapped in a failed <see cref="Result"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> of <see cref="Result"/> that completes when the task returned by
    /// <paramref name="function"/> completes. On a caught exception (thrown synchronously by
    /// <paramref name="function"/> or by the task it returns) the failed <see cref="Result"/>'s
    /// <see cref="Result.ErrorMessage"/> is the exception's message (or its type name when the
    /// message is null, empty, or whitespace; see <see cref="Try"/>).
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is the overload an <c>async</c> lambda with no return value binds to. Without it such a
    /// lambda would bind to <see cref="RunAsync(Action, CancellationToken)"/> as an <c>async void</c>
    /// delegate, which reports success before the work finishes and lets its exception escape.
    /// </para>
    /// <para>
    /// <paramref name="function"/> is invoked on the caller's thread, not through
    /// <see cref="Task.Run(Func{Task}, CancellationToken)"/>: everything before its first
    /// <c>await</c> runs synchronously, and this method does not return until then. C# also binds
    /// a non-<c>async</c> lambda whose end cannot be reached to this overload rather than to the
    /// <see cref="Action"/> one, for example an endless worker loop
    /// (<c>() =&gt; { while (true) { token.ThrowIfCancellationRequested(); Work(); } }</c>) or a
    /// bare <c>() =&gt; throw ...</c>. Such a loop then blocks the caller until it ends. To run it
    /// on the thread pool, cast it to <see cref="Action"/>:
    /// <c>Try.RunAsync((Action)(() =&gt; { ... }), token)</c>.
    /// </para>
    /// <para>
    /// Because several overloads take a single delegate, a literal <see langword="null"/> argument is
    /// ambiguous; cast it, as in <c>Try.RunAsync((Action)null!)</c>.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
    /// <exception cref="OperationCanceledException">
    /// <paramref name="token"/> was already canceled when this method was called, or
    /// <paramref name="function"/> observed cancellation during execution and let an
    /// <see cref="OperationCanceledException"/> escape.
    /// </exception>
    /// <example>
    /// <code>
    /// Result result = await Try.RunAsync(async () =>
    /// {
    ///     await File.WriteAllTextAsync("out.txt", "data");
    /// });
    /// if (result.Failed)
    /// {
    ///     Console.WriteLine("Write failed: " + result.ErrorMessage);
    /// }
    /// </code>
    /// </example>
    [SuppressMessage(
        "ApiDesign",
        "RS0026:Do not add multiple public overloads with optional parameters",
        Justification = "Matches the shipped RunAsync overloads, which all take an optional token. The delegate types (Action, Func<Task>, Func<Task<T>>) differ, so the optional token cannot make a call ambiguous.")]
    public static async Task<Result> RunAsync([NotNull] Func<Task>? function, CancellationToken token = default)
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        // Same contract as RunAsync<T>: fail fast on an already-canceled token, outside the try so
        // the OperationCanceledException reaches the caller unwrapped.
        token.ThrowIfCancellationRequested();

        try
        {
            await function().ConfigureAwait(false);
            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return Result.Failure(SafeErrorMessage(ex));
        }
    }



    /// <summary>
    /// Executes the specified asynchronous function, catching any exception that may occur.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="function">The asynchronous function to execute.</param>
    /// <param name="token">
    /// A <see cref="CancellationToken"/> that is checked before <paramref name="function"/> is
    /// invoked. If cancellation is already requested, an <see cref="OperationCanceledException"/>
    /// is thrown without invoking <paramref name="function"/>. The token is not threaded into
    /// <paramref name="function"/> itself; if you need cancellation inside the function, capture
    /// the token in the lambda's closure. If <paramref name="function"/> observes the token during
    /// execution (e.g. via <see cref="Task.Delay(int, CancellationToken)"/>) and the resulting
    /// <see cref="OperationCanceledException"/> escapes, it is also propagated to the caller
    /// rather than wrapped in a failed <see cref="Result{T}"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> of <see cref="Result{T}"/> representing the asynchronous operation. On a
    /// caught exception the failed <see cref="Result{T}"/>'s <see cref="Result.ErrorMessage"/> is the
    /// exception's message (or its type name when the message is null, empty, or whitespace; see
    /// <see cref="Try"/>).
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
    /// <exception cref="OperationCanceledException">
    /// <paramref name="token"/> was already canceled when this method was called, or
    /// <paramref name="function"/> observed cancellation during execution and let an
    /// <see cref="OperationCanceledException"/> escape.
    /// </exception>
#if NET5_0_OR_GREATER
    public static async Task<Result<T?>> RunAsync<T>([NotNull] Func<Task<T?>>? function, CancellationToken token = default)
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        // Observe the token before invoking. The delegate signature has no token parameter, so we
        // cannot flow it through; the best we can do is fail fast if cancellation was already
        // requested. ThrowIfCancellationRequested propagates OperationCanceledException directly
        // to the caller (it runs before the try/catch and is intentionally not wrapped in a Result).
        token.ThrowIfCancellationRequested();

        try
        {
            var result = await function().ConfigureAwait(false);
            return Result<T?>.Success(result);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return Result<T?>.Failure(SafeErrorMessage(ex));
        }
    }
#else
    public static async Task<Result<T>> RunAsync<T>([NotNull] Func<Task<T>>? function, CancellationToken token = default)
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        // Observe the token before invoking. The delegate signature has no token parameter, so we
        // cannot flow it through; the best we can do is fail fast if cancellation was already
        // requested. ThrowIfCancellationRequested propagates OperationCanceledException directly
        // to the caller (it runs before the try/catch and is intentionally not wrapped in a Result).
        token.ThrowIfCancellationRequested();

        try
        {
            var result = await function().ConfigureAwait(false);
            return Result<T>.Success(result);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(SafeErrorMessage(ex));
        }
    }
#endif



    // Coerce an exception message that Result.Failure would reject
    // (null / empty / whitespace) into a usable fallback so Try.Run
    // itself never throws from inside its catch block. Fixes #273:
    // consumers of exceptions with whitespace-only Message (or the
    // parameterless-ctor variants of some framework exceptions) were
    // getting an ArgumentException out of Try.Run rather than a
    // Failed Result.
    private static string SafeErrorMessage(Exception ex)
    {
        string message = ex.Message;
        return string.IsNullOrWhiteSpace(message) ? ex.GetType().Name : message;
    }
}
