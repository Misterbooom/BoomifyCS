using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Lexer;
using BoomifyCS.Ast.Validators;

namespace BoomifyCS.Ast
{
    class AssignmentOperatorHandler : TokenHandler
    {
        public AssignmentOperatorHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            Token assignToken = builder.tokens[builder.tokenIndex];
            Token variableToken = TokensFormatter.GetTokenOrNull(builder.tokens, builder.tokenIndex - 1);
            builder.tokenIndex += 1;
            List<Token> valueTokens = builder.tokens[builder.tokenIndex..];

            if (builder.operandStack.TryPop(out AstNode node) && node is AstIndexOperator indexOperator)
            {
                indexOperator.Token.Type = TokenType.INDEX_OPERATOR;
                VariableDeclarationValidator.Validate(indexOperator.Token, new Token(TokenType.ASSIGN, ""), valueTokens);
                builder.tokenIndex = builder.tokens.Count;
                AstBuilder valueBuilder = new(valueTokens);
                AstNode valueNode = valueBuilder.BuildNode();

                AstAssignmentOperator astAssignmentOperator = new(assignToken, indexOperator, valueNode);
                builder.AddOperand(astAssignmentOperator);

            }
            else
            {
                VariableDeclarationValidator.Validate(variableToken, new Token(TokenType.ASSIGN, ""), valueTokens);
                builder.tokenIndex = builder.tokens.Count;
                builder.operandStack.Pop();
                AstBuilder valueBuilder = new(valueTokens);
                AstNode valueNode = valueBuilder.BuildNode();

                AstAssignmentOperator astAssignmentOperator = new(assignToken, (AstIdentifier)NodeConventer.TokenToNode(variableToken), valueNode);
                builder.AddOperand(astAssignmentOperator);
            }

        }
    }
}
