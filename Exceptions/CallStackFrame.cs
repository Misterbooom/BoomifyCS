namespace BoomifyCS.Exceptions
{
    public class CallStackFrame(string functionName, int lineNumber, string filePath, string codeLine)
    {
        public string FunctionName { get; } = functionName;
        public int LineNumber { get; } = lineNumber;
        public string FilePath { get; } = filePath;
        public string CodeLine { get; } = codeLine;

        public override string ToString() => $"{FunctionName} at {FilePath}:{LineNumber} - {CodeLine}";
    }
}