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
        public static Dictionary<string, int> Precedence = new Dictionary<string, int>
        {
            { "++", 30 },
            { "--", 30 },
            { "+", 20 },
            { "-", 20 },
            { "*", 40 },
            { "/", 40 },
            { "%", 40 },
            { "==", 10 },
            { "!=", 10 },
            { ">", 10 },
            { "<", 10 },
            { ">=", 10 },
            { "<=", 10 },
            { "&&", 5 },
            { "||", 4 },
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
