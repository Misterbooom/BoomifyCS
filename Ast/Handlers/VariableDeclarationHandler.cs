using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Lexer;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Parser;
using BoomifyCS.Exceptions;
namespace BoomifyCS.Ast
{
    class VariableDeclarationHandler : TokenHandler
    {
        public VariableDeclarationHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            builder.tokenIndex++; // To Skip the ASSIGN token
            AstNode identifierNode = builder.operandStack.Pop();
            AstNode typeNode = builder.operandStack.Pop();

            List<Token> valueTokens = builder.tokens[builder.tokenIndex..];
            BifyDebug.Log($"Value Tokens - {valueTokens.TokensToString()}");
            AstNode valueNode = builder.ParseCondition(valueTokens);
            builder.tokenIndex = builder.tokens.Count;

            VariableDeclarationValidator.Validate(identifierNode, typeNode, valueNode, valueTokens, token);

            AstAssignment astAssignment = new(token, identifierNode, valueNode);
            AstVarDecl astVarDecl = new(token, astAssignment, typeNode, valueNode);
            builder.AddOperand(astVarDecl);
        }
    }
}
