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
            BifyDebug.Log($"Identifier Node - {identifierNode}");
            BifyDebug.Log($"Type Node - {typeNode}");
            List<Token> valueTokens = builder.tokens[builder.tokenIndex..];
            AstNode valueNode = builder.ParseCondition(valueTokens);
            VariableDeclarationValidator.Validate(identifierNode, typeNode, valueNode, valueTokens, token);
            AstAssignment astAssignment = new(token, identifierNode, valueNode);
            AstVarDecl astVarDecl = new(token, astAssignment, typeNode, valueNode);
            builder.tokenIndex = builder.tokens.Count;
            builder.AddOperand(astVarDecl);
        }
    }
}
