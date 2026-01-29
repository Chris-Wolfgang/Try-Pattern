# Introduction

Welcome to the Try Pattern library! This library provides a simple and effective way to implement the Try Pattern in your C# applications.

## What is the Try Pattern?

The Try Pattern is a design pattern where code execution returns success/failure status without exposing detailed exception information to the caller. This is particularly useful when:

- The caller only needs to know if an operation succeeded or failed
- The specific reason for failure is not relevant to the caller
- You want to avoid using exceptions for control flow

## Why Use the Try Pattern?

### Benefits

1. **Simplified Error Handling**: No need to catch and handle specific exceptions
2. **Cleaner Code**: Reduces try-catch blocks throughout your codebase
3. **Better Performance**: Avoids the overhead of exception creation for expected failures
4. **Clearer Intent**: Makes it explicit that an operation may fail

### Real-World Examples

You're already familiar with the Try Pattern from .NET's built-in types:
- `int.TryParse(string, out int)`
- `DateTime.TryParse(string, out DateTime)`
- `Dictionary<TKey, TValue>.TryGetValue(TKey, out TValue)`

## How This Library Helps

This library extends the Try Pattern to any operation by providing:

1. **`Try` Class**: Static helper methods to execute operations and handle exceptions
2. **`Result` Type**: Represents the outcome of an operation without a return value
3. **`Result<T>` Type**: Represents the outcome of an operation that returns a value

## Quick Example

```csharp
// Without Try Pattern - traditional approach
try
{
    var content = await File.ReadAllTextAsync("config.json");
    ProcessConfig(content);
}
catch (Exception ex)
{
    Console.WriteLine("Error reading config: " + ex.Message);
}

// With Try Pattern - simplified approach
var result = await Try.RunAsync(() => File.ReadAllTextAsync("config.json"));
if (result.Succeeded)
{
    ProcessConfig(result.Value);
}
else
{
    Console.WriteLine("Error reading config: " + result.ErrorMessage);
}
```

## When to Use the Try Pattern

**Use the Try Pattern when:**
- Operations may fail in expected ways (file not found, network timeout, etc.)
- The caller only needs success/failure information
- You want to avoid exception-based control flow

**Don't use the Try Pattern when:**
- You need specific exception details for different error handling
- The failure is truly exceptional and should propagate up the call stack
- You're dealing with programming errors (null arguments, invalid state)

## Next Steps

Ready to get started? Check out the [Getting Started](getting-started.md) guide to learn how to install and use the library in your projects.
