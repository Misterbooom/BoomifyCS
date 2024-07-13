﻿using System;
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
        public string LineTokensString { get; private set; }
        public List<Token> InvalidTokens { get; private set; }
        public string InvalidTokensString { get; private set;}
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
        public BifyException(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) :base(message)
        {
            InvalidTokens = invalidTokens;
            LineTokens = tokens;
            CurrentLine = currentLine;

        }
        public BifyException(string message,string tokens,string invalidTokens, int currentLine) : base(message)
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
            if (LineTokens != null)
            {
                foreach (Token token in LineTokens)
                {

                    if (InvalidTokens.Contains(token))
                    {

                        ColorConsole.Write(token.Value, Color.Red);
                    }
                    else
                    {
                        ColorConsole.Write(token.Value, Color.White);
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

    }

    public class BifySyntaxError : BifyException
    {
        public BifySyntaxError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifySyntaxError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { Console.WriteLine("tokens " + tokens); }
    }

    public class BifyNameError : BifyException
    {
        public BifyNameError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyNameError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyTypeError : BifyException
    {
        public BifyTypeError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyTypeError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyValueError : BifyException
    {
        public BifyValueError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyValueError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyZeroDivisionError : BifyException
    {
        public BifyZeroDivisionError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyZeroDivisionError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyIndexError : BifyException
    {
        public BifyIndexError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyIndexError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyAttributeError : BifyException
    {
        public BifyAttributeError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyAttributeError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyKeyError : BifyException
    {
        public BifyKeyError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyKeyError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyAssertionError : BifyException
    {
        public BifyAssertionError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyAssertionError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyImportError : BifyException
    {
        public BifyImportError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyImportError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyIOError : BifyException
    {
        public BifyIOError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyIOError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyRuntimeError : BifyException
    {
        public BifyRuntimeError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyRuntimeError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyOverflowError : BifyException
    {
        public BifyOverflowError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyOverflowError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyIndentationError : BifyException
    {
        public BifyIndentationError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyIndentationError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyMemoryError : BifyException
    {
        public BifyMemoryError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyMemoryError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyRecursionError : BifyException
    {
        public BifyRecursionError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyRecursionError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyTimeoutError : BifyException
    {
        public BifyTimeoutError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyTimeoutError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyUnboundLocalError : BifyException
    {
        public BifyUnboundLocalError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyUnboundLocalError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyUnknownError : BifyException
    {
        public BifyUnknownError(string message, List<Token> tokens, List<Token> invalidTokens,int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyUnknownError(string message, string tokens, string invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
    }
}