using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Lexer;
using BoomifyCS.Ast.Validators;

namespace BoomifyCS.Ast
{
    class UnaryOperatorHandler : TokenHandler
    {
        public UnaryOperatorHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            Token previousToken = TokensFormatter.GetTokenOrNull(builder.tokens, builder.tokenIndex - 1);
            UnaryOperatorValidator.Validate(builder.tokens[builder.tokenIndex].Value, previousToken);
            AstIdentifier astIdentifier = (AstIdentifier)NodeConventer.TokenToNode(previousToken);
            AstUnaryOperator unaryOperator = new(builder.tokens[builder.tokenIndex], astIdentifier, 1);
            builder.operandStack.Pop();
            builder.AddOperand(unaryOperator);
        }
    }
}
