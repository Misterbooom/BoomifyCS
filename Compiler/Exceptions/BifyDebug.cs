using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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

            string logMessage = $"[DEBUG] [{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n" +
                                $"Called from: {callerName} in {fileName} at line {callerLineNumber}";
            Console.WriteLine(logMessage);
        }
    }
}
