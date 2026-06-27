# Getting Started

## Prerequisites

- .NET 10 SDK or later (required to build and test this repository, because the library multi-targets .NET 10.0)
- Any project targeting .NET Framework 4.6.2+, .NET Standard 2.0, .NET 8.0, or .NET 10.0

## Installation

### Package Manager Console

```powershell
Install-Package Wolfgang.TryPattern
```

### .NET CLI

```bash
dotnet add package Wolfgang.TryPattern
```

### PackageReference

```xml
<PackageReference Include="Wolfgang.TryPattern" Version="0.3.1" />
```

## Basic Usage

### Running an Action

Use `Try.Run` to execute an action that might throw:

```csharp
using Wolfgang.TryPattern;

var result = Try.Run(() => File.Delete(tempPath));

if (result.Failed)
{
    Console.WriteLine($"Could not delete file: {result.ErrorMessage}");
}
```

### Running a Function

Use `Try.Run<T>` to execute a function and capture its return value:

```csharp
var result = Try.Run(() => int.Parse(userInput));

if (result.Succeeded)
{
    Console.WriteLine($"Parsed value: {result.Value}");
}
else
{
    Console.WriteLine($"Invalid input: {result.ErrorMessage}");
}
```

### Async Operations

Both `Action` and `Func<Task<T>>` have async counterparts:

```csharp
var result = await Try.RunAsync(
    () => httpClient.GetStringAsync(url)
);

if (result.Succeeded)
{
    ProcessResponse(result.Value);
}
```

Async operations support cancellation:

```csharp
var result = await Try.RunAsync(
    () => httpClient.GetStringAsync(url),
    cancellationToken
);
```

> [!NOTE]
> `OperationCanceledException` is intentionally **not** caught by `Try.RunAsync`. Cancellation propagates normally so callers can distinguish between a failed operation and a cancelled one.

### Composing Results

When you need to run multiple operations and check the overall outcome:

```csharp
var result1 = Try.Run(() => ValidateName(name));
var result2 = Try.Run(() => ValidateEmail(email));
var result3 = Try.Run(() => ValidateAge(age));

// Check if any failed
if (Result.AnyFailed(result1, result2, result3))
{
    Console.WriteLine("Validation errors occurred.");
}

// Or flatten into a single result with combined error messages
var combined = Result.Flatten(result1, result2, result3);

if (combined.Failed)
{
    // ErrorMessage contains all failure messages joined by newlines
    Console.WriteLine(combined.ErrorMessage);
}
```

### Creating Results Directly

You can also create `Result` objects directly for your own methods:

```csharp
public Result ValidateOrder(Order order)
{
    if (order.Items.Count == 0)
    {
        return Result.Failure("Order must contain at least one item.");
    }

    return Result.Success();
}

public Result<decimal> CalculateDiscount(Order order)
{
    if (order.Total < 50)
    {
        return Result<decimal>.Failure("Minimum order for discount is $50.");
    }

    return Result<decimal>.Success(order.Total * 0.10m);
}
```

## Next Steps

- Browse the [API Reference](../api/index.md) for complete method signatures and XML documentation
- View the [source code](https://github.com/Chris-Wolfgang/Try-Pattern) on GitHub
