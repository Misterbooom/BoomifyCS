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
            AstNode identifierNode = builder.operandStack.Pop();
            AstNode typeNode = BuildPointerToType(identifierNode,
                builder.ParseCondition(builder.tokens[0..(builder.tokenIndex - 1)]));





            builder.tokenIndex++;

            List<Token> valueTokens = builder.tokens[builder.tokenIndex..];
            AstNode valueNode = builder.ParseCondition(valueTokens);
            builder.tokenIndex = builder.tokens.Count;

            VariableDeclarationValidator.Validate(identifierNode, typeNode, valueNode, valueTokens, token);

            AstAssignment astAssignment = new(token, identifierNode, valueNode);
            AstVarDecl astVarDecl = new(token, astAssignment, typeNode, valueNode);
            builder.operatorStack.Clear();
            builder.operandStack.Clear();
            builder.AddOperand(astVarDecl);
        }
        private AstNode BuildPointerToType(AstNode pointerToName, AstNode name)
        {
            if (pointerToName == null)
                return name;
            if (pointerToName  is not AstUnaryOperator)
            {
                return name;
            }
            AstNode current = pointerToName;
            while (current is AstUnaryOperator unaryOp && unaryOp.Token.Type == TokenType.MUL)
            {
                if (unaryOp == null)
                {
                    unaryOp.Operand = name;
                    return pointerToName;
                }
                current = unaryOp.Operand;
            }
            return pointerToName;
        }
    }
}
