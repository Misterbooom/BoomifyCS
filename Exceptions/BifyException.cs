using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Lexer;
using ColorConsole = Colorful.Console;

namespace BoomifyCS.Exceptions
{
    public abstract class BifyError : Exception
    {
        public int CurrentLine { get; set; }
        public List<CallStackFrame> CallStack { get; set; } = [];
        public string LineTokensString { get; set; } = "";
        public string InvalidTokensString { get; set; }
        public string FileName { get; set; }

        protected BifyError() : base() => FileName = "0";

        protected BifyError(string message) : base(message) => FileName = "0";



        protected BifyError(string message, string tokens, string invalidTokens, int currentLine = 1) : base(message)
        {
            InvalidTokensString = invalidTokens;
            LineTokensString = tokens;
            CurrentLine = currentLine;
        }

        public void PrintException()
        {
            string exceptionInfo = $"{this.GetType().Name}: {Message}";
            string fileInfo = $"    File '{FileName}', (line {CurrentLine})";

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

            foreach (char token in LineTokensString)
            {
                if (InvalidTokensString.Contains(token.ToString()))
                {
                    ColorConsole.Write(token, Color.Red);
                }
                else
                {
                    ColorConsole.Write(token, Color.White);
                }
            }
            

            Console.Write("\n");
        }

       
    }


}
