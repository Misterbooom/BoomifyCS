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
        { TokenType.LPAREN, 0 },
        { TokenType.RPAREN, 0 },
        { TokenType.POW, 3 },
        { TokenType.MUL, 2 },
        { TokenType.DIV, 2 },
        { TokenType.FLOORDIV,2 },
        { TokenType.MOD, 2 },
        { TokenType.ADD, 1 },
        { TokenType.SUB, 1 },
        { TokenType.GT, 7 },
        { TokenType.LT, 7 },
        { TokenType.GTEQ, 7 },
        { TokenType.LTEQ, 7 },
        { TokenType.EQ, 6 },
        { TokenType.NEQ, 6 },
        { TokenType.INCREMENT,2},
        { TokenType.DECREMENT,2},

        };


    }
}
