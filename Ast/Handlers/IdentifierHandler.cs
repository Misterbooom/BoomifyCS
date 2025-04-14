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
                new CallHandler(builder).HandleToken(token);
            }
           
            else if (builder.operandStack.Count == 1 && builder.operandStack.Peek() is AstIdentifier && builder.tokenIndex == builder.tokens.Count - 1)
            {
                builder.tokenIndex++;
                builder.AddOperand(NodeConventer.TokenToNode(token));

                new VariableDeclarationHandler(builder).HandleToken(token);
            }
            else
            {
                builder.AddOperand(NodeConventer.TokenToNode(token));
            }
        }
    }
}
