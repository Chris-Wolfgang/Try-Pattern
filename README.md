# Wolfgang.TryPattern

[![NuGet](https://img.shields.io/nuget/v/Wolfgang.TryPattern.svg?logo=nuget&label=NuGet)](https://www.nuget.org/packages/Wolfgang.TryPattern)
[![NuGet downloads](https://img.shields.io/nuget/dt/Wolfgang.TryPattern.svg?logo=nuget&label=downloads)](https://www.nuget.org/packages/Wolfgang.TryPattern)
[![PR build](https://img.shields.io/github/actions/workflow/status/Chris-Wolfgang/Try-Pattern/pr.yaml?branch=main&label=PR%20build&logo=github)](https://github.com/Chris-Wolfgang/Try-Pattern/actions/workflows/pr.yaml)
[![Release](https://img.shields.io/github/actions/workflow/status/Chris-Wolfgang/Try-Pattern/release.yaml?label=release&logo=github)](https://github.com/Chris-Wolfgang/Try-Pattern/actions/workflows/release.yaml)
[![OSSF Scorecard](https://api.scorecard.dev/projects/github.com/Chris-Wolfgang/Try-Pattern/badge)](https://scorecard.dev/viewer/?uri=github.com/Chris-Wolfgang/Try-Pattern)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-Multi--Targeted-purple.svg)](https://dotnet.microsoft.com/)
[![GitHub](https://img.shields.io/badge/GitHub-Repository-181717?logo=github)](https://github.com/Chris-Wolfgang/Try-Pattern)

A lightweight .NET library that provides a Try/Result pattern for executing actions and functions with automatic exception handling. Instead of try/catch blocks scattered throughout your code, wrap operations in `Try.Run()` and get back a `Result` indicating success or failure.

---

## 📦 Installation

### Via .NET CLI

```bash
dotnet add package Wolfgang.TryPattern
```

### Via Package Manager Console

```powershell
Install-Package Wolfgang.TryPattern
```

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## 📚 Documentation

- **GitHub Repository:** [https://github.com/Chris-Wolfgang/Try-Pattern](https://github.com/Chris-Wolfgang/Try-Pattern)
- **API Documentation:** [https://chris-wolfgang.github.io/Try-Pattern/](https://chris-wolfgang.github.io/Try-Pattern/)
- **API Reference:** [https://chris-wolfgang.github.io/Try-Pattern/api/](https://chris-wolfgang.github.io/Try-Pattern/api/)
- **Contributing Guide:** [CONTRIBUTING.md](CONTRIBUTING.md)
- **Changelog:** [CHANGELOG.md](CHANGELOG.md)

---

## 🚀 Quick Start

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

`Try.RunAsync(Action, CancellationToken)` runs a synchronous action on the thread pool via `Task.Run`. The token cancels the task before it starts; once running, cancellation is cooperative (your code must check the token). `OperationCanceledException` is always rethrown, never captured as a `Result`.

```csharp
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

var result = await Try.RunAsync(() =>
{
    foreach (var item in largeDataSet)
    {
        cts.Token.ThrowIfCancellationRequested();
        Process(item);
    }
}, cts.Token);
```

---

## ✨ Features

| Feature | Description |
|---------|-------------|
| `Try.Run(Action)` | Execute an action, return `Result` |
| `Try.Run<T>(Func<T>)` | Execute a function, return `Result<T>` with the value |
| `Try.RunAsync(Action, CancellationToken)` | Run a synchronous action on the thread pool with cooperative cancellation |
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

## 🔗 Combining Results

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

## 📖 Real-World Examples

### Database access with Try.RunAsync

Wrap database calls to get a clean `Result` instead of scattered try/catch:

```csharp
public async Task<Result<Customer>> GetCustomerByIdAsync(int id, CancellationToken token = default)
{
    return await Try.RunAsync(async () =>
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(token);
        await using var command = new SqlCommand("SELECT Id, Name, Email FROM Customers WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync(token);
        if (!await reader.ReadAsync(token))
            throw new InvalidOperationException($"Customer {id} not found.");

        return new Customer
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Email = reader.GetString(2)
        };
    }, token);
}

// Usage
var result = await GetCustomerByIdAsync(42);
if (result.Succeeded)
{
    Console.WriteLine($"Found: {result.Value.Name}");
}
else
{
    Console.WriteLine($"Lookup failed: {result.ErrorMessage}");
}
```

### Async query returning a list

```csharp
public async Task<Result<List<Order>>> GetRecentOrdersAsync(int customerId, CancellationToken token = default)
{
    return await Try.RunAsync(async () =>
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(token);
        // ... query and return orders
        return orders;
    }, token);
}
```

### Using Result as a repository return type

`Result` and `Result<T>` work well as return types from repositories and service layers. You don't need `Try.Run()` to create them -- use the static factory methods directly:

```csharp
public class CustomerRepository
{
    public Result<Customer> GetById(int id)
    {
        var customer = dbContext.Customers.Find(id);

        return customer is not null
            ? Result<Customer>.Success(customer)
            : Result<Customer>.Failure($"Customer with ID {id} not found.");
    }

    public Result Save(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.Name))
            return Result.Failure("Customer name is required.");

        if (string.IsNullOrWhiteSpace(customer.Email))
            return Result.Failure("Customer email is required.");

        dbContext.Customers.Update(customer);
        dbContext.SaveChanges();
        return Result.Success();
    }

    public Result Delete(int id)
    {
        var customer = dbContext.Customers.Find(id);
        if (customer is null)
            return Result.Failure($"Customer with ID {id} not found.");

        dbContext.Customers.Remove(customer);
        dbContext.SaveChanges();
        return Result.Success();
    }
}
```

### Using Result in a Web API controller

Return `Result` from your service layer and map it to HTTP responses:

```csharp
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerRepository _repository;

    public CustomersController(CustomerRepository repository) => _repository = repository;

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var result = _repository.GetById(id);

        return result.Succeeded
            ? Ok(result.Value)
            : NotFound(new { error = result.ErrorMessage });
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, CustomerDto dto)
    {
        var lookup = _repository.GetById(id);
        if (lookup.Failed)
            return NotFound(new { error = lookup.ErrorMessage });

        lookup.Value.Name = dto.Name;
        lookup.Value.Email = dto.Email;

        var saveResult = _repository.Save(lookup.Value);

        return saveResult.Succeeded
            ? NoContent()
            : BadRequest(new { error = saveResult.ErrorMessage });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var result = _repository.Delete(id);

        return result.Succeeded
            ? NoContent()
            : NotFound(new { error = result.ErrorMessage });
    }
}
```

### Chaining operations with validation

```csharp
public Result<Order> PlaceOrder(OrderRequest request)
{
    // Validate
    var validation = Result.Flatten(
        ValidateCustomer(request.CustomerId),
        ValidateItems(request.Items),
        ValidatePayment(request.PaymentMethod)
    );

    if (validation.Failed)
        return Result<Order>.Failure(validation.ErrorMessage);

    // Execute
    return Try.Run(() =>
    {
        var order = orderService.Create(request);
        emailService.SendConfirmation(order);
        return order;
    });
}
```

---

## 🎯 Target Frameworks

| Framework | Version |
|-----------|---------|
| .NET Framework | 4.6.2+ |
| .NET Standard | 2.0 |
| .NET | 8.0, 10.0 |

---

## 🔍 Code Quality & Static Analysis

This project enforces **strict code quality standards** through **8 specialized analyzers**, a `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` Release gate, and SourceLink-embedded provenance for debugger step-into.

### Analyzers in Use

1. **Microsoft.CodeAnalysis.NetAnalyzers** — Built-in .NET analyzers for correctness and performance
2. **Roslynator.Analyzers** — Advanced refactoring and code quality rules
3. **AsyncFixer** — Async/await best practices and anti-pattern detection
4. **Microsoft.VisualStudio.Threading.Analyzers** — Thread safety and async patterns
5. **Microsoft.CodeAnalysis.BannedApiAnalyzers** — Prevents usage of banned synchronous APIs (see `BannedSymbols.txt`)
6. **Meziantou.Analyzer** — Comprehensive code quality rules
7. **SonarAnalyzer.CSharp** — Industry-standard code analysis
8. **Microsoft.CodeAnalysis.PublicApiAnalyzers** — Tracks the public API surface via `PublicAPI.Shipped.txt` / `PublicAPI.Unshipped.txt`; surfaces additions/removals at compile time as a breaking-change review gate

---

## 🛠️ Building from Source

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

## 🤝 Contributing

Contributions are welcome — see [CONTRIBUTING.md](CONTRIBUTING.md) for the
development workflow, coding conventions, and PR checklist.
