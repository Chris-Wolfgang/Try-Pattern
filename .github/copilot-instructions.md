# GitHub Copilot Instructions for Try-Pattern

## Project Overview
This is a C# library that implements the Try Pattern - a design pattern where code execution returns success/failure status without exposing detailed exception information to the caller.

## Core Principles

### 1. Try Pattern Philosophy
- Use the Try Pattern when callers only care about success/failure, not specific error details
- Return `Result` for void operations, `Result<T>` for operations that return values
- Exceptions should be caught internally and converted to failure results
- Reserve throwing exceptions for programming errors (null arguments, contract violations)

### 2. Exception Handling Guidelines
- **DO NOT** throw exceptions for expected failures (file not found, network timeout, etc.)
- **DO** throw exceptions for programming errors (null arguments, invalid state)
- Wrap operations that may throw with `Try. Run()` or `Try.RunAsync()`
- Use `Result.Failure()` for expected error conditions
- Use `Result.Success()` or `Result<T>.Success(value)` for successful operations

### 3. When to Use Exceptions vs Result
**Use Result (no exceptions):**
- User-provided file doesn't exist
- Network requests fail
- Database connection issues
- Validation failures (expected failures)
- Any scenario where failure is a normal possibility

**Use Exceptions:**
- Null arguments to public methods
- Invalid method parameters
- Programming contract violations
- Unexpected internal state

## Code Style & Conventions

### Naming Conventions
- Use PascalCase for public methods and properties
- Use camelCase for private fields and parameters
- Use descriptive names that clearly indicate purpose

### Method Structure
```csharp
// For public methods that may fail, return Result or Result<T>
public async Task<Result<Order>> GetOrderAsync(string orderId)
{
    // Validate arguments - throw for programming errors
    if (string. IsNullOrWhiteSpace(orderId))
    {
        throw new ArgumentNullException(nameof(orderId));
    }
    
    // Use Try.RunAsync for operations that may fail
    var result = await Try.RunAsync(async () => {
        // Implementation that may throw
    });
    
    return result;
}
```

### Async/Await Patterns
- Suffix async methods with `Async`
- Prefer `Task<Result>` or `Task<Result<T>>` return types for async operations
- Use `await Try.RunAsync()` for async operations that need try pattern

### Result Usage
```csharp
// Creating success results
return Result.Success();
return Result<string>.Success("value");

// Creating failure results
return Result.Failure("Error message describing what failed");
return Result<string>. Failure("Error message");

// Checking results
if (result. Succeeded)
{
    // Use result.Value for Result<T>
}
else
{
    // Use result. ErrorMessage
}
```

## Testing Guidelines

### Unit Test Structure
- Use xUnit or similar testing framework
- Test both success and failure paths
- Verify `Succeeded` property is correct
- Verify `ErrorMessage` is populated on failure
- Verify `Value` is correct on success for `Result<T>`

### Test Naming
- Use descriptive test method names:  `MethodName_Scenario_ExpectedBehavior`
- Example: `ValidateUserInput_EmptyString_ReturnsFailure`

### Test Example
```csharp
[Fact]
public async Task TryRunAsync_FileNotFound_ReturnsFailureResult()
{
    // Arrange
    var nonExistentFile = "does-not-exist.txt";
    
    // Act
    var result = await Try.RunAsync(() => File.ReadAllTextAsync(nonExistentFile));
    
    // Assert
    Assert.False(result.Succeeded);
    Assert.NotNull(result.ErrorMessage);
}
```

## Documentation Standards

### XML Documentation
- All public types, methods, and properties must have XML documentation
- Include `<summary>`, `<param>`, `<returns>`, and `<exception>` tags
- Provide clear examples in `<example>` tags for public API

```csharp
/// <summary>
/// Executes an asynchronous operation and returns a result indicating success or failure.
/// </summary>
/// <typeparam name="T">The type of value returned by the operation.</typeparam>
/// <param name="func">The asynchronous function to execute.</param>
/// <returns>A Result<T> containing the value if successful, or an error message if failed.</returns>
/// <exception cref="ArgumentNullException">Thrown when func is null.</exception>
public static async Task<Result<T>> RunAsync<T>(Func<Task<T>> func)
```

## Benchmarking
- Add benchmarks for performance-critical operations
- Use BenchmarkDotNet
- Place benchmarks in the `benchmarks` directory
- Document performance characteristics in comments

## Dependencies
- Minimize external dependencies
- Target multiple . NET versions if possible
- Keep the library lightweight and focused

## Code Review Checklist
When generating code for this repository, ensure:
- [ ] Public methods return `Result` or `Result<T>` for operations that may fail
- [ ] Exceptions are only thrown for programming errors
- [ ] All public APIs have XML documentation
- [ ] Async methods use `ConfigureAwait(false)`
- [ ] Unit tests cover success and failure paths
- [ ] Code follows C# coding conventions
- [ ] Null checks are performed on public method parameters

## Common Patterns

### Pattern:  File I/O Operations
```csharp
var result = await Try.RunAsync(() => File.ReadAllTextAsync(filePath));
if (result.Succeeded)
{
    ProcessContent(result.Value);
}
```

### Pattern: HTTP Operations
```csharp
public async Task<Result<TResponse>> GetAsync<TResponse>(string url)
{
    using var response = await httpClient.GetAsync(url).ConfigureAwait(false);
    
    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content. ReadAsStringAsync().ConfigureAwait(false);
        var data = JsonSerializer.Deserialize<TResponse>(content);
        return Result<TResponse>.Success(data);
    }
    
    return Result<TResponse>. Failure($"Request failed:  {response.StatusCode}");
}
```

### Pattern: Validation
```csharp
public Result Validate(string input)
{
    if (string.IsNullOrWhiteSpace(input))
        return Result.Failure("Input cannot be empty");
        
    if (input.Length < 5)
        return Result. Failure("Input must be at least 5 characters");
        
    return Result. Success();
}
```
