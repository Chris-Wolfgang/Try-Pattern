namespace TryPattern;

/// <summary>
/// Provides methods to execute actions and functions with automatic exception handling.
/// </summary>
public static class Try
{
    /// <summary>
    /// Executes an action and swallows any exceptions that occur.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public static void Action(Action action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        try
        {
            action();
        }
        catch
        {
            // Swallow exception
        }
    }

    /// <summary>
    /// Executes an asynchronous action and swallows any exceptions that occur.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task ActionAsync(Func<Task> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        try
        {
            await action().ConfigureAwait(false);
        }
        catch
        {
            // Swallow exception
        }
    }

    /// <summary>
    /// Executes a function and returns its result, or the default value of T if an exception occurs.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <returns>The result of the function, or default(T) if an exception occurs.</returns>
    public static T? Function<T>(Func<T> function)
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        try
        {
            return function();
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Executes an asynchronous function and returns its result, or the default value of T if an exception occurs.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="function">The asynchronous function to execute.</param>
    /// <returns>A task representing the asynchronous operation that returns the result of the function, or default(T) if an exception occurs.</returns>
    public static async Task<T?> FunctionAsync<T>(Func<Task<T>> function)
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        try
        {
            return await function().ConfigureAwait(false);
        }
        catch
        {
            return default;
        }
    }
}
