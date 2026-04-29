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
    /// <param name="token">The CancellationToken to monitor.</param>
    /// <returns>
    /// A <see cref="Task"/> of <see cref="Result"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
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
    /// <param name="token">The CancellationToken to monitor.</param>
    /// <returns>
    /// A <see cref="Task"/> of <see cref="Result{T}"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="function"/> is null.</exception>
#if NET5_0_OR_GREATER
    public static async Task<Result<T?>> RunAsync<T>(Func<Task<T?>> function, CancellationToken token = default)
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        // The token parameter is part of the public API for call-site signalling but cannot be
        // passed into Func<Task<T?>> (no token parameter on the delegate). Discard it explicitly.
        _ = token;

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

        // The token parameter is part of the public API for call-site signalling but cannot be
        // passed into Func<Task<T>> (no token parameter on the delegate). Discard it explicitly.
        _ = token;

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
