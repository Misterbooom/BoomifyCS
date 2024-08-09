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
    public class BifyError : Exception
    {
        public int CurrentLine { get; set; }
        public List<CallStackFrame> CallStack { get; set; } = [];
        public List<Token> LineTokens { get; set; }
        public string LineTokensString { get; set; }
        public List<Token> InvalidTokens { get; set; }
        public string InvalidTokensString { get; set; }
        public string FileName { get; set; }

        public BifyError() : base() => FileName = "0";

        public BifyError(string message) : base(message) => FileName = "0";

        public BifyError(string message, Exception innerException) : base(message, innerException) => FileName = "0";

        public BifyError(string message, int currentLine, string[] callStack, List<Token> lineTokens, List<Token> invalidTokens, string fileName = "0")
            : base(message)
        {
            CurrentLine = currentLine;
            LineTokens = lineTokens;
            InvalidTokens = invalidTokens;
            FileName = fileName;
            CallStack = callStack.Select(c => new CallStackFrame(c, currentLine, fileName, LineTokensString)).ToList();
        }

        public BifyError(string message, Exception innerException, int currentLine, string[] callStack, List<Token> lineTokens, List<Token> invalidTokens, string fileName = "0")
            : base(message, innerException)
        {
            CurrentLine = currentLine;
            LineTokens = lineTokens;
            InvalidTokens = invalidTokens;
            FileName = fileName;
            CallStack = callStack.Select(c => new CallStackFrame(c, currentLine, fileName, LineTokensString)).ToList();
        }

        public BifyError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message)
        {
            InvalidTokens = invalidTokens;
            LineTokens = tokens;
            CurrentLine = currentLine;
        }

        public BifyError(string message, string tokens, string invalidTokens, int currentLine = 1) : base(message)
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

            WriteLineTokens(LineTokens, 10);


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



        public void WriteLineTokens(List<Token> tokens, int indentInt)
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

        private static void WriteToken(Token token, Color color, int indentInt = 0)
        {
            if (token.Type == TokenType.OBJECT)
            {
                string[] lines = token.Value.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            }
            else if (token.Type == TokenType.NEXTLINE)
            {
                Console.Write($"{new String(' ', indentInt)}\n");
            }
            else
            {
                ColorConsole.Write(token.Value, color);
            }
        }
    }


}
