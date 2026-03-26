# Setup Guide

This guide provides instructions for setting up the Try Pattern library for development and usage.

## For Library Users

### Prerequisites

- .NET SDK (the library supports .NET Framework 4.6.2, .NET Standard 2.0, .NET 8.0, and .NET 10.0)
- Visual Studio 2022, VS Code, or Rider (recommended)

### Quick Setup

1. **Install via NuGet**
   ```bash
   dotnet add package Wolfgang.TryPattern
   ```

2. **Add using statement**
   ```csharp
   using Wolfgang.TryPattern;
   ```

3. **Start using the library**
   ```csharp
   var result = await Try.RunAsync(() => SomeOperationAsync());
   ```

## For Contributors

### Prerequisites

- .NET 8.0 SDK or later (for building the library which targets .NET 4.6.2 through .NET 10.0)
- Git
- Visual Studio 2022, VS Code, or Rider

### Clone the Repository

```bash
git clone https://github.com/Chris-Wolfgang/Try-Pattern.git
cd Try-Pattern
```

### Build the Solution

```bash
dotnet restore
dotnet build
```

### Run Tests

```bash
dotnet test
```

### Project Structure

```
Try-Pattern/
├── src/
│   └── Wolfgang.TryPattern/        # Main library project
├── tests/
│   └── Wolfgang.TryPattern.Tests/  # Unit tests
├── examples/
│   ├── CSharp.DotNet8.Example/     # C# .NET 8 examples
│   ├── CSharp.DotNet462.Example/   # C# .NET Framework examples
│   ├── FSharp.DotNet8.Example/     # F# .NET 8 examples
│   ├── FSharp.DotNet462.Example/   # F# .NET Framework examples
│   ├── VB.DotNet8.Example/         # VB.NET .NET 8 examples
│   └── VB.DotNet462.Example/       # VB.NET .NET Framework examples
├── benchmarks/                      # Performance benchmarks
└── docs/                            # Documentation files
```

## Development Workflow

### 1. Create a Feature Branch

```bash
git checkout -b feature/your-feature-name
```

### 2. Make Changes

Edit the code, add tests, and ensure all tests pass.

### 3. Run Quality Checks

```bash
# Run tests
dotnet test

# Build the solution
dotnet build

# Run benchmarks (optional)
cd benchmarks
dotnet run -c Release
```

### 4. Commit and Push

```bash
git add .
git commit -m "Add: your feature description"
git push origin feature/your-feature-name
```

### 5. Create Pull Request

Open a pull request on GitHub and wait for review.

## Building Documentation

The project uses DocFX for documentation generation.

```bash
cd docfx_project
docfx build
docfx serve
```

Visit `http://localhost:8080` to view the documentation.

## Running Examples

Navigate to any example project and run:

```bash
cd examples/CSharp.DotNet8.Example
dotnet run
```

## Troubleshooting

### Build Errors

**Issue**: Missing .NET SDK version
```
Solution: Install the required .NET SDK from https://dotnet.microsoft.com/download
```

**Issue**: NuGet package restore fails
```
Solution: Clear NuGet cache and restore
dotnet nuget locals all --clear
dotnet restore
```

### Test Failures

**Issue**: Tests fail on first run
```
Solution: Ensure all dependencies are restored
dotnet restore
dotnet build
dotnet test
```

## IDE Setup

### Visual Studio 2022

1. Open `TryPattern.sln`
2. Restore NuGet packages (automatic)
3. Build solution (Ctrl+Shift+B)
4. Run tests (Ctrl+R, A)

### VS Code

1. Open folder in VS Code
2. Install C# extension
3. Use integrated terminal for commands:
   ```bash
   dotnet build
   dotnet test
   ```

### Rider

1. Open `TryPattern.sln`
2. Restore NuGet packages (automatic)
3. Build solution
4. Run tests from Test Explorer

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- **Issues**: Report bugs or request features on [GitHub Issues](https://github.com/Chris-Wolfgang/Try-Pattern/issues)
- **Discussions**: Ask questions in [GitHub Discussions](https://github.com/Chris-Wolfgang/Try-Pattern/discussions)
- **Documentation**: Check the [docs](docs/) folder for detailed documentation
