using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast
{
    class IdentifierHandler : TokenHandler
    {
        public IdentifierHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            Token nextToken = TokensFormatter.GetTokenOrNull(builder.tokens, builder.tokenIndex + 1);
            if (nextToken != null && nextToken.Type == TokenType.LPAREN)
            {
                new CallHandler(builder).HandleToken(token);
            }
            builder.AddOperand(NodeConventer.TokenToNode(builder.tokens[builder.tokenIndex]));
        }
    }
}
