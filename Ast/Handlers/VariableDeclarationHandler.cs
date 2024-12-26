using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Lexer;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Parser;
namespace BoomifyCS.Ast
{
    class VariableDeclarationHandler : TokenHandler
    {
        public VariableDeclarationHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            Token variableToken = TokensFormatter.GetTokenOrNull(builder.tokens, builder.tokenIndex + 1);
            Token assignmentToken = TokensFormatter.GetTokenOrNull(builder.tokens, builder.tokenIndex + 2);
            builder.tokenIndex += 3;
            List<Token> valueTokens = builder.tokens[builder.tokenIndex..];
            builder.tokenIndex = builder.tokens.Count;
            VariableDeclarationValidator.Validate(variableToken, assignmentToken, valueTokens);
            AstBuilder valueBuilder = new(valueTokens);
            AstNode valueNode = valueBuilder.BuildNode();
            Console.WriteLine($"Value - {valueNode}");
            AstIdentifier identifierNode = (AstIdentifier)NodeConventer.TokenToNode(variableToken);
            identifierNode.LineNumber = variableToken.Line;
            AstAssignment assignmentNode = new(assignmentToken, identifierNode, valueNode);
            AstVarDecl varDeclNode = new(builder.tokens[builder.tokenIndex - 3], assignmentNode);
            builder.AddOperand(varDeclNode);
        }
    }
}
