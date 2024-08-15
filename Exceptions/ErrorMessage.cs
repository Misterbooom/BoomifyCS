using System;

namespace BoomifyCS.Exceptions
{
    public static class ErrorMessage
    {
        public static string UnmatchedOpeningParenthesis()
        {
            return "Unmatched '('. Expected closing parenthesis ')'.";
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

        public static string ForLoopMustHaveInitialization()
        {
            return "The 'for' loop must include an initialization statement.";
        }

        public static string ForLoopMustHaveCondition()
        {
            return "The 'for' loop must include a condition statement.";
        }

        public static string ForLoopMustHaveIncrement()
        {
            return "The 'for' loop must include an increment statement.";
        }

        public static string InvalidIncrementExpression()
        {
            return "Invalid increment expression in 'for' loop. The increment must be an assignment, a unary operation, or a function call.";
        }

        public static string InvalidVariableDeclaration()
        {
            return "The first statement in the 'for' loop must be a variable initialization.";
        }

        public static string InvalidConditionExpression()
        {
            return "The condition in a 'for' loop must be a valid expression.";
        }

        public static string InvalidInitializationStatement()
        {
            return "Incorrect number of semicolon-separated statements in 'for' loop. There should be exactly three: initialization, condition, and increment.";
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
    }
}
