using System.Diagnostics.CodeAnalysis;

namespace Wolfgang.TryPattern.Tests.Unit;

[ExcludeFromCodeCoverage]
internal class Person
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}