using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast
{
    class BinaryOperatorHandler : TokenHandler
    {
        public BinaryOperatorHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            while (builder.operatorStack.Count > 0 && builder.ShouldPopOperator(token))
            {
                builder.PopOperator();
            }

            AstNode operatorNode = new AstBinaryOp(token);
            builder.AddOperator(operatorNode);
        }
    }
}
