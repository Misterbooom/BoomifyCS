using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Lexer;
using BoomifyCS.Ast.Validators;
using System.Diagnostics;
using BoomifyCS.Exceptions;
using BoomifyCS.Parser;
namespace BoomifyCS.Ast
{
    class AssignmentOperatorHandler : TokenHandler
    {
        public AssignmentOperatorHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            Token assignToken = builder.tokens[builder.tokenIndex];
            builder.tokenIndex += 1;
            List<Token> valueTokens = builder.tokens[builder.tokenIndex..];

            if (!builder.operandStack.TryPop(out AstNode node))
            {
                Traceback.Instance.ThrowException(
                    new BifySyntaxError($"Expected an identifier  but got {node.Token.Value}", "", node.Token.Value
                    ));
            }
            if (node is AstIndexOperator indexOperator)
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

                Token variableToken = node.Token;

                VariableDeclarationValidator.Validate(variableToken, token, valueTokens);
                builder.tokenIndex = builder.tokens.Count;
                AstBuilder valueBuilder = new(valueTokens);
                AstNode valueNode = valueBuilder.BuildNode();

                AstAssignmentOperator astAssignmentOperator = new(assignToken, (AstIdentifier)node, valueNode);
                builder.AddOperand(astAssignmentOperator);
            }

        }
    }
}
