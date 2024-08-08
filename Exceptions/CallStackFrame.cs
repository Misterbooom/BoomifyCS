using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomifyCS.Exceptions
{
    public class CallStackFrame(string functionName, int lineNumber, string filePath, string codeLine)
    {
        public string FunctionName { get; } = functionName;
        public int LineNumber { get; } = lineNumber;
        public string FilePath { get; } = filePath;
        public string CodeLine { get; } = codeLine;

        public override string ToString()
        {
            return $"{FunctionName} at {FilePath}:{LineNumber} - {CodeLine}";
        }
    }
}
