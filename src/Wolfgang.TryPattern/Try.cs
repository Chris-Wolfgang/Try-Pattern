using System;
using System.Threading;
using System.Threading.Tasks;

namespace Wolfgang.TryPattern;

/// <summary>
/// Provides methods to execute actions and functions with automatic exception handling returning
/// a <see cref="Result"/> representing the outcome of the action/function and the return value
/// of the function if successful or an error message if the action/function failed.
/// </summary>
public static class Try
{
    /// <summary>
    /// Executes the specified action, catching any exception that may occur.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>
    /// A <see cref="Result"/> that indicates if the action was successful.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
    public static Result Run(Action action)
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
            return Result.Failure(ex.Message);
        }
    }



    /// <summary>
    /// Executes the specified function, catching any exception that may occur.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> indicating if the function was successful or not and the result of
    /// the function if it was.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
#if NET5_0_OR_GREATER
    public static Result<T?> Run<T>(Func<T> function)
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
            return Result<T?>.Failure(ex.Message);
        }
    }
#else
    public static Result<T> Run<T>(Func<T>? function)
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
            return Result<T>.Failure(ex.Message);
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
    /// A <see cref="Task"/> of <see cref="Result"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
    /// <exception cref="OperationCanceledException">
    /// Cancellation was observed via <paramref name="token"/> (either before
    /// <paramref name="action"/> started, or because <paramref name="action"/> itself observed
    /// the token and threw).
    /// </exception>
    public static async Task<Result> RunAsync(Action action, CancellationToken token = default)
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
            return Result.Failure(ex.Message);
        }
    }



    /// <summary>
    /// Executes the specified function, catching any exception that may occur.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="function">The function to execute.</param>
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
    /// A <see cref="Task"/> of <see cref="Result{T}"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
    /// <exception cref="OperationCanceledException">
    /// <paramref name="token"/> was already canceled when this method was called, or
    /// <paramref name="function"/> observed cancellation during execution and let an
    /// <see cref="OperationCanceledException"/> escape.
    /// </exception>
#if NET5_0_OR_GREATER
    public static async Task<Result<T?>> RunAsync<T>(Func<Task<T?>> function, CancellationToken token = default)
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
            return Result<T?>.Failure(ex.Message);
        }
    }
#else
    public static async Task<Result<T>> RunAsync<T>(Func<Task<T>> function, CancellationToken token = default)
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
            return Result<T>.Failure(ex.Message);
        }
    }
#endif
}
