using System;
using System.IO;

namespace BoomifyCS.Exceptions
{
    class ErrorWrapper
    {
        private readonly Exception _exception;

        public ErrorWrapper(Exception exception) => _exception = exception;

        public void PrintStackTrace()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Traceback (most recent call last):");

            var traceLines = _exception.StackTrace?
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                ?? Array.Empty<string>();

            foreach (var line in traceLines)
            {
                string trimmed = line.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed))
                {
                    string file = ExtractFile(trimmed);
                    string lineNumber = ExtractLineNumber(trimmed);
                    string method = ExtractMethod(trimmed);

                    Console.WriteLine($"  File \"{file}\", line {lineNumber}, in {method}");
                    if (file != "<unknown>" && int.TryParse(lineNumber, out int numLine))
                    {
                        PrintCodeSnippet(file, numLine);
                    }
                }
            }

            Console.WriteLine($"{_exception.GetType().Name}: {_exception.Message}");
            Console.ResetColor();
        }

        private static string ExtractFile(string stackLine)
        {
            int inIndex = stackLine.IndexOf(" in ");
            int lineIndex = stackLine.LastIndexOf(":line");
            return (inIndex >= 0 && lineIndex > inIndex)
                ? stackLine[(inIndex + 4)..lineIndex].Trim()
                : "<unknown>";
        }

        private static string ExtractLineNumber(string stackLine)
        {
            int lineIndex = stackLine.LastIndexOf(":line");
            return (lineIndex >= 0 && stackLine.Length > lineIndex + 6)
                ? stackLine[(lineIndex + 6)..].Trim()
                : "?";
        }

        private static string ExtractMethod(string stackLine)
        {
            int atIndex = stackLine.IndexOf("at ");
            int inIndex = stackLine.IndexOf(" in ");
            return (atIndex >= 0 && inIndex > atIndex)
                ? stackLine[(atIndex + 3)..inIndex].Trim()
                : stackLine.Trim();
        }

        private static void PrintCodeSnippet(string fileName, int errorLine)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine("  (source file not found)");
                return;
            }

            string[] lines = File.ReadAllLines(fileName);
            int targetIndex = errorLine - 1;
            if (targetIndex >= 0 && targetIndex < lines.Length)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"> {errorLine}| {lines[targetIndex]}");
                Console.ResetColor();
            }
        }
    }
}
