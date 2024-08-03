using System;
using System.Collections.Generic;
using System.Text;
using BoomifyCS.Lexer;
using System.Drawing;
using ColorConsole = Colorful.Console;


namespace BoomifyCS
{
    public class BifyException : Exception
    {
        public int CurrentLine;
        public string[] CallStack { get; set; }
        public List<Token> LineTokens { get; set; }
        public string LineTokensString { get; set; }
        public List<Token> InvalidTokens { get; set; }
        public string InvalidTokensString { get; set;}
        public string FileName { get; set; }

        public BifyException() : base()
        {
            FileName = "0";
        }

        public BifyException(string message) : base(message)
        {
            FileName = "0";
        }

        public BifyException(string message, Exception innerException) : base(message, innerException)
        {
            FileName = "0";
        }

        public BifyException(string message, int currentLine, string[] callStack, List<Token> lineTokens, List<Token> invalidTokens, string fileName = "0")
            : base(message)
        {
            CurrentLine = currentLine;
            CallStack = callStack;
            LineTokens = lineTokens;
            InvalidTokens = invalidTokens;
            FileName = fileName;
        }

        public BifyException(string message, Exception innerException, int currentLine, string[] callStack, List<Token> lineTokens, List<Token> invalidTokens, string fileName = "0")
            : base(message, innerException)
        {
            CurrentLine = currentLine;
            CallStack = callStack;
            LineTokens = lineTokens;
            InvalidTokens = invalidTokens;
            FileName = fileName;
        }
        public BifyException(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) :base(message)
        {
            InvalidTokens = invalidTokens;
            LineTokens = tokens;
            CurrentLine = currentLine;

        }
        public BifyException(string message,string tokens,string invalidTokens, int currentLine = 1) : base(message)
        {
            InvalidTokensString = invalidTokens;
            LineTokensString = tokens;
            CurrentLine = currentLine;
        }
        /// <summary>
        /// InterpreterTypeError: unsupported operator '/' for 'Integer' and 'String'
        //File '0', line 1
        //   21 / 2

        /// </summary>
        public void PrintException()
        {
            string exceptionInfo = $"{this.GetType().Name}: {Message}";

            string fileInfo = $"    file '{FileName}' line: {CurrentLine}";

            ColorConsole.WriteLine(exceptionInfo,Color.IndianRed);
            ColorConsole.WriteLine(fileInfo,Color.OrangeRed);
            WriteLineTokens(LineTokens,5);
           

        }
        public void WriteLineTokens(List<Token> tokens,int indentInt)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < indentInt; i++)
            {
                builder.Append(' ');
            }
            Console.Write(builder.ToString());
            if (tokens != null)
            {
                foreach (Token token in tokens)
                {
                    if (token.Type == TokenType.NEXTLINE)
                    {
                        break;
                    }
                    if (InvalidTokens.Contains(token))
                    {
                        WriteToken(token,Color.Red,indentInt);
                    }
                    else
                    {
                        WriteToken(token,Color.White,indentInt);

                    }

                }
            }
            else
            {
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
            }
            Console.Write("\n");

        }
        private void WriteToken(Token token, Color color,int indentInt = 0)
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

    public class BifySyntaxError : BifyException
    {
        public BifySyntaxError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifySyntaxError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { Console.WriteLine("tokens " + tokens); }
    }

    public class BifyNameError : BifyException
    {
        public BifyNameError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyNameError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyTypeError : BifyException
    {
        public BifyTypeError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyTypeError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyValueError : BifyException
    {
        public BifyValueError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyValueError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyZeroDivisionError : BifyException
    {
        public BifyZeroDivisionError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyZeroDivisionError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyIndexError : BifyException
    {
        public BifyIndexError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyIndexError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyAttributeError : BifyException
    {
        public BifyAttributeError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyAttributeError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyKeyError : BifyException
    {
        public BifyKeyError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyKeyError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyAssertionError : BifyException
    {
        public BifyAssertionError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyAssertionError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyImportError : BifyException
    {
        public BifyImportError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyImportError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyIOError : BifyException
    {
        public BifyIOError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyIOError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyRuntimeError : BifyException
    {
        public BifyRuntimeError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyRuntimeError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyOverflowError : BifyException
    {
        public BifyOverflowError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyOverflowError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyIndentationError : BifyException
    {
        public BifyIndentationError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyIndentationError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyMemoryError : BifyException
    {
        public BifyMemoryError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyMemoryError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyRecursionError : BifyException
    {
        public BifyRecursionError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyRecursionError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyTimeoutError : BifyException
    {
        public BifyTimeoutError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyTimeoutError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyUnboundLocalError : BifyException
    {
        public BifyUnboundLocalError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyUnboundLocalError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyUnknownError : BifyException
    {
        public BifyUnknownError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyUnknownError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    // Additional Exceptions
    public class BifyStopIterationError : BifyException
    {
        public BifyStopIterationError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyStopIterationError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyArithmeticError : BifyException
    {
        public BifyArithmeticError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyArithmeticError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyFileNotFoundError : BifyException
    {
        public BifyFileNotFoundError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyFileNotFoundError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyPermissionError : BifyException
    {
        public BifyPermissionError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyPermissionError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyConnectionError : BifyException
    {
        public BifyConnectionError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public

     BifyConnectionError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyTimeoutExpiredError : BifyException
    {
        public BifyTimeoutExpiredError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyTimeoutExpiredError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyBrokenPipeError : BifyException
    {
        public BifyBrokenPipeError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyBrokenPipeError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyModuleNotFoundError : BifyException
    {
        public BifyModuleNotFoundError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyModuleNotFoundError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyEOFError : BifyException
    {
        public BifyEOFError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyEOFError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifySyntaxWarning : BifyException
    {
        public BifySyntaxWarning(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifySyntaxWarning(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyDeprecationWarning : BifyException
    {
        public BifyDeprecationWarning(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyDeprecationWarning(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyResourceWarning : BifyException
    {
        public BifyResourceWarning(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyResourceWarning(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }
    public class BifyOperationError : BifyException
    {
        public BifyOperationError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyOperationError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }
    public class BifyParsingError : BifyException
    {
        public BifyParsingError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyParsingError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }
    public class BifyInitializationError : BifyException
    {
        public BifyInitializationError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyInitializationError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }
    public class BifyUndefinedError : BifyException
    {
        public BifyUndefinedError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyUndefinedError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }
    public class BifyArgumentError : BifyException
    {
        public BifyArgumentError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyArgumentError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }
    public class BifyCastError : BifyException
    {
        public BifyCastError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyCastError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }
}
