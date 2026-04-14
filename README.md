# Wolfgang.TryPattern

[![NuGet](https://img.shields.io/nuget/v/Wolfgang.TryPattern.svg)](https://www.nuget.org/packages/Wolfgang.TryPattern)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A lightweight .NET library that provides a Try/Result pattern for executing actions and functions with automatic exception handling. Instead of try/catch blocks scattered throughout your code, wrap operations in `Try.Run()` and get back a `Result` indicating success or failure.

- **GitHub Repository:** [https://github.com/Chris-Wolfgang/Try-Pattern](https://github.com/Chris-Wolfgang/Try-Pattern)
- **API Documentation:** [https://chris-wolfgang.github.io/Try-Pattern/](https://chris-wolfgang.github.io/Try-Pattern/)
- **API Reference:** [https://chris-wolfgang.github.io/Try-Pattern/api/](https://chris-wolfgang.github.io/Try-Pattern/api/)
- **Formatting Guide:** [docs/README-FORMATTING.md](docs/README-FORMATTING.md)
- **Contributing Guide:** [CONTRIBUTING.md](CONTRIBUTING.md)

---

## Installation

### Via .NET CLI

```bash
dotnet add package Wolfgang.TryPattern
```

### Via Package Manager Console

```powershell
Install-Package Wolfgang.TryPattern
```

---

## Quick Start

### Execute an action safely

```csharp
using Wolfgang.TryPattern;

var result = Try.Run(() => File.Delete("temp.txt"));

if (result.Succeeded)
{
    Console.WriteLine("File deleted.");
}
else
{
    Console.WriteLine($"Failed: {result.ErrorMessage}");
}
```

### Execute a function and get the return value

```csharp
var result = Try.Run(() => int.Parse("42"));

if (result.Succeeded)
{
    Console.WriteLine($"Parsed value: {result.Value}");
}
else
{
    Console.WriteLine($"Parse failed: {result.ErrorMessage}");
}
```

### Async support

```csharp
var result = await Try.RunAsync(async () =>
{
    var response = await httpClient.GetStringAsync("https://example.com");
    return response;
});

if (result.Failed)
{
    Console.WriteLine($"Request failed: {result.ErrorMessage}");
}
```

### Cancellation support

```csharp
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

// OperationCanceledException is rethrown, not captured
var result = await Try.RunAsync(() => LongRunningWork(), cts.Token);
```

---

## Features

| Feature | Description |
|---------|-------------|
| `Try.Run(Action)` | Execute an action, return `Result` |
| `Try.Run<T>(Func<T>)` | Execute a function, return `Result<T>` with the value |
| `Try.RunAsync(Action, CancellationToken)` | Async action execution with cancellation support |
| `Try.RunAsync<T>(Func<Task<T>>, CancellationToken)` | Async function execution with cancellation support |
| `Result.Success()` | Create a successful result |
| `Result.Failure(message)` | Create a failed result with an error message |
| `Result<T>.Success(value)` | Create a successful result with a value |
| `Result<T>.Failure(message)` | Create a failed result |
| `Result.Flatten(results)` | Combine multiple results into one |
| `Result.AnyFailed(results)` | Check if any results failed |
| `Result.AllSucceeded(results)` | Check if all results succeeded |

### Result Properties

| Property | Description |
|----------|-------------|
| `Succeeded` | `true` if the operation completed successfully |
| `Failed` | `true` if the operation failed (inverse of `Succeeded`) |
| `ErrorMessage` | The error message if failed, empty string if succeeded |
| `Value` | (Generic only) The return value if succeeded, throws `InvalidOperationException` if failed |

---

## Combining Results

```csharp
var r1 = Try.Run(() => ValidateName(name));
var r2 = Try.Run(() => ValidateEmail(email));
var r3 = Try.Run(() => ValidateAge(age));

// Flatten into a single result
var combined = Result.Flatten(r1, r2, r3);

if (combined.Failed)
{
    // ErrorMessage contains all failures separated by newlines
    Console.WriteLine(combined.ErrorMessage);
}

// Or check individually
if (Result.AnyFailed(r1, r2, r3))
{
    Console.WriteLine("At least one validation failed.");
}
```

---

## Target Frameworks

| Framework | Version |
|-----------|---------|
| .NET Framework | 4.6.2+ |
| .NET Standard | 2.0 |
| .NET | 8.0, 10.0 |

---

## Building from Source

```bash
# Clone the repository
git clone https://github.com/Chris-Wolfgang/Try-Pattern.git
cd Try-Pattern

# Restore and build
dotnet restore
dotnet build --configuration Release

# Run tests
dotnet test --configuration Release

# Format code
dotnet format

# Verify formatting
dotnet format --verify-no-changes
```

---

## License

This project is licensed under the [MIT License](LICENSE).
