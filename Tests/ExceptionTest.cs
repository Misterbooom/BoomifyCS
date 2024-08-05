using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Tests
{
    public class ExceptionTest
    {
        public static void CallStackTest()
        {
            BifySyntaxError testException = new BifySyntaxError("Testing callStack", "StartApplication(arg1,arg2)", "StartApplication(arg1,arg2)",1);
            List<CallStackFrame> callStack = new List<CallStackFrame>
            {
                new CallStackFrame("Main", 10, @"C:\Project\Program.cs", "Main(args);"),
                new CallStackFrame("Initialize", 20, @"C:\Project\Program.cs", "Initialize();"),
                new CallStackFrame("LoadConfiguration", 30, @"C:\Project\Config.cs", "LoadConfiguration();"),
                new CallStackFrame("SetupServices", 40, @"C:\Project\Services.cs", "SetupServices();"),
                new CallStackFrame("StartApplication", 50, @"C:\Project\App.cs", "StartApplication();")
            };
            testException.CallStack = callStack;
            testException.PrintException();
        }
    }
}
