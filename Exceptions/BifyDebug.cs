using System;
using System.IO;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Spectre.Console;

namespace BoomifyCS.Exceptions
{
    internal class BifyDebug
    {
        public static void Log(
            string message,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal("INFO", message, callerName, callerFilePath, callerLineNumber, "white");
        }

        public static void LogWarning(
            string message,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal("WARNING", message, callerName, callerFilePath, callerLineNumber, "yellow");
        }

        public static void LogError(
            string message,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal("ERROR", message, callerName, callerFilePath, callerLineNumber, "red");
        }

        public static void LogException(
            Exception exception,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal("EXCEPTION", exception.ToString(), callerName, callerFilePath, callerLineNumber, "maroon");
        }

        public static void Assert(
            bool condition,
            string message = "Assertion failed.",
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (!condition)
            {
                LogInternal("ASSERT", message, callerName, callerFilePath, callerLineNumber, "magenta");
                throw new AssertionException(message);
            }
        }

        private static void LogInternal(
            string level,
            string? message,
            string? callerName,
            string callerFilePath,
            int callerLineNumber,
            string color)
        {
            string fileName = Path.GetFileName(callerFilePath);
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            callerName = Markup.Escape(callerName ?? string.Empty);
            message = Markup.Escape(message ?? string.Empty);
            fileName = Markup.Escape(fileName ?? string.Empty);

            string logMessage = 
                "[[[grey]" + callerName + "[/]]] " +
                "[[[" + color + "]" + level + "[/]]] " +
                "[[" + time + "]] " + 
                message + " - [dim]" + fileName + ":" + callerLineNumber + "[/]";

            AnsiConsole.MarkupLine(logMessage);
        }
    }
}
