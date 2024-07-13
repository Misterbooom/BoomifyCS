using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;

namespace BoomifyCS.Parser
{
    public static class BifyObjectParser
    {
        public static BifyObject CalculateBifyObjects(BifyObject a, BifyObject b, TokenType op)
        {
            if (op == TokenType.ADD)
            {
                return a.Add(b);
            }
            else if (op == TokenType.SUB)
            {
                return a.Sub(b);
            }
            else if (op == TokenType.MUL)
            {
                return a.Mul(b);
            }
            else if (op == TokenType.DIV)
            {
                return a.Div(b);
            }
            else if (op == TokenType.MOD)
            {
                return a.Mod(b);
            }
            else if (op == TokenType.POW)
            {
                return a.Pow(b);
            }
            else if (op == TokenType.FLOORDIV)
            {
                return a.FloorDiv(b);
            }
            throw new InvalidOperationException($"Unsupported operator: {op}");
        }
    }
}
