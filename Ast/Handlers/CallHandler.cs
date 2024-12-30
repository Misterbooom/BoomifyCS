using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast
{
    class CallHandler : TokenHandler
    {
        public CallHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            if (builder.operandStack.Count == 1 && builder.operatorStack.Count == 0)
            {
                new FunctionDeclarationHandler(builder).HandleToken(token);
            }
            else
            {
                HandleCall();
            }
       
    
        }
        private void HandleCall()
        {
            AstIdentifier identifier = (AstIdentifier)NodeConventer.TokenToNode(builder.tokens[builder.tokenIndex]);
            List<Token> parenthesisTokens = TokensFormatter.GetTokensBetween(builder.tokens, ref builder.tokenIndex, TokenType.LPAREN, TokenType.RPAREN);
            AstNode argumentsNode = new AstBuilder(parenthesisTokens).BuildNode();
            identifier.Token.Type = TokenType.CALL;
            AstCall astCall = new(identifier.Token, identifier, argumentsNode);
            builder.AddOperand(astCall);
        }
    }
}
