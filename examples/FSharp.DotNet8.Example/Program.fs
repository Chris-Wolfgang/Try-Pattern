module FSharp.DotNet462.Example

open System
open System.IO
open System.Threading.Tasks
open System.Diagnostics.CodeAnalysis
open Wolfgang.TryPattern

[<ExcludeFromCodeCoverage>]
let private getWordCount (content: string) =
    content
        .Split([| ' '; '\n'; '\r' |], StringSplitOptions.RemoveEmptyEntries)
        .Length

let private mainAsync (args: string array) =
    task {
        Console.ForegroundColor <- ConsoleColor.Cyan

        let! fileReadResult =
            Try.RunAsync(fun () -> File.ReadAllTextAsync(@".\sample.txt"))

        if fileReadResult.Succeeded then
            Console.WriteLine("File read successfully.")
        else
            Console.WriteLine("An error occurred while attempting to read the file.")
            Console.WriteLine(fileReadResult.ErrorMessage)
            Console.ResetColor()
            return ()

        let successfulWordCount =
            Try.Run(fun () -> getWordCount fileReadResult.Value)

        if successfulWordCount.Succeeded then
            Console.WriteLine($"The file contains {successfulWordCount.Value} words.")

        let failingWordCount =
            Try.Run(fun () -> getWordCount null)

        if failingWordCount.Failed then
            Console.ForegroundColor <- ConsoleColor.Red
            Console.WriteLine("An error occurred while attempting to count words.")
            Console.WriteLine(failingWordCount.ErrorMessage)

        Console.ResetColor()
    }


[<EntryPoint>]
let main args =
    mainAsync args
    |> Async.AwaitTask
    |> Async.RunSynchronously
    0
