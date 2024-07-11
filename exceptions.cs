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
        public int CurrentLine { get; private set; }
        public string[] CallStack { get; private set; }
        public List<Token> LineTokens { get; private set; }
        public List<Token> InvalidTokens { get; private set; }
        public string FileName { get; private set; }

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
            WriteLineTokens(5);

        }
        public void WriteLineTokens(int indentInt)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < indentInt; i++)
            {
                builder.Append(' ');
            }
            Console.Write(builder.ToString());
            foreach (Token token in LineTokens)
            {
                
                if (InvalidTokens.Contains(token))
                {

                    ColorConsole.Write(token.Value,Color.Red);
                }
                else
                {
                    ColorConsole.Write(token.Value, Color.White);
                }
            }
            Console.Write("\n");

        }

    }

    public class BifySyntaxError : BifyException
    {
        public BifySyntaxError(string message) : base($"BifySyntaxError: {message}") { }
    }

    public class BifyNameError : BifyException
    {
        public BifyNameError(string message) : base($"BifyNameError: {message}") { }
    }

    public class BifyTypeError : BifyException
    {
        public BifyTypeError(string message) : base($"BifyTypeError: {message}") { }
    }

    public class BifyValueError : BifyException
    {
        public BifyValueError(string message) : base($"BifyValueError: {message}") { }
    }

    public class BifyZeroDivisionError : BifyException
    {
        public BifyZeroDivisionError(string message) : base($"BifyZeroDivisionError: {message}") { }
    }

    public class BifyIndexError : BifyException
    {
        public BifyIndexError(string message) : base($"BifyIndexError: {message}") { }
    }

    public class BifyAttributeError : BifyException
    {
        public BifyAttributeError(string message) : base($"BifyAttributeError: {message}") { }
    }

    public class BifyKeyError : BifyException
    {
        public BifyKeyError(string message) : base($"BifyKeyError: {message}") { }
    }

    public class BifyAssertionError : BifyException
    {
        public BifyAssertionError(string message) : base($"BifyAssertionError: {message}") { }
    }

    public class BifyImportError : BifyException
    {
        public BifyImportError(string message) : base($"BifyImportError: {message}") { }
    }

    public class BifyIOError : BifyException
    {
        public BifyIOError(string message) : base($"BifyIOError: {message}") { }
    }

    public class BifyRuntimeError : BifyException
    {
        public BifyRuntimeError(string message) : base($"BifyRuntimeError: {message}") { }
    }

    public class BifyOverflowError : BifyException
    {
        public BifyOverflowError(string message) : base($"BifyOverflowError: {message}") { }
    }


    public class BifyIndentationError : BifyException
    {
        public BifyIndentationError(string message) : base($"BifyIndentationError: {message}") { }
    }

    public class BifyMemoryError : BifyException
    {
        public BifyMemoryError(string message) : base($"BifyMemoryError: {message}") { }
    }

    public class BifyRecursionError : BifyException
    {
        public BifyRecursionError(string message) : base($"BifyRecursionError: {message}") { }
    }

    public class BifyTimeoutError : BifyException
    {
        public BifyTimeoutError(string message) : base($"BifyTimeoutError: {message}") { }
    }

    public class BifyUnboundLocalError : BifyException
    {
        public BifyUnboundLocalError(string message) : base($"BifyUnboundLocalError: {message}") { }
    }

    public class BifyUnknownError : BifyException
    {
        public BifyUnknownError(string message) : base($"BifyUnknownError: {message}") { }
    }
}
