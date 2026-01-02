using System.Diagnostics.CodeAnalysis;

namespace Wolfgang.TryPattern.Tests;

[ExcludeFromCodeCoverage]
internal class Person
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}