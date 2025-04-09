using System.Collections.Generic;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast
{
    public static class AstConfig
    {
        // Precedence configuration with higher values indicating higher precedence
        public static readonly Dictionary<TokenType, int> Precedence = new()
        {
            // Parentheses have the highest precedence, to ensure that expressions inside them are evaluated first
            { TokenType.LPAREN,   0 },
            { TokenType.RPAREN,   0 },

            // Arithmetic operators
            { TokenType.POW,      10 },      // Exponentiation
            { TokenType.MUL,      9 },       // Multiplication
            { TokenType.DIV,      9 },       // Division
            { TokenType.FLOORDIV, 9 },       // Floor division
            { TokenType.MOD,      9 },       // Modulo
            { TokenType.ADD,      8 },       // Addition
            { TokenType.SUB,      8 },       // Subtraction

            // Comparison operators
            { TokenType.GT,       7 },       // Greater than
            { TokenType.LT,       7 },       // Less than
            { TokenType.GTEQ,     7 },       // Greater than or equal to
            { TokenType.LTEQ,     7 },       // Less than or equal to
            { TokenType.EQ,       6 },       // Equal to
            { TokenType.NEQ,      6 },       // Not equal to

            // Boolean operators (AND, OR, NOT have lower precedence than arithmetic)
            { TokenType.AND,      5 },       // Logical AND
            { TokenType.OR,       4 },       // Logical OR
            { TokenType.NOT,      3 },       // Logical NOT

            // Bitwise operators
            { TokenType.BITAND,   2 },       // Bitwise AND
            { TokenType.BITOR,    2 },       // Bitwise OR
            { TokenType.BITXOR,   2 },       // Bitwise XOR
            { TokenType.BITNOT,   1 },       // Bitwise NOT

            // Increment/Decrement operators
            { TokenType.INCREMENT, 11 },     // Increment operator (highest precedence for unary operators)
            { TokenType.DECREMENT, 11 },     // Decrement operator

            // Other operators
            { TokenType.COMMA,    0 },       // Comma for separating arguments
            { TokenType.RANGE,    0 },       // Range operator (for slicing/ranges)
        };

        private enum PrecedenceLevel
        {
            Highest = 12,
            Arithmetic = 8,
            Comparison = 7,
            Boolean = 3,
            Bitwise = 1,
            Lowest = 0
        }
    }
}
