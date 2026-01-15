Imports System
Imports System.IO
Imports System.Diagnostics.CodeAnalysis
Imports Wolfgang.TryPattern


<ExcludeFromCodeCoverage>
Friend Module Program
	Sub Main(args As String())
		Console.ForegroundColor = ConsoleColor.Cyan

		' Try reading a file
		Dim fileReadResult = [Try].Run(Function() File.ReadAllText(".\sample.txt"))

		' If file read failed, print the error message
		If fileReadResult.Succeeded Then
			Console.WriteLine("File read successfully.")
		Else
			Console.WriteLine("An error occurred while attempting to read the file.")
			Console.WriteLine(fileReadResult.ErrorMessage)
			Return
		End If

		' Get the word count from the file content
		Dim wordCountResult = [Try].Run(Function() GetWordCount(fileReadResult.Value))
		If wordCountResult.Succeeded Then
			Console.WriteLine($"The file contains {wordCountResult.Value} words.")
		End If

		' Try counting words in the file content
		wordCountResult = [Try].Run(Function() GetWordCount(Nothing))

		' If word count failed, print the error message
		If wordCountResult.Failed Then
			Console.ForegroundColor = ConsoleColor.Red
			Console.WriteLine("An error occurred while attempting to count words.")
			Console.WriteLine(wordCountResult.ErrorMessage)
		End If

		Console.ResetColor()
	End Sub



	Private Function GetWordCount(content As String) As Integer
		Return content _
			.Split(New Char() {" "c, ControlChars.Lf, ControlChars.Cr}, StringSplitOptions.RemoveEmptyEntries) _
			.Length
	End Function
End Module
