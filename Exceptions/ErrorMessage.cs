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
            return "'else if' cannot directly follow an 'else' statement. It must follow an 'if' or another 'else if'.";
        }

        public static string ForLoopMustHaveInitialization()
        {
            return "The 'for' loop is missing an initialization statement. A 'for' loop must begin with an initialization.";
        }

        public static string ForLoopMustHaveCondition()
        {
            return "The 'for' loop is missing a condition statement. A 'for' loop must include a condition to determine when to exit the loop.";
        }

        public static string ForLoopMustHaveIncrement()
        {
            return "The 'for' loop is missing an increment statement. A 'for' loop must include an increment statement to update the loop counter.";
        }

        public static string InvalidIncrementExpression()
        {
            return "Invalid increment expression in 'for' loop. The increment statement must be an assignment operation (e.g., i = i + 1), a unary operator (e.g., i++), or a function call.";
        }

        public static string InvalidVariableDeclaration()
        {
            return "The first statement in 'for' loop must be a variable initialization. Ensure that variables are properly declared and initialized.";
        }

        public static string InvalidConditionExpression()
        {
            return "Invalid condition expression. The condition in a 'for' loop must be a binary expression (e.g., i < 10).";
        }

        public static string InvalidInitializationStatement()
        {
            return "Incorrect number of semicolon-separated statements in 'for' loop. A 'for' loop must have exactly three semicolon-separated statements: initialization, condition, and increment.";
        }
        public static string MissingCloseQuotationMark()
        {
            return "You forgot to close a quotation mark.";
        }
        public static string NotEnoughOperands(string op)
        {
            return $"Not enough operands for operator - '{op}'";
        }
        public static string InvalidTypeForOperation(string op, string left, string right)
        {
            return $"Invalid type for operation '{op}' between '{left}' and '{right}'.";
        }
        public static string DivisionByZero() {
            return "Cannot Divide by zero.";
        }
    }
}
