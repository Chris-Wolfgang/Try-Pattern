# Introduction

Wolfgang.TryPattern is a lightweight .NET library that brings structured error handling to your codebase through the Try pattern. It replaces scattered try/catch blocks with a clean, composable API that makes error handling explicit and predictable.

## The Problem

Traditional .NET error handling relies on try/catch blocks that can obscure control flow, make it difficult to compose operations, and lead to inconsistent error handling across a codebase:

```csharp
string content;
try
{
    content = File.ReadAllText(path);
}
catch (Exception ex)
{
    // Now what? Log? Return null? Throw a different exception?
    logger.LogError(ex, "Failed to read file");
    return null;
}
```

## The Solution

Wolfgang.TryPattern wraps operations in `Try.Run` and returns a `Result` object that clearly communicates the outcome:

```csharp
var result = Try.Run(() => File.ReadAllText(path));

if (result.Succeeded)
{
    Console.WriteLine(result.Value);
}
else
{
    Console.WriteLine($"Error: {result.ErrorMessage}");
}
```

## Core Types

### `Try`

A static class providing methods to execute actions and functions with automatic exception handling:

- **`Try.Run(Action)`** - Execute an action, returning a `Result`
- **`Try.Run<T>(Func<T>)`** - Execute a function, returning a `Result<T>` with the return value
- **`Try.RunAsync(Action, CancellationToken)`** - Execute an action asynchronously
- **`Try.RunAsync<T>(Func<Task<T>>, CancellationToken)`** - Execute an async function

### `Result`

Represents the outcome of an operation:

- **`Succeeded`** - `true` if the operation completed without exceptions
- **`Failed`** - `true` if the operation threw an exception
- **`ErrorMessage`** - The exception message if the operation failed

### `Result<T>`

Extends `Result` with a typed return value:

- **`Value`** - The return value from the function (throws `InvalidOperationException` if accessed on a failed result)

### Composition Helpers

- **`Result.Flatten(params Result[])`** - Combine multiple results into one; if any failed, the error messages are joined
- **`Result.AnyFailed(params Result[])`** - Check if any result in a set indicates failure
- **`Result.AllSucceeded(params Result[])`** - Check if every result in a set indicates success

## Platform Support

Wolfgang.TryPattern targets multiple frameworks for broad compatibility:

| Target | Version |
|--------|---------|
| .NET Framework | 4.6.2+ |
| .NET Standard | 2.0 |
| .NET | 8.0, 10.0 |
