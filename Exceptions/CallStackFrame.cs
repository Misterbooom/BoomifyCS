using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomifyCS.Exceptions
{
    public class CallStackFrame
    {
        public string FunctionName { get; }
        public int LineNumber { get; }
        public string FilePath { get; }
        public string CodeLine { get; }

        public CallStackFrame(string functionName, int lineNumber, string filePath, string codeLine)
        {
            FunctionName = functionName;
            LineNumber = lineNumber;
            FilePath = filePath;
            CodeLine = codeLine;
        }

        public override string ToString()
        {
            return $"{FunctionName} at {FilePath}:{LineNumber} - {CodeLine}";
        }
    }
}
