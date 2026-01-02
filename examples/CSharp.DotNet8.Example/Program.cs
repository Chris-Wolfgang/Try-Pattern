using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolfgang.TryPattern;

namespace CSharp.DotNet462.Example
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            // Try reading a file async
            var fileReadResult = await Try.RunAsync(() => File.ReadAllTextAsync(@".\sample.txt"));

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


        static int GetWordCount(string? content)
        {
            return content
                .Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Length;
        }
    }
}
