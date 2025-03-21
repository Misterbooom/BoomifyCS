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
            builder.operandStack.WriteNodes();

            AstNode flagNode = null;

            AstNode identifierNode = builder.operandStack.Pop();

            int typeStartIndex = (builder.operandStack.Count == 2) ? 1 : 0;
            AstNode parsedType = builder.ParseCondition(builder.tokens[typeStartIndex..(builder.tokenIndex - 1)]);
            AstNode typeNode = BuildPointerToType(identifierNode, parsedType);

            if (builder.operandStack.Count == 2)
            {
                builder.operandStack.Pop();
                flagNode = builder.operandStack.Pop();
            }

            builder.tokenIndex++;

            List<Token> valueTokens = builder.tokens[builder.tokenIndex..];
            AstNode valueNode = builder.ParseCondition(valueTokens);
            builder.tokenIndex = builder.tokens.Count;

            VariableDeclarationValidator.Validate(identifierNode, typeNode, valueNode, valueTokens, flagNode, token);

            AstAssignment astAssignment = new(token, identifierNode, valueNode);
            AstVarDecl astVarDecl = new(token, astAssignment, typeNode, flagNode);

            builder.operatorStack.Clear();
            builder.operandStack.Clear();
            builder.AddOperand(astVarDecl);
        }

 
        private AstNode BuildPointerToType(AstNode pointerToName, AstNode name)
        {
            if (pointerToName == null || pointerToName is not AstUnaryOperator)
                return name;

            AstUnaryOperator current = (AstUnaryOperator)pointerToName;
            while (current.Operand is AstUnaryOperator next && next.Token.Type == TokenType.MUL)
            {
                current = next;
            }

            if (current.Operand == null)
            {
                current.Operand = name;
            }
            return pointerToName;
        }
    }
}
