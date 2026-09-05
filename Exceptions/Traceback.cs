using System;
using System.Collections.Generic;
using System.Linq;
namespace BoomifyCS.Exceptions
{
    public class Traceback
    {
        private static Traceback _instance;
        public int Line = 0;
        public string FilePath = "main";
        public string[] Source;
        public List<CallStackFrame> CallStack;
        private readonly Stack<BifyError> _stack;
        private readonly Stack<Type> _track;
        private Traceback() { _stack = []; _track = []; }
        public static Traceback Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Traceback();
                }
                return _instance;
            }
        }

        public void InitializeSource(string[] sourceCode) => Source = sourceCode;
        public void Catch(Type type) => _track.Push(type);
        public BifyError GetError()
        {
            if (_stack.Count == 0)
            {
                return null;
            }
            return _stack.Pop();
        }
        public void TrackPop() => _track.Pop();
        public void SetCurrentLine(int currentLine) => Line = currentLine;

        public void ThrowException(BifyError error, int column = 0)
        {

            if (Source != null && Source.Length > Line - 1)
            {
                error.CurrentLine = Math.Clamp(Line - 1, 0, Source.Length - 1);
                error.FileName = FilePath;
                error.LineTokensString = Source[Math.Clamp(Line - 1, 0, Source.Length - 1)];
                error.Column = column;
                error.CallStack = CallStack;
                if (_track.Any(type => type == error.GetType()))
                {
                    _stack.Push(error);
                    return;
                }


                error.PrintException();
                throw new ApplicationException();
                Environment.Exit(-1);
            }
            else
            {
                if (Source == null) return;
                var sourceContent = Source.Aggregate("", (current, line) => current + line);
                throw new MissingMemberException(
                    $"Error: Source code is either uninitialized or contains an invalid Line. Length: {Source?.Length ?? 0}. Content: \"{sourceContent}\"");
            }
        }
    }
}