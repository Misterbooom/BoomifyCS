using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast
{
    public class AstConfig
    {
        public static Dictionary<TokenType, int> Precedence = new Dictionary<TokenType, int>
        {
            {TokenType.ADD, 1},
            {TokenType.SUB, 1},
            {TokenType.MUL, 2},
            {TokenType.DIV, 2},
            {TokenType.MOD, 2},
            {TokenType.POW, 3},
            {TokenType.EQ, 6},
            {TokenType.NEQ, 6},
            {TokenType.GT, 7},
            {TokenType.LT, 7},
            {TokenType.LPAREN,0},
            {TokenType.RPAREN, 0},
        };
        public static Dictionary<TokenType, Func<int, int, int>> operationDictionary = new Dictionary<TokenType, Func<int, int, int>>
        {
            { TokenType.ADD, (a, b) => a + b },
            { TokenType.SUB, (a, b) => a - b },
            { TokenType.MUL, (a, b) => a * b },
            { TokenType.DIV, (a, b) => b != 0 ? a / b : throw new DivideByZeroException("Cannot divide by zero.") },
            { TokenType.MOD, (a, b) => a % b },
            { TokenType.POW, (a, b) => (int)Math.Pow(a, b) } 
        };

    }
}
