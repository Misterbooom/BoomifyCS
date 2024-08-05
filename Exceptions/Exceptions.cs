using System;
using System.Collections.Generic;
using System.Text;
using BoomifyCS.Lexer;

namespace BoomifyCS.Exceptions
{
 

    public class BifySyntaxError : BifyError
    {
        public BifySyntaxError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifySyntaxError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyNameError : BifyError
    {
        public BifyNameError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyNameError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyTypeError : BifyError
    {
        public BifyTypeError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyTypeError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyValueError : BifyError
    {
        public BifyValueError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyValueError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyZeroDivisionError : BifyError
    {
        public BifyZeroDivisionError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyZeroDivisionError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyIndexError : BifyError
    {
        public BifyIndexError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyIndexError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyAttributeError : BifyError
    {
        public BifyAttributeError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyAttributeError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyKeyError : BifyError
    {
        public BifyKeyError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyKeyError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyAssertionError : BifyError
    {
        public BifyAssertionError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyAssertionError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyImportError : BifyError
    {
        public BifyImportError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyImportError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyIOError : BifyError
    {
        public BifyIOError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyIOError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyRuntimeError : BifyError
    {
        public BifyRuntimeError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyRuntimeError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyOverflowError : BifyError
    {
        public BifyOverflowError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyOverflowError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyIndentationError : BifyError
    {
        public BifyIndentationError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyIndentationError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyMemoryError : BifyError
    {
        public BifyMemoryError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyMemoryError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyRecursionError : BifyError
    {
        public BifyRecursionError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyRecursionError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyTimeoutError : BifyError
    {
        public BifyTimeoutError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyTimeoutError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyUnboundLocalError : BifyError
    {
        public BifyUnboundLocalError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyUnboundLocalError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyUnknownError : BifyError
    {
        public BifyUnknownError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyUnknownError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    // Additional Exceptions
    public class BifyStopIterationError : BifyError
    {
        public BifyStopIterationError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyStopIterationError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyArithmeticError : BifyError
    {
        public BifyArithmeticError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyArithmeticError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyFileNotFoundError : BifyError
    {
        public BifyFileNotFoundError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyFileNotFoundError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyPermissionError : BifyError
    {
        public BifyPermissionError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyPermissionError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyConnectionError : BifyError
    {
        public BifyConnectionError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public

     BifyConnectionError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyTimeoutExpiredError : BifyError
    {
        public BifyTimeoutExpiredError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyTimeoutExpiredError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyBrokenPipeError : BifyError
    {
        public BifyBrokenPipeError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyBrokenPipeError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyModuleNotFoundError : BifyError
    {
        public BifyModuleNotFoundError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyModuleNotFoundError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyEOFError : BifyError
    {
        public BifyEOFError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyEOFError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifySyntaxWarning : BifyError
    {
        public BifySyntaxWarning(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifySyntaxWarning(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyDeprecationWarning : BifyError
    {
        public BifyDeprecationWarning(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyDeprecationWarning(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }

    public class BifyResourceWarning : BifyError
    {
        public BifyResourceWarning(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyResourceWarning(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }
    public class BifyOperationError : BifyError
    {
        public BifyOperationError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyOperationError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }
    public class BifyParsingError : BifyError
    {
        public BifyParsingError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyParsingError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }
    public class BifyInitializationError : BifyError
    {
        public BifyInitializationError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyInitializationError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }
    public class BifyUndefinedError : BifyError
    {
        public BifyUndefinedError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyUndefinedError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }
    public class BifyArgumentError : BifyError
    {
        public BifyArgumentError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyArgumentError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }
    public class BifyCastError : BifyError
    {
        public BifyCastError(string message, List<Token> tokens, List<Token> invalidTokens, int currentLine) : base(message, tokens, invalidTokens, currentLine) { }
        public BifyCastError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : base(message, tokens, invalidTokens, currentLine) { }
    }
}
