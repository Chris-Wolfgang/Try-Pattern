using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Wolfgang.TryPattern;

namespace CSharp.DotNet462.Example
{
    [ExcludeFromCodeCoverage]
    internal static class Program
    {
        private static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            // Try reading a file
            var fileReadResult = Try.Run(() => File.ReadAllText(@".\sample.txt"));

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

            // Get the word count from the file content
            var wordCountResult = Try.Run(() => GetWordCount(fileReadResult.Value));
            if (wordCountResult.Succeeded)
            {
                Console.WriteLine($"The file contains {wordCountResult.Value} words.");
            }

            // Try counting words in the file content
            wordCountResult = Try.Run(() => GetWordCount(null));

            // If word count failed, print the error message
            if (wordCountResult.Failed)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An error occurred while attempting to count words.");
                Console.WriteLine(wordCountResult.ErrorMessage);
            }
            Console.ResetColor();
        }


        private static int GetWordCount(string content)
        {
            return content
                .Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Length;
        }
    }
}
