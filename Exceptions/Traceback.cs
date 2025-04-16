using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace BoomifyCS.Exceptions
{
    public class Traceback
    {
        private static Traceback _instance;
        public int Line = 0;
        public string FilePath = "main";
        public string[] source;
        public List<CallStackFrame> callStack;
        private Stack<BifyError> stack;
        private Stack<Type> track;
        private Traceback() { stack = []; track = []; }
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

        public void InitializeSource(string[] sourceCode) => source = sourceCode;
        public void Catch(Type type) => track.Push(type);
        public BifyError GetError()
        {
            if (stack.Count == 0)
            {
                return null;
            }
            return stack.Pop();
        }
        public void TrackPop() => track.Pop();
        public void SetCurrentLine(int currentLine) => Line = currentLine;

        public void ThrowException(BifyError error, int column = 0)
        {

            if (source != null && source.Length > Line - 1)
            {
                error.CurrentLine = Math.Clamp(Line - 1, 0, source.Length - 1);
                error.FileName = FilePath;
                error.LineTokensString = source[Math.Clamp(Line - 1, 0, source.Length - 1)];
                error.Column = column;
                error.CallStack = callStack;
                foreach (Type type in track)
                {
                    if (type == error.GetType())
                    {
                        stack.Push(error);
                        return;
                    }
                    else
                    {
                        Console.WriteLine($"{error.GetType().Name.Replace("Bify","")} != {type.Name}");
                    }
                }


                error.PrintException();
                //throw new NotFiniteNumberException();
                Environment.Exit(-1);
            }
            else
            {
                string sourceContent = "";
                foreach (string line in source)
                {
                    sourceContent += line;
                }
                throw new MissingMemberException(
                    $"Error: Source code is either uninitialized or contains an invalid Line. Length: {source?.Length ?? 0}. Content: \"{sourceContent}\"");


            }
        }
    }
}