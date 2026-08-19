#if NET8_0_OR_GREATER
// Doc-example rot detection per issue #179.
//
// Every `<example><code>...</code></example>` block in the public XML
// docs is extracted from Wolfgang.TryPattern.xml (produced by
// <GenerateDocumentationFile>True</...> at build time) and compiled
// with Roslyn against the same references the tests project already
// has loaded. A compilation error fails the test with the failing
// snippet + errors in the message.
//
// This TFM-guards to net8+: Roslyn's compiler-as-a-library requires
// modern .NET, and the doc examples are TFM-invariant so verifying
// once on the modern slice is enough.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Wolfgang.TryPattern.Tests.Unit;

public class DocExampleCompilationTests
{
    // Standard usings prepended to every extracted snippet. Snippets
    // that need additional namespaces should either import them
    // inline (`using SomeNamespace;` inside the `<code>` block itself)
    // or use fully-qualified type names.
    private const string SnippetPreamble = """
        using System;
        using System.Collections.Generic;
        using System.IO;
        using System.Linq;
        using System.Threading;
        using System.Threading.Tasks;
        using Wolfgang.TryPattern;
        """;

    // Type in the library — used for locating the XML doc file and
    // pulling MetadataReference for the compilation.
    private static readonly Assembly LibraryAssembly = typeof(Try).Assembly;

    public static IEnumerable<object[]> AllDocExamples()
    {
        string xmlPath = Path.ChangeExtension(LibraryAssembly.Location, ".xml");
        if (!File.Exists(xmlPath))
        {
            // Missing XML doc: repo-level GenerateDocumentationFile is required.
            // Test harness will surface this as "no examples found".
            yield break;
        }

        XDocument doc = XDocument.Load(xmlPath);
        int index = 0;
        foreach (XElement member in doc.Descendants("member"))
        {
            string memberName = member.Attribute("name")?.Value ?? "?";
            foreach (XElement example in member.Descendants("example"))
            {
                foreach (XElement code in example.Descendants("code"))
                {
                    string snippet = code.Value;
                    if (string.IsNullOrWhiteSpace(snippet))
                    {
                        continue;
                    }

                    yield return new object[] { memberName, index++, snippet };
                }
            }
        }
    }

    [Theory]
    [MemberData(nameof(AllDocExamples))]
    public void Doc_example_compiles(string memberName, int index, string snippet)
    {
        ArgumentNullException.ThrowIfNull(memberName);
        ArgumentNullException.ThrowIfNull(snippet);

        // Wrap the snippet in a method body so free-standing
        // statements (e.g. `var r = Try.Run(...);`) are valid. If
        // the snippet declares a class or top-level members it will
        // fail this wrapping — that IS the failure mode we want, and
        // authors can restructure the snippet to be a method body.
        string source = $$"""
            {{SnippetPreamble}}

            public class DocExample_{{index}}
            {
                // async Task so `await ...` inside snippets compiles;
                // synchronous snippets ignore the async without warning.
                // Extra parameters — httpClient / connectionString /
                // largeDataSet — are stub references the async / DB
                // / collection examples in the XML docs commonly use.
                public async System.Threading.Tasks.Task Run(
                    System.Net.Http.HttpClient httpClient,
                    string connectionString,
                    System.Collections.Generic.IEnumerable<int> largeDataSet)
                {
                    await System.Threading.Tasks.Task.CompletedTask;
                    {{snippet}}
                }

                private static void Process(int item) { }
            }
            """;

        SyntaxTree tree = CSharpSyntaxTree.ParseText(source);

        // The runtime's trusted-platform-assemblies list is the
        // canonical enumeration of every reference assembly available
        // to loaded code — includes System.Console, System.Linq,
        // System.Runtime, System.Net.Http, etc., even if the test
        // itself hasn't touched them. Using AppDomain.GetAssemblies()
        // instead only surfaces already-loaded assemblies, which
        // misses ones like System.Console that the test process
        // hasn't happened to trigger.
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
        // Also add Wolfgang.TryPattern.dll explicitly — TPA covers
        // framework assemblies; the library-under-test lives next to
        // the test.dll.
        references.Add(MetadataReference.CreateFromFile(LibraryAssembly.Location));

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: $"DocExample_{index}",
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
                $"XML doc example on `{memberName}` (example index {index}) failed to compile.{Environment.NewLine}" +
                $"Errors:{Environment.NewLine}{errorList}{Environment.NewLine}" +
                $"Snippet:{Environment.NewLine}{reproHint}{Environment.NewLine}" +
                $"Fix the snippet in the source XML doc, or if it needs an additional using, add it inline in the `<code>` block.");
        }
    }
}

#endif
