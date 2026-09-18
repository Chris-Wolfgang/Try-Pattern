#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Wolfgang.TryPattern.Tests.Unit;

/// <summary>
/// Builds the assertion message the snippet-compilation tests
/// (<see cref="DocExampleCompilationTests"/>, <see cref="ReadmeExampleCompilationTests"/>)
/// fail with: the compiler errors, the offending snippet indented for
/// copy-paste reproduction, and a repo-specific hint on where to fix it.
/// </summary>
internal static class CompileFailureMessage
{
    internal static string Format
    (
        string header,
        IEnumerable<Diagnostic> errors,
        string snippet,
        string hint
    )
    {
        string errorList = string.Join(Environment.NewLine, errors.Select(e => $"  {e}"));
        string reproHint = string.Join(Environment.NewLine, snippet.Split('\n').Select(l => $"    {l.TrimEnd()}"));
        return
            $"{header}{Environment.NewLine}" +
            $"Errors:{Environment.NewLine}{errorList}{Environment.NewLine}" +
            $"Snippet:{Environment.NewLine}{reproHint}{Environment.NewLine}" +
            hint;
    }
}
#endif
