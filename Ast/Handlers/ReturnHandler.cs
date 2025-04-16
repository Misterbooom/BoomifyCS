using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Handlers
{
    class ReturnHandler(AstBuilder builder):TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {

            builder.NextToken();
            AstNode valueNode = builder.ParseTokens(builder.tokens[(builder.tokenIndex)..]);
            builder.MoveToEnd();
            builder.CurrentNode = new AstReturn(token, valueNode);
        }
    }
}
