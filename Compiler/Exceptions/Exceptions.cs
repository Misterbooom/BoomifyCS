using System.Collections.Generic;
using BoomifyCS.Lexer;

namespace BoomifyCS.Exceptions
{


    public class BifySyntaxError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyNameError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyTypeError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyValueError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyZeroDivisionError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyIndexError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyAttributeError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyKeyError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyAssertionError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyImportError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyIOError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyRuntimeError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyOverflowError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyIndentationError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyMemoryError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyRecursionError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyTimeoutError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyUnboundLocalError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyUnknownError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyStopIterationError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyArithmeticError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyFileNotFoundError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyPermissionError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyConnectionError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyTimeoutExpiredError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyBrokenPipeError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyModuleNotFoundError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyEOFError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifySyntaxWarning(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyDeprecationWarning(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }

    public class BifyResourceWarning(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }
    public class BifyOperationError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }
    public class BifyParsingError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }
    public class BifyInitializationError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }
    public class BifyUndefinedError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }
    public class BifyArgumentError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }
    public class BifyCastError(string message, string tokens = "", string invalidTokens = "", int currentLine = 0) : BifyError(message, tokens, invalidTokens, currentLine)
    {
    }
    public class BifyNullError(string message, string tokens = "", string invalidTokens = "",int currentLine = 0): BifyError(message,tokens, invalidTokens, currentLine) { } 
}
