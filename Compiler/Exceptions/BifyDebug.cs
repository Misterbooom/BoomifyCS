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
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string fileName = Path.GetFileName(callerFilePath);

            string logMessage = $"\n[DEBUG LOG] \n" +
                                $"Timestamp   : {timestamp}\n" +
                                $"Message     : {message}\n" +
                                $"Caller      : {callerName}\n" +
                                $"File        : {fileName}\n" +
                                $"Line        : {callerLineNumber}\n";

            Console.WriteLine(logMessage);
        }
    }
}
