using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast
{
    class ArrayHandler : TokenHandler
    {
        public ArrayHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            List<Token> tokensInBrackets = TokensFormatter.GetTokensBetween(builder.tokens, ref builder.tokenIndex, TokenType.LBRACKET, TokenType.RBRACKET);
            AstNode valueNode = new AstBuilder(tokensInBrackets).BuildNode();
            AstArray arrayNode = new(builder.tokens[builder.tokenIndex - tokensInBrackets.Count], valueNode);
            builder.AddOperand(arrayNode);
        }
    }
}
