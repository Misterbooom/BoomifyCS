using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Lexer;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Parser;

namespace BoomifyCS.Ast
{
    class BracketHandler : TokenHandler
    {
        public BracketHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            List<Token> tokensInBrackets = TokensFormatter.GetTokensBetween(builder.tokens, ref builder.tokenIndex, TokenType.LBRACKET, TokenType.RBRACKET);
            if (builder.operandStack.TryPop(out AstNode previousNode))
            {
                AstNode indexNode = new AstBuilder(tokensInBrackets).BuildNode();
                IndexOperatorValidator.Validate(previousNode, indexNode);
                AstIndexOperator astIndexOperator = new(indexNode, previousNode);
                builder.AddOperand(astIndexOperator);
            }
            else
            {
                HandleArray(token, tokensInBrackets);
            }

        }

        private void HandleArray(Token token, List<Token> tokensInBrackets)
        {

            AstBuilder newBuilder = new(tokensInBrackets);
            AstNode valueNode = newBuilder.BuildNode();
            AstArray arrayNode = new(token, valueNode);
    

            builder.AddOperand(arrayNode);
        }
    }
}
