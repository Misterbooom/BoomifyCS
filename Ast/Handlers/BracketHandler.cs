using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Lexer;
using BoomifyCS.Ast.Validators;

namespace BoomifyCS.Ast
{
    class BracketHandler : TokenHandler
    {
        public BracketHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            List<Token> tokensInBrackets = TokensFormatter.GetTokensBetween(builder.tokens, ref builder.tokenIndex, TokenType.LBRACKET, TokenType.RBRACKET);
            if (builder.operatorStack.TryPop(out AstNode operatorNode))
            {
                new ArrayHandler(builder).HandleToken(token);
            }
            else if (builder.operandStack.TryPop(out AstNode previousNode))
            {
                Console.WriteLine(builder.operatorStack.ToString());
                AstNode indexNode = new AstBuilder(tokensInBrackets).BuildNode();
                IndexOperatorValidator.Validate(previousNode, indexNode);
                AstIndexOperator astIndexOperator = new(indexNode, previousNode);
                builder.AddOperand(astIndexOperator);
            }
        }
    }
}
