using System;
using System.Threading.Tasks;

namespace Wolfgang.TryPattern;

/// <summary>
/// Provides methods to execute actions and functions with automatic exception handling.
/// </summary>
public static class Try
{
    /// <summary>
    /// Executes an action and swallows any exceptions that occur.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public static Result Action(Action action)
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
    /// Executes an asynchronous action and swallows any exceptions that occur.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task<Result> ActionAsync(Action action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        try
        {
            await Task.Run(action).ConfigureAwait(false);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }



    /// <summary>
    /// Executes a function and returns its result, or the default value of T if an exception occurs.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <returns>The result of the function, or default(T) if an exception occurs.</returns>
#if NET5_0_OR_GREATER
    public static Result<T?> Function<T>(Func<T> function)
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
    public static Result<T> Function<T>(Func<T>? function)
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
    /// Executes an asynchronous function and returns its result, or the default value of T if an exception occurs.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="function">The asynchronous function to execute.</param>
    /// <returns>A task representing the asynchronous operation that returns the result of the function, or default(T) if an exception occurs.</returns>
#if NET5_0_OR_GREATER
    public static async Task<Result<T?>> FunctionAsync<T>(Func<Task<T>> function)
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }
        
        try
        {
            var result = await function().ConfigureAwait(false);
            return Result<T?>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<T?>.Failure(ex.Message);
        }
    }
#else
    public static async Task<Result<T>> FunctionAsync<T>(Func<Task<T>> function)
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }
        
        try
        {
            var result = await function().ConfigureAwait(false);
            return Result<T>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(ex.Message);
        }
    }
#endif
}




