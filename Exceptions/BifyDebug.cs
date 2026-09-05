using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace BoomifyCS.Exceptions
{
    class BifyDebug
    {
        public static void Log(
            string message,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            string fileName = Path.GetFileName(callerFilePath);

            string logMessage =
                $"[{callerName}] [{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}, {fileName}:{callerLineNumber}";
            Console.WriteLine(logMessage);
        }
    }
}