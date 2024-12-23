using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;
using ColorConsole = Colorful.Console;

namespace BoomifyCS.Exceptions
{
    public abstract class BifyError : BifyObject 
    {
        public int CurrentLine { get; set; }
        public int Column { get; set; } // Added property for column tracking
        public List<CallStackFrame> CallStack { get; set; } = new();
        public string LineTokensString { get; set; } = "";
        public string InvalidTokensString { get; set; }
        public string FileName { get; set; }
        public string Message;

        protected BifyError() : base() => FileName = "0";


        protected BifyError(string message, string tokens, string invalidTokens, int currentLine = 1, int column = 0)
        {
            InvalidTokensString = invalidTokens;
            LineTokensString = tokens;
            CurrentLine = currentLine;
            Column = column;
            Message = message;
        }

        public void PrintException()
        {
            string exceptionInfo = $"{this.GetType().Name}: {Message}";
            string fileInfo = $"    File '{FileName}', line {CurrentLine}, column {Column}";

            ColorConsole.WriteLine(exceptionInfo, Color.IndianRed);
            ColorConsole.WriteLine(fileInfo, Color.OrangeRed);

            WriteLineTokens(10);
            PrintCallStack();
        }

        private void PrintCallStack()
        {
            if (CallStack != null && CallStack.Count != 0)
            {
                foreach (var frame in CallStack)
                {
                    Console.Write(new string(' ', 4));
                    ColorConsole.WriteLine(new string('-', 35), Color.Gray);

                    ColorConsole.Write($"    at ", Color.Red);
                    ColorConsole.Write(frame.FilePath, Color.IndianRed);
                    ColorConsole.Write($": ", Color.Red);
                    ColorConsole.Write(frame.FunctionName, Color.Red);
                    ColorConsole.Write($" (line ", Color.Red);
                    ColorConsole.Write(frame.LineNumber.ToString(), Color.Red);
                    ColorConsole.WriteLine(")", Color.Red);

                    ColorConsole.WriteLine($"        {frame.CodeLine}", Color.Red);
                }

                Console.Write(new string(' ', 4));
                ColorConsole.WriteLine(new string('-', 35), Color.Gray);
            }
        }

        public void WriteLineTokens(int indentInt)
        {
            StringBuilder builder = new();
            for (int i = 0; i < indentInt; i++)
            {
                builder.Append(' ');
            }

            Console.Write(builder.ToString());

            for (int i = 0; i < LineTokensString.Length; i++)
            {
                if (i == Column - 1)
                {
                    ColorConsole.Write(LineTokensString.Substring(i , InvalidTokensString.Length), Color.Red);

                    i += InvalidTokensString.Length - 1;
                }
                else
                {
                    ColorConsole.Write(LineTokensString[i], Color.White);
                }
            }


            Console.Write("\n");
        }
    }
}
