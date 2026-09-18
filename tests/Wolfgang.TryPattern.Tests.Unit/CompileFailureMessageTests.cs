#if NET8_0_OR_GREATER
using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Wolfgang.TryPattern.Tests.Unit;

public class CompileFailureMessageTests
{
    [Fact]
    public void Format_lists_every_error_then_the_indented_snippet_then_the_hint()
    {
        const string snippet = "class C\n{\n    void M() { int x = ; }   \n}";
        var errors = CSharpSyntaxTree.ParseText(snippet)
            .GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();
        Assert.NotEmpty(errors);

        string message = CompileFailureMessage.Format("Fence #3 failed to compile.", errors, snippet, "Fix the fence.");

        string[] lines = message.Split(Environment.NewLine);
        Assert.Equal("Fence #3 failed to compile.", lines[0]);
        Assert.Equal("Errors:", lines[1]);
        Assert.All(errors, e => Assert.Contains($"  {e}", lines));
        Assert.Contains("Snippet:", lines);
        Assert.Contains("        void M() { int x = ; }", lines);   // 4-space indent added, trailing whitespace trimmed
        Assert.Equal("Fix the fence.", lines[^1]);
    }
}
#endif
