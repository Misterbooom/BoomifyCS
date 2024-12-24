using System;
using BoomifyCS.Lexer;

namespace BoomifyCS.Exceptions
{
    public static class ErrorMessage
    {
        public static string InvalidForLoopStructure() => "Invalid for loop structure. A for loop requires an initialization, condition, and increment statement.";
        public static string InitStatementIsRequired() => "Initialization statement is required in a for loop.";
        public static string ConditionIsRequired() => "Condition statement is required in a for loop.";
        public static string IncrementStatementIsRequired() => "Increment statement is required in a for loop.";
        public static string InvalidInitNode() => "Initialization node is missing or invalid in the for loop.";
        public static string InvalidInitNodeType() => "Initialization node type is invalid. Expected variable declaration or assignment.";
        public static string InvalidConditionNode() => "Condition node is missing or invalid in the for loop.";
        public static string InvalidConditionNodeType() => "Condition node type is invalid. Expected binary operation, unary operation, identifier, function call, or indexing operator.";
        public static string InvalidIncrementNode() => "Increment node is missing or invalid in the for loop.";
        public static string InvalidIncrementNodeType() => "Increment node type is invalid. Expected assignment or unary operation.";
        public static string InvalidOperandType(string operand)
        {
            return $"Invalid operand type '{operand}'.";
        }
        public static string UnmatchedOpeningParenthesis()
        {
            return "Unmatched '('. Expected closing parenthesis ')'.";
        }
        public static string UnmatchedToken(string open, string close)
        {
            return $"Unmatched token: expected a '{close}' to match the '{open}' token.";
        }
        public static string UnmatchedClosingParenthesis()
        {
            return "Unmatched ')'. Expected opening parenthesis '('.";
        }

        public static string UnmatchedOpeningBrace()
        {
            return "Unmatched '{'. Expected closing brace '}'.";
        }

        public static string UnmatchedClosingBrace()
        {
            return "Unmatched '}'. Expected opening brace '{'.";
        }
        public static string UnmatchedOpeningBracket()
        {
            return "Unmatched '['. Expected closing bracket ']'.";
        }
        public static string UnmatchedClosingBracket()
        {
            return "Unmatched ']'. Expected opening bracket '['.";
        }
        public static string ElseWithoutMatchingIf()
        {
            return "'else' statement without a matching 'if' statement.";
        }

        public static string ElseIfWithoutMatchingIf()
        {
            return "'else if' statement without a matching 'if' statement.";
        }

        public static string ElseIfCannotFollowElseDirectly()
        {
            return "'else if' cannot follow an 'else' statement directly. It must follow an 'if' or another 'else if'.";
        }



        public static string InvalidVariableDeclaration()
        {
            return "The first statement in the 'for' loop must be a variable initialization.";
        }
        public static string InvalidIndexTarget()
        {
            return "The target of an index operation must be an array, identifier, or callable.";
        }

 
        public static string MissingCloseQuotationMark()
        {
            return "Missing closing quotation mark.";
        }

        public static string NotEnoughOperands(string op)
        {
            return $"Not enough operands for operator '{op}'.";
        }

        public static string InvalidTypeForOperation(string op, string left, string right)
        {
            return $"Invalid type for operation '{op}' between '{left}' and '{right}'.";
        }

        public static string DivisionByZero()
        {
            return "Division by zero is not allowed.";
        }

        public static string IndexOfArrayTypeError()
        {
            return "Array index must be an integer.";
        }
        public static string ArrayIsEmpty()
        {
            return "Array is Empty";
        }
        public static string InvalidIndex(int index, int length)
        {
            return $"Invalid array index '{index}'. Array length is {length}.";
        }
        public static string MissingOperandAfterUnaryOperator(string operatorSymbol)
        {
            return $"Missing operand before unary operator '{operatorSymbol}'.";
        }
        public static string OperandMustBeIdentifierAfterUnaryOperator(string operatorSymbol)
        {
            return $"Operand before unary operator '{operatorSymbol}' must be an identifier.";
        }

        public static string UnexpectedToken(string token)
        {
            return $"Unexpected token '{token}'";
        }
        public static string ExpectedValueAfterAssignment()
        {
            return "Expected a value after assignment operator.";
        }
        public static string ExpectedTokenNotFound(string tokenType)
        {
            return $"Expected token of type {tokenType} not found";
        }

        public static string InvalidFunctionDeclaration()
        {
            return "Invalid function declaration syntax\r\n";
        }
        public static string ExpectedFunctionName()
        {
            return "Expected function name";
        }
        public static string InvalidParameter(string parameterType)
        {
            return $"Invalid parameter type '{parameterType}'. Expected a valid identifier or comma.";
        }
        public static string InvalidIndexExpression()
        {
            return "The index of an array or callable must be a number, range, or identifier.";
        }
        public static string OperationNotSupported(string operation, string objectName)
        {
            return $"{operation} not supported for {objectName}.";
        }

        public static string InvalidVariableName(string var)
        {
            return $"Invalid variable name. The variable must be a valid identifier. Not a '{var}'";
        }
        public static string EmptyValueAssigned()
        {
            return "A value must be assigned after the '=' operator.";
        }
        public static string MissingAssignmentOperator()
        {
            return "Missing assignment operator (=).";
        }


        public static string InvalidSyntaxInAssignment()
        {
            return "Invalid syntax in assignment statement.";
        }
    }
}
