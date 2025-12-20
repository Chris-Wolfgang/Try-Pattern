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
    public static TryActionResult Action(Action action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        try
        {
            action();
            return new TryActionResult(true, null);
        }
        catch (Exception ex)
        {
            return new TryActionResult(false, ex);
        }
    }



    /// <summary>
    /// Executes an asynchronous action and swallows any exceptions that occur.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task<TryActionResult> ActionAsync(Func<Task> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        try
        {
            await action().ConfigureAwait(false);
            return new TryActionResult(true, null);
        }
        catch(Exception ex)
        {
            return new TryActionResult(false, ex);
        }
    }



    /// <summary>
    /// Executes a function and returns its result, or the default value of T if an exception occurs.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <returns>The result of the function, or default(T) if an exception occurs.</returns>
#if NET5_0_OR_GREATER
        public static TryFuncResult<T?> Function<T>(Func<T> function)
#else
    public static TryFuncResult<T> Function<T>(Func<T> function)
#endif
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        try
        {
            var result = function();
            return new TryFuncResult<T>(result);
        }
        catch (Exception ex)
        {
            return new TryFuncResult<T>(false, default, ex);
        }
    }



    /// <summary>
    /// Executes an asynchronous function and returns its result, or the default value of T if an exception occurs.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="function">The asynchronous function to execute.</param>
    /// <returns>A task representing the asynchronous operation that returns the result of the function, or default(T) if an exception occurs.</returns>
#if NET5_0_OR_GREATER
        public static async Task<TryFuncResult<T?>> FunctionAsync<T>(Func<Task<T>> function)
#else
    public static async Task<TryFuncResult<T>> FunctionAsync<T>(Func<Task<T>> function)
#endif
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }


        try
        {
            var result = await function().ConfigureAwait(false);
            return new TryFuncResult<T>(result);
        }
        catch (Exception ex)
        {
            return new TryFuncResult<T>(false, default, ex);
        }
    }
}




