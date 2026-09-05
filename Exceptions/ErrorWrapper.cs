using System;
using System.IO;

namespace BoomifyCS.Exceptions
{
    public class SimpleErrorWrapper(Exception exception)
    {
        /// <summary>
        /// Prints the detailed traceback and code snippets using improved colors.
        /// </summary>
        /// <param name="contextLines">Number of code lines before and after the error line to display.</param>
        public void PrintStackTrace(int contextLines = 2)
        {
            // Save the original console color.
            ConsoleColor originalColor = Console.ForegroundColor;

            // Use a bright header color.
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Traceback (most recent call last):");
            Console.WriteLine();

            // Print the exception details.
            PrintException(exception, contextLines);

            // Restore the console color.
            Console.ForegroundColor = originalColor;
        }

        private void PrintException(Exception ex, int contextLines)
        {
            // Check if the stack trace exists.
            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                string[] lines = ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    string trimmed = line.Trim();
                    // Attempt to parse stack trace lines that have a file and line indicator.
                    if (trimmed.Contains(" in ") && trimmed.Contains(":line "))
                    {
                        int inIndex = trimmed.IndexOf(" in ");
                        int lineIndex = trimmed.IndexOf(":line ");
                        string methodInfo = trimmed.Substring(0, inIndex);
                        string filePath = trimmed.Substring(inIndex + 4, lineIndex - (inIndex + 4));
                        string lineNumberStr = trimmed.Substring(lineIndex + 6).Trim();

                        // Print file, line, and method info using improved colors.
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("  File \"");
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write(filePath);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("\", line ");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write(lineNumberStr);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(", in ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(methodInfo);

                        // Display code snippet if possible.
                        if (File.Exists(filePath) && int.TryParse(lineNumberStr, out int lineNumber))
                        {
                            PrintCodeSnippet(filePath, lineNumber, contextLines);
                        }
                    }
                    else
                    {
                        // Fallback for lines that do not contain file and line info.
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("  " + trimmed);
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("  (No stack trace available)");
            }

            // Print the exception type and message.
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            Console.WriteLine();

            // Recursively print inner exception details if they exist.
            if (ex.InnerException != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Inner Exception:");
                PrintException(ex.InnerException, contextLines);
            }
        }

        /// <summary>
        /// Prints a snippet of the source code around the given error line.
        /// </summary>
        /// <param name="filePath">The path of the source file.</param>
        /// <param name="errorLine">The error line number in the file.</param>
        /// <param name="contextLines">Number of context lines before and after the error line.</param>
        private void PrintCodeSnippet(string filePath, int errorLine, int contextLines)
        {
            try
            {
                string[] sourceLines = File.ReadAllLines(filePath);
                int targetIndex = errorLine - 1;
                int startIndex = Math.Max(0, targetIndex - contextLines);
                int endIndex = Math.Min(sourceLines.Length - 1, targetIndex + contextLines);

                for (int i = startIndex; i <= endIndex; i++)
                {
                    // Highlight the error line.
                    if (i == targetIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("> ");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("  ");
                    }
                    Console.WriteLine($"{i + 1,4}: {sourceLines[i]}");
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("  (Could not read source file: " + ex.Message + ")");
            }
        }
    }
}
