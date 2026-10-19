using System.Diagnostics.CodeAnalysis;
using Wolfgang.TryPattern;

namespace CSharp.DotNet8.Example;

[ExcludeFromCodeCoverage]
internal static class Program
{
    private static async Task Main()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;

        // Try reading a file async
        var fileReadResult = await Try.RunAsync(async () => await File.ReadAllTextAsync(@".\sample.txt"));

        // If file read failed, print the error message
        if (fileReadResult.Succeeded)
        {
            Console.WriteLine("File read successfully.");
        }
        else
        {
            Console.WriteLine("An error occurred while attempting to read the file.");
            Console.WriteLine(fileReadResult.ErrorMessage);
            return;
        }

        // Synchronous work goes through Try.Run; see CountWords.
        CountWords(fileReadResult.Value);
        Console.ResetColor();
    }



    // Try.Run is the synchronous counterpart of Try.RunAsync. It lives in a
    // non-async method so the example shows it the way a caller would use it:
    // for synchronous work, without blocking inside an async method.
    private static void CountWords(string? content)
    {
        // Get the word count from the file content
        var wordCountResult = Try.Run(() => GetWordCount(content));
        if (wordCountResult.Succeeded)
        {
            Console.WriteLine($"The file contains {wordCountResult.Value} words.");
        }

        // Try counting words in the file content
        wordCountResult = Try.Run(() => GetWordCount(content: null));

        // If word count failed, print the error message
        if (wordCountResult.Failed)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("An error occurred while attempting to count words.");
            Console.WriteLine(wordCountResult.ErrorMessage);
        }
    }



    private static int GetWordCount(string? content) =>
        content!
            .Split([' ', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
            .Length;
}