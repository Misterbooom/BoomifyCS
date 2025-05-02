using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Handlers
{
    class ArrayHandler(AstBuilder builder):TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {
            builder.CurrentNode = GetArrayNode(token);
        }
        public AstNode GetArrayNode(Token token)
        {
            var tokensInBrackets = TokensFormatter.GetTokensBetween(builder.tokens, ref builder.tokenIndex,
               TokenType.LBRACKET, TokenType.RBRACKET);

            AstNode valueNode = builder.ParseTokens(tokensInBrackets);

            builder.tokenIndex++;
            return new AstArray(token, valueNode);
        }
    }
}
