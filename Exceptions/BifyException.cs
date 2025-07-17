using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using BoomifyCS.Lexer;
using ColorConsole = Colorful.Console;

namespace BoomifyCS.Exceptions
{
    public static class AnsiColorHelper
    {
        public static void WriteLine(string text, Color color)
        {
            Console.Write($"\x1b[38;2;{color.R};{color.G};{color.B}m{text}\x1b[0m\n");
        }

        public static void Write(string text, Color color)
        {
            Console.Write($"\x1b[38;2;{color.R};{color.G};{color.B}m{text}\x1b[0m");
        }
    }

    public abstract class BifyError
    {
        public int CurrentLine { get; set; }
        public int Column { get; set; } 
        public List<CallStackFrame> CallStack { get; set; } = new();
        public string LineTokensString { get; set; } = "";
        public string InvalidTokensString { get; set; }
        public string FileName { get; set; }
        public string Message;

        protected BifyError() : base()
        {
            FileName = "0";
        }

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
            string exceptionInfo = $"{this.GetType().Name.Replace("Bify", "")}: {Message}";
            string fileInfo = $"    File '{FileName}', Line {CurrentLine}, column {Column}";

            AnsiColorHelper.WriteLine(exceptionInfo, Color.IndianRed);
            AnsiColorHelper.WriteLine(fileInfo, Color.OrangeRed);

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
                    AnsiColorHelper.WriteLine(new string('-', 35), Color.Gray);

                    AnsiColorHelper.Write("    at ", Color.Red);
                    AnsiColorHelper.Write(frame.FilePath, Color.IndianRed);
                    AnsiColorHelper.Write(": ", Color.Red);
                    AnsiColorHelper.Write(frame.FunctionName, Color.Red);
                    AnsiColorHelper.Write(" (Line ", Color.Red);
                    AnsiColorHelper.Write(frame.LineNumber.ToString(), Color.Red);
                    AnsiColorHelper.WriteLine(")", Color.Red);

                    AnsiColorHelper.WriteLine($"        {frame.CodeLine}", Color.Red);
                }

                Console.Write(new string(' ', 4));
                AnsiColorHelper.WriteLine(new string('-', 35), Color.Gray);
            }
        }

       

        public T Throw<T>()
        {
            Traceback.Instance.ThrowException(this);
            return default(T);
        }
        public void Throw()
        {
            Traceback.Instance.ThrowException(this);
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
                if (i == Column - 1 && InvalidTokensString != null)
                {
                    AnsiColorHelper.Write(LineTokensString.Substring(i, InvalidTokensString.Length), Color.Red);

                    i += InvalidTokensString.Length - 1;
                }
                else
                {
                    AnsiColorHelper.Write(LineTokensString[i].ToString(), Color.LightGray);
                }
            }

            Console.Write("\n");
        }
    }
}