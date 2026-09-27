#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wolfgang.TryPattern.Tests.Unit;

/// <summary>
/// Keeps the Native AOT smoke app (tests/Wolfgang.TryPattern.AotSmoke) in step
/// with the public API: every member listed in the PublicAPI baseline files must
/// have at least one <c>Check("&lt;member&gt;", ...)</c> in the smoke app, so a new
/// public member cannot ship without being exercised under Native AOT.
/// </summary>
/// <remarks>
/// Plain string parsing on purpose: [GeneratedRegex] emits source-generated code
/// into this assembly, which the 100% test-assembly coverage gate would count.
/// </remarks>
public class AotSmokeCoverageTests
{
    private const string Namespace = "Wolfgang.TryPattern.";



    [Fact]
    public async Task AotSmoke_when_compared_to_the_PublicAPI_files_checks_every_public_member()
    {
        var repoRoot = LocateRepoRoot();
        var apiFiles = Directory.GetFiles
        (
            Path.Combine(repoRoot, "src", "Wolfgang.TryPattern"),
            "PublicAPI.*.txt",
            SearchOption.AllDirectories
        );
        var smokeSource = await File.ReadAllTextAsync(Path.Combine(repoRoot, "tests", "Wolfgang.TryPattern.AotSmoke", "Program.cs"));

        var publicMembers = new SortedSet<string>(StringComparer.Ordinal);
        foreach (var file in apiFiles)
        {
            publicMembers.UnionWith((await File.ReadAllLinesAsync(file)).SelectMany(MemberKey));
        }

        var checkedMembers = CheckedMembers(smokeSource).ToHashSet(StringComparer.Ordinal);

        // Guards the parser itself: if it stopped recognising the files, an empty
        // set would make the comparison below pass vacuously.
        Assert.Contains("Try.RunAsync<T>", publicMembers);
        Assert.Equal
        (
            Array.Empty<string>(),
            publicMembers.Where(m => !checkedMembers.Contains(m)).ToArray()
        );
    }



    // "static Wolfgang.TryPattern.Result.Failure(string! errorMessage) -> ..." -> "Result.Failure"
    // "Wolfgang.TryPattern.Result<T>.Value.get -> T?"                        -> "Result<T>.Value"
    // Type declarations ("Wolfgang.TryPattern.Result<T>") have neither "(" nor
    // " ->", and the "#nullable enable" header lacks the namespace; both yield nothing.
    private static IEnumerable<string> MemberKey(string line)
    {
        var text = line.StartsWith("static ", StringComparison.Ordinal) ? line["static ".Length..] : line;
        var end = new[] { text.IndexOf('(', StringComparison.Ordinal), text.IndexOf(" ->", StringComparison.Ordinal) }
            .Where(i => i > 0)
            .DefaultIfEmpty(-1)
            .Min();
        return text.StartsWith(Namespace, StringComparison.Ordinal) && end > 0
            ? [text[Namespace.Length..end].Replace(".get", "", StringComparison.Ordinal)]
            : [];
    }



    // The first string argument of every Check("...") and Throws<T>("...") /
    // ThrowsAsync<T>("...") call in the smoke app.
    private static IEnumerable<string> CheckedMembers(string source) =>
        new[] { "Check(\"", ">(\"" }
            .SelectMany(marker => source.Split(marker).Skip(1))
            .Select(rest => rest[..rest.IndexOf('"', StringComparison.Ordinal)]);



    private static string LocateRepoRoot()
    {
        var dir = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(dir, "TryPattern.sln")))
        {
            dir = Path.GetDirectoryName(dir) ?? throw new DirectoryNotFoundException("TryPattern.sln not found above " + AppContext.BaseDirectory);
        }
        return dir;
    }
}
#endif
