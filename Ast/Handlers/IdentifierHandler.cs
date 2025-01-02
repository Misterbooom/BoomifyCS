using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Exceptions;
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
                BifyDebug.Log("Handling call nde");
                new CallHandler(builder).HandleToken(token);
            }
            else
            {
                builder.AddOperand(NodeConventer.TokenToNode(token));
            }
        }
    }
}
