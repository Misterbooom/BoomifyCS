using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast
{
    class DotHandler : TokenHandler
    {
        public DotHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            Token nextToken = TokensFormatter.GetTokenOrNull(builder.tokens, builder.tokenIndex + 1);
            if (nextToken.Type == TokenType.DOT)
            {
                Token rangeToken = new Token(TokenType.RANGE, builder.tokens[builder.tokenIndex + 1].Value + builder.tokens[builder.tokenIndex]);
                builder.AddOperator(new AstRangeOperator(rangeToken));
                builder.tokenIndex++;
            }
        }
    }
}
