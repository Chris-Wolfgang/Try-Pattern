#if NET8_0_OR_GREATER
// README code-fence rot detection — companion to DocExampleCompilationTests
// which covers the XML `<example>` blocks. README code fences drift
// silently otherwise; the code-review that surfaced this file caught a
// `Result<Customer>` → `Result<Customer?>` mismatch that had shipped
// twice.
//
// Not every fence is compilable at the test-project boundary — the
// Web API / SqlConnection / EF dbContext examples use frameworks the
// test project deliberately does not reference. Fences containing
// obvious external-framework markers (SqlConnection, [ApiController],
// [HttpGet], dbContext) are SKIPPED with a note; the compilable
// fences (Quick Start / Combining / Cancellation) are what this file
// enforces.
//
// TFM-guarded to net8+ for Roslyn availability — identical guard to
// DocExampleCompilationTests.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Wolfgang.TryPattern.Tests.Unit;

public class ReadmeExampleCompilationTests
{
    private const string SnippetPreamble = """
        using System;
        using System.Collections.Generic;
        using System.IO;
        using System.Linq;
        using System.Threading;
        using System.Threading.Tasks;
        using Wolfgang.TryPattern;
        """;

    // Fences containing any of these substrings need frameworks the
    // test project doesn't reference (ASP.NET Core, SqlClient, EF).
    // Compile-checking them would require dragging those packages
    // into the test csproj — heavy dependency for questionable value.
    // Skip with a note; the intent is to catch rot in fences that
    // exclusively use the Try / Result surface.
    private static readonly string[] SkipMarkers =
    {
        "SqlConnection",
        "SqlCommand",
        "[ApiController]",
        "[HttpGet",
        "[HttpPut",
        "[HttpDelete",
        "dbContext",
        "IActionResult",
        "orderService.",
        "emailService.",
        "httpClient.",
        "ValidateCustomer",
        "ValidateName",
    };

    private static readonly Assembly LibraryAssembly = typeof(Try).Assembly;


    public static IEnumerable<object[]> AllReadmeFences()
    {
        string readmePath = LocateReadme();
        if (!File.Exists(readmePath))
        {
            yield break;
        }

        string content = File.ReadAllText(readmePath);
        // Matches ```csharp\n...content...\n``` — the ``` opener/closer
        // must be at the start of its own line. `Singleline` lets `.`
        // cross newlines; the inner (.*?) is non-greedy to stop at the
        // nearest closing fence.
        var regex = new Regex(@"^```csharp\s*\r?\n(.*?)\r?\n```", RegexOptions.Multiline | RegexOptions.Singleline);
        int index = 0;
        foreach (Match m in regex.Matches(content))
        {
            string snippet = m.Groups[1].Value;
            if (string.IsNullOrWhiteSpace(snippet))
            {
                continue;
            }
            if (SkipMarkers.Any(marker => snippet.Contains(marker, StringComparison.Ordinal)))
            {
                continue;
            }
            // Strip leading `using X;` directives — README fences often
            // start with `using Wolfgang.TryPattern;` for reader
            // clarity, but the SnippetPreamble already includes it and
            // the method-body wrapper below can't contain
            // using-directives (only using-statements with an
            // initializer). Left-over `using` lines are re-added
            // implicitly by the preamble's imports.
            snippet = Regex.Replace(snippet, @"^\s*using\s+[\w.]+\s*;\s*\r?\n", "", RegexOptions.Multiline);
            yield return new object[] { index++, snippet };
        }
    }


    [Theory]
    [MemberData(nameof(AllReadmeFences))]
    public void Readme_fence_compiles(int index, string snippet)
    {
        if (snippet is null)
        {
            throw new ArgumentNullException(nameof(snippet));
        }

        // Wrap in an async method body so `await` / free-standing
        // statements both compile. Async is superset — synchronous
        // snippets ignore it.
        string source = $$"""
            {{SnippetPreamble}}

            public class ReadmeFence_{{index}}
            {
                public async System.Threading.Tasks.Task Run(
                    System.Collections.Generic.IEnumerable<int> largeDataSet)
                {
                    await System.Threading.Tasks.Task.CompletedTask;
                    {{snippet}}
                }

                private static void Process(int item) { }
            }
            """;

        SyntaxTree tree = CSharpSyntaxTree.ParseText(source);

        List<MetadataReference> references = new();
        string? tpa = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
        if (!string.IsNullOrEmpty(tpa))
        {
            foreach (string path in tpa.Split(Path.PathSeparator))
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    references.Add(MetadataReference.CreateFromFile(path));
                }
            }
        }
        references.Add(MetadataReference.CreateFromFile(LibraryAssembly.Location));

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: $"ReadmeFence_{index}",
            syntaxTrees: new[] { tree },
            references: references,
            options: new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: NullableContextOptions.Enable));

        List<Diagnostic> errors = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        if (errors.Count > 0)
        {
            string errorList = string.Join(Environment.NewLine, errors.Select(e => $"  {e}"));
            string reproHint = string.Join(Environment.NewLine, snippet.Split('\n').Select(l => $"    {l.TrimEnd()}"));
            Assert.Fail(
                $"README fence #{index} failed to compile.{Environment.NewLine}" +
                $"Errors:{Environment.NewLine}{errorList}{Environment.NewLine}" +
                $"Snippet:{Environment.NewLine}{reproHint}{Environment.NewLine}" +
                $"Fix the fence in README.md, or if it uses framework types the test project doesn't reference (SqlConnection, ASP.NET Core, EF dbContext), add a marker string to ReadmeExampleCompilationTests.SkipMarkers.");
        }
    }


    private static string LocateReadme()
    {
        // Walk upward from the test binary's directory until we find
        // README.md. On CI the test runs from bin/Release/net10.0/,
        // so the repo root is a few levels up.
        string? dir = AppContext.BaseDirectory;
        for (int depth = 0; depth < 10 && dir is not null; depth++)
        {
            string candidate = Path.Combine(dir, "README.md");
            if (File.Exists(candidate))
            {
                return candidate;
            }
            dir = Path.GetDirectoryName(dir);
        }
        return "README.md";  // last-ditch, caller checks Exists
    }
}

#endif
