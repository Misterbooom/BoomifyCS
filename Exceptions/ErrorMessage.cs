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
        public static string InvalidOperandType(string operand) => $"Invalid operand type '{operand}'.";
        public static string UnmatchedOpeningParenthesis() => "Unmatched '('. Expected closing parenthesis ')'.";
        public static string UnmatchedToken(string open, string close) => $"Unmatched token: expected a '{close}' to match the '{open}' token.";
        public static string UnmatchedClosingParenthesis() => "Unmatched ')'. Expected opening parenthesis '('.";

        public static string UnmatchedOpeningBrace() => "Unmatched '{'. Expected closing brace '}'.";

        public static string UnmatchedClosingBrace() => "Unmatched '}'. Expected opening brace '{'.";
        public static string UnmatchedOpeningBracket() => "Unmatched '['. Expected closing bracket ']'.";
        public static string UnmatchedClosingBracket() => "Unmatched ']'. Expected opening bracket '['.";
        public static string ElseWithoutMatchingIf() => "'else' statement without a matching 'if' statement.";

        public static string ElseIfWithoutMatchingIf() => "'else if' statement without a matching 'if' statement.";

        public static string ElseIfCannotFollowElseDirectly() => "'else if' cannot follow an 'else' statement directly. It must follow an 'if' or another 'else if'.";



        public static string InvalidVariableDeclaration() => "The first statement in the 'for' loop must be a variable initialization.";
        public static string InvalidIndexTarget() => "The target of an index operation must be an array, identifier, or callable.";


        public static string MissingCloseQuotationMark() => "Missing closing quotation mark.";

        public static string NotEnoughOperands(string op) => $"Not enough operands for operator '{op}'.";

        public static string InvalidTypeForOperation(string op, string left, string right) => $"Invalid type for operation '{op}' between '{left}' and '{right}'.";

        public static string DivisionByZero() => "Division by zero is not allowed.";

        public static string IndexOfArrayTypeError() => "Array index must be an integer.";
        public static string ArrayIsEmpty() => "Array is Empty";
        public static string InvalidIndex(int index, int length) => $"Invalid array index '{index}'. Array length is {length}.";
        public static string MissingOperandAfterUnaryOperator(string operatorSymbol) => $"Missing operand before unary operator '{operatorSymbol}'.";
        public static string OperandMustBeIdentifierAfterUnaryOperator(string operatorSymbol) => $"Operand before unary operator '{operatorSymbol}' must be an identifier.";

        public static string UnexpectedToken(string token) => $"Unexpected token '{token}'";
        public static string ExpectedValueAfterAssignment() => "Expected a value after assignment operator.";
        public static string ExpectedTokenNotFound(string tokenType) => $"Expected token of type {tokenType} not found";

        public static string InvalidFunctionDeclaration() => "Invalid function declaration syntax\r\n";
        public static string ExpectedFunctionName() => "Expected function name";
        public static string InvalidParameter(string parameterType) => $"Invalid parameter type '{parameterType}'. Expected a valid identifier or comma.";
        public static string InvalidIndexExpression() => "The index of an array or callable must be a number, range, or identifier.";
        public static string OperationNotSupported(string operation, string objectName) => $"{operation} not supported for {objectName}.";

        public static string InvalidVariableName(string var) => $"Invalid variable name. The variable must be a valid identifier. Not a '{var}'";
        public static string EmptyValueAssigned() => "A value must be assigned after the '=' operator.";
        public static string MissingAssignmentOperator() => "Missing assignment operator (=).";


        public static string InvalidSyntaxInAssignment() => "Invalid syntax in assignment statement.";

        public static string ExpectedTypeOfVariable() => "Expected type of variable not found during assignment.";

        public static string InvalidVariableType(string value) => $"Invalid variable type '{value}'.";
    }
}
