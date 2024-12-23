using System;
using System.Collections.Generic;

namespace BoomifyCS.Exceptions
{
    public class Traceback
    {
        private static Traceback _instance;
        public int line = 0;
        private string fileName = "main";
        public string[] source;
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

        public void InitializeSource(string[] sourceCode)
        {
            source = sourceCode;
        }
        public void Catch(Type type)
        {
            track.Push(type);
        }
        public BifyError GetError()
        {
            if (stack.Count == 0)
            {
                return null;
            }
            return stack.Pop();
        }
        public void SetCurrentLine(int currentLine)
        {
            line = currentLine;
        }

        public void ThrowException(BifyError error, int column = 0)
        {
            if (source != null && source.Length > line - 1)
            {
                error.CurrentLine = line;
                error.FileName = fileName;
                error.LineTokensString = source[line - 1];
                error.Column = column;
                foreach (Type type in track)
                {
                    if (type.IsAssignableFrom(error.GetType()))
                    {
                        stack.Push(error);
                        track.Pop();
                        return;
                    }
                    else
                    {
                        Console.WriteLine($"{error.GetType().Name} != {type.Name}");
                    }
                }


                error.PrintException();
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
                    $"Error: Source code is either uninitialized or contains an invalid line. Length: {source?.Length ?? 0}. Content: \"{sourceContent}\"");


            }
        }
    }
}
