using System.Collections.Generic;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast
{
    public static class AstConfig
    {
        public static readonly Dictionary<TokenType, int> Precedence = new()
        {
            { TokenType.LPAREN, (int)PrecedenceLevel.Lowest },
            { TokenType.RPAREN, (int)PrecedenceLevel.Lowest },
            { TokenType.POW, (int)PrecedenceLevel.Power },
            { TokenType.MUL, (int)PrecedenceLevel.Product },
            { TokenType.DIV, (int)PrecedenceLevel.Product },
            { TokenType.FLOORDIV, (int)PrecedenceLevel.Product },
            { TokenType.MOD, (int)PrecedenceLevel.Product },
            { TokenType.ADD, (int)PrecedenceLevel.Sum },
            { TokenType.SUB, (int)PrecedenceLevel.Sum },
            { TokenType.GT, (int)PrecedenceLevel.Comparison },
            { TokenType.LT, (int)PrecedenceLevel.Comparison },
            { TokenType.GTEQ, (int)PrecedenceLevel.Comparison },
            { TokenType.LTEQ, (int)PrecedenceLevel.Comparison },
            { TokenType.EQ, (int)PrecedenceLevel.Equality },
            { TokenType.NEQ, (int)PrecedenceLevel.Equality },
            { TokenType.INCREMENT, (int)PrecedenceLevel.Product },
            { TokenType.DECREMENT, (int)PrecedenceLevel.Product },
            { TokenType.BITAND, (int)PrecedenceLevel.BitwiseAnd },
            { TokenType.BITOR, (int)PrecedenceLevel.BitwiseOr },
            { TokenType.BITXOR, (int)PrecedenceLevel.BitwiseXor },
            { TokenType.BITNOT, (int)PrecedenceLevel.BitwiseNot },
            { TokenType.NOT, (int)PrecedenceLevel.BitwiseNot }, // Added NOT operator
            { TokenType.LSHIFT, (int)PrecedenceLevel.Shift },
            { TokenType.RSHIFT, (int)PrecedenceLevel.Shift },
            { TokenType.AND, (int)PrecedenceLevel.And },
            { TokenType.OR, (int)PrecedenceLevel.Or },
            { TokenType.COMMA, (int)PrecedenceLevel.Lowest },
            { TokenType.RANGE, (int)PrecedenceLevel.Range }
        };

        private enum PrecedenceLevel
        {
            Lowest = 0,
            Sum = 1,
            Product = 2,
            Power = 3,
            Shift = 4,
            BitwiseNot = 5, // BitwiseNot and NOT share the same precedence
            BitwiseOr = 6,
            BitwiseXor = 7,
            BitwiseAnd = 8,
            Equality = 9,
            Comparison = 10,
            Range = 11, // Added Range Precedence
            And = 12,
            Or = 13,
        }
    }
}
