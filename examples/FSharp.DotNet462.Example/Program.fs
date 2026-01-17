open System
open System.IO
open System.Diagnostics.CodeAnalysis
open Wolfgang.TryPattern


[<ExcludeFromCodeCoverage>]
module Program =
    let private getWordCount (content: string) =
        content.Split([| ' '; '\n'; '\r' |], StringSplitOptions.RemoveEmptyEntries).Length

    [<EntryPoint>]
    let main argv =
        Console.ForegroundColor <- ConsoleColor.Cyan

        let fileReadResult = Try.Run(fun () -> File.ReadAllText(@".\sample.txt"))

        let exitCode =
            if fileReadResult.Succeeded then
                Console.WriteLine("File read successfully.")

                let wordCountResult = Try.Run(fun () -> getWordCount fileReadResult.Value)
                if wordCountResult.Succeeded then
                    Console.WriteLine($"The file contains {wordCountResult.Value} words.")

                let failingWordCount = Try.Run(fun () -> getWordCount null)
                if failingWordCount.Failed then
                    Console.ForegroundColor <- ConsoleColor.Red
                    Console.WriteLine("An error occurred while attempting to count words.")
                    Console.WriteLine(failingWordCount.ErrorMessage)

                0
            else
                Console.WriteLine("An error occurred while attempting to read the file.")
                Console.WriteLine(fileReadResult.ErrorMessage)
                0

        Console.ResetColor()
        exitCode
