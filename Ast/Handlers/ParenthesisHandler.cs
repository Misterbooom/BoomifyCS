using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Lexer;
using BoomifyCS.Ast.Validators;

namespace BoomifyCS.Ast
{
    class ParenthesisHandler : TokenHandler
    {
        public ParenthesisHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            List<Token> parenthesisTokens = TokensFormatter.GetTokensBetween(builder.tokens, ref builder.tokenIndex, TokenType.LPAREN, TokenType.RPAREN);
            AstBuilder parenthesisBuilder = new(parenthesisTokens);
            builder.operandStack.Push(parenthesisBuilder.BuildNode());
        }
    }
}
