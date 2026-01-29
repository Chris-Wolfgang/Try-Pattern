# Getting Started

This guide will help you get up and running with the Try Pattern library in just a few minutes.

## Installation

### NuGet Package

Install the Try Pattern library via NuGet Package Manager:

```bash
dotnet add package Wolfgang.TryPattern
```

Or using the Package Manager Console:

```powershell
Install-Package Wolfgang.TryPattern
```

### Manual Installation

Alternatively, you can clone the repository and reference the project directly:

```bash
git clone https://github.com/Chris-Wolfgang/Try-Pattern.git
```

## Basic Usage

### 1. Using Try with Async Operations

The most common use case is wrapping async operations that may fail:

```csharp
using Wolfgang.TryPattern;

// Reading a file that might not exist
var result = await Try.RunAsync(() => File.ReadAllTextAsync("data.txt"));

if (result.Succeeded)
{
    Console.WriteLine($"File content: {result.Value}");
}
else
{
    Console.WriteLine($"Failed to read file: {result.ErrorMessage}");
}
```

### 2. Using Try with Synchronous Operations

You can also use the Try Pattern with synchronous operations:

```csharp
using Wolfgang.TryPattern;

// Parsing a JSON file
var result = Try.Run(() => JsonSerializer.Deserialize<MyData>(jsonString));

if (result.Succeeded)
{
    ProcessData(result.Value);
}
else
{
    LogError(result.ErrorMessage);
}
```

### 3. Using Result for Custom Methods

Use `Result` and `Result<T>` as return types for your own methods:

```csharp
using Wolfgang.TryPattern;

public Result ValidateEmail(string email)
{
    if (string.IsNullOrWhiteSpace(email))
    {
        return Result.Failure("Email cannot be empty");
    }
    
    if (!email.Contains("@"))
    {
        return Result.Failure("Email must contain @");
    }
    
    return Result.Success();
}

public Result<User> GetUser(int userId)
{
    var user = database.FindUser(userId);
    
    if (user == null)
    {
        return Result<User>.Failure("User not found");
    }
    
    return Result<User>.Success(user);
}
```

## Common Patterns

### Pattern 1: File Operations

```csharp
var result = await Try.RunAsync(() => File.ReadAllTextAsync(filePath));
if (!result.Succeeded)
{
    // Handle failure - file doesn't exist, no permissions, etc.
    return;
}

// Process the file content
var content = result.Value;
```

### Pattern 2: HTTP Requests

```csharp
public async Task<Result<string>> FetchDataAsync(string url)
{
    using var response = await httpClient.GetAsync(url);
    
    if (!response.IsSuccessStatusCode)
    {
        return Result<string>.Failure($"HTTP {response.StatusCode}");
    }
    
    var content = await response.Content.ReadAsStringAsync();
    return Result<string>.Success(content);
}
```

### Pattern 3: Database Operations

```csharp
public async Task<Result> SaveUserAsync(User user)
{
    if (user == null)
    {
        throw new ArgumentNullException(nameof(user)); // Programming error
    }
    
    var saveResult = await Try.RunAsync(() => database.SaveAsync(user));
    
    if (!saveResult.Succeeded)
    {
        return Result.Failure("Failed to save user to database");
    }
    
    return Result.Success();
}
```

## Examples

The repository includes several example projects demonstrating the Try Pattern in different scenarios:

- **C# .NET 8 Example**: Modern C# with latest features
- **C# .NET 4.6.2 Example**: Legacy framework support
- **F# Examples**: Functional programming approach
- **VB.NET Examples**: Visual Basic support

You can find these examples in the `examples` folder of the repository.

## API Reference

### Try Class

- `Try.Run<T>(Func<T> func)`: Execute a synchronous function
- `Try.RunAsync<T>(Func<Task<T>> func)`: Execute an async function
- `Try.Run(Action action)`: Execute a synchronous action
- `Try.RunAsync(Func<Task> func)`: Execute an async action

### Result Class

- `Result.Success()`: Create a successful result
- `Result.Failure(string message)`: Create a failed result
- `Succeeded`: Boolean indicating success
- `ErrorMessage`: Error message when failed

### Result<T> Class

- `Result<T>.Success(T value)`: Create a successful result with a value
- `Result<T>.Failure(string message)`: Create a failed result
- `Succeeded`: Boolean indicating success
- `Value`: The returned value when successful
- `ErrorMessage`: Error message when failed

## Next Steps

- Read the [Introduction](introduction.md) for a deeper understanding of the Try Pattern
- Check out the examples in the `examples` folder
- Review the [readme.md](readme.md) for more detailed examples
- Explore the source code in the `src/Wolfgang.TryPattern` folder

## Need Help?

- Open an issue on [GitHub](https://github.com/Chris-Wolfgang/Try-Pattern/issues)
- Check the [Contributing Guide](CONTRIBUTING.md) if you want to contribute
- Review the [Code of Conduct](CODE_OF_CONDUCT.md)
