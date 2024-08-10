using System;
using System.IO;
using System.Text.RegularExpressions;
using ColorConsole = Colorful.Console;
using System.Drawing;

namespace BoomifyCS.Parser
{
    public static class ExceptionExtension
    {
        public static void ParseError(string name,string stackTrace,string message)
        {
            var lines = stackTrace.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            ColorConsole.WriteLine($"{name}:{message}",Color.Aqua);
            foreach (var line in lines)
            {
                var parsedLine = ParseLine(line);
                if (parsedLine.HasValue)
                {
                    WriteLineOfStack(parsedLine.Value);
                }
            }
        }

        private static (string Method, string Path, int LineNumber)? ParseLine(string line)
        {
            var regex = new Regex(@"^\s*at\s+(?<method>.*)\s+in\s+(?<path>.*):line\s+(?<lineNumber>\d+)", RegexOptions.IgnoreCase);
            var match = regex.Match(line);

            if (match.Success)
            {
                var method = match.Groups["method"].Value;
                var path = match.Groups["path"].Value;
                var lineNumber = int.Parse(match.Groups["lineNumber"].Value);
                return (method, path, lineNumber);
            }
            return null;
        }

        private static void WriteLineOfStack((string Method, string Path, int LineNumber) parsedLine)
        {
            var (method, path, lineNumber) = parsedLine;
            string codeLine = null;

            try
            {
                // Read the specified line from the file
                codeLine = File.ReadAllLines(path)[lineNumber - 1];
            }
            catch (Exception ex)
            {
                codeLine = $"Error reading file: {ex.Message}";
            }

            // Print stack trace information with red colors
            ColorConsole.WriteLine($"    at {path}:{lineNumber} (line {lineNumber})",Color.Red);
            ColorConsole.WriteLine($"        {method}",Color.DarkRed);

            ColorConsole.WriteLine($"        {codeLine}",Color.IndianRed);
        }
    }
}
