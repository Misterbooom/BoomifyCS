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

            var tokensBehind = builder.tokens[typeStartIndex..(builder.tokenIndex - 1)];
            BifyDebug.Log($"Tokens behind: {tokensBehind.TokensToString()}");
            AstNode parsedType = builder.ParseTokens(tokensBehind);

            (AstNode lastOperand, AstNode typeNode) = SwitchLastPointerOperand(identifierNode, parsedType);

            if (builder.operandStack.Count == 2)
            {
                builder.operandStack.Pop();
                flagNode = builder.operandStack.Pop();
            }

            if (builder.tokenIndex < builder.tokens.Count)
            {
                builder.tokenIndex++;
            }

            List<Token> valueTokens = builder.tokens[builder.tokenIndex..];
            AstNode valueNode = builder.ParseTokens(valueTokens);

            builder.tokenIndex = builder.tokens.Count;

            // Fix for CS0103: Ensure typeNode is passed correctly
            VariableDeclarationValidator.Validate(
                identifierNode,
                typeNode,
                valueNode,
                valueTokens,
                flagNode,
                token
            );

            AstAssignment astAssignment = new(token, identifierNode, valueNode);
            AstVarDecl astVarDecl = new(token, astAssignment, typeNode, flagNode);

            builder.operatorStack.Clear();
            builder.operandStack.Clear();
            builder.AddOperand(astVarDecl);
        }
        public static (AstNode lastOperand, AstNode finalPointer) SwitchLastPointerOperand(AstNode pointer, AstNode operand)
        {
            if (pointer is not AstUnaryOperator pointerNode)
                return (pointer, operand);

            AstNode ReplaceLast(AstUnaryOperator node, AstNode replacement, out AstNode lastOperand)
            {
                if (node.Operand is AstUnaryOperator nested)
                {
                    var replaced = ReplaceLast(nested, replacement, out lastOperand);
                    return new AstUnaryOperator(node.Token, replaced); 
                }
                else
                {
                    lastOperand = node.Operand;
                    return new AstUnaryOperator(node.Token, replacement);
                }
            }

            var final = ReplaceLast(pointerNode, operand, out var lastOperand);
            BifyDebug.Log($"Final Pointer: {final}");

            return (lastOperand, final);
        }






    }
}