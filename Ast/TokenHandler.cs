using System;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using LLVMSharp;

namespace BoomifyCS.Ast.Handlers
{
    abstract class TokenHandler
    {
        protected AstBuilder builder;

        protected TokenHandler(AstBuilder builder)
        {
            this.builder = builder;
        }

        public abstract void HandleToken(Token token);
        protected static (AstNode lastOperand, AstNode finalPointer) SwitchLastOperand(AstNode pointer, AstNode operand)
        {

            if (pointer is not AstUnaryOperator pointerNode)
            {
                return (pointer, operand);
            }

            static AstNode ReplaceLast(AstUnaryOperator node, AstNode replacement, out AstNode lastOperand)
            {

                if (node.Operand is AstUnaryOperator nested)
                {
                    var replaced = ReplaceLast(nested, replacement, out lastOperand);
                    return node.Update(replaced);
                }
                else
                {
                    lastOperand = node.Operand;
                    return node.Update(replacement);
                }
            }

            var final = ReplaceLast(pointerNode, operand, out var lastOperand);

            return (lastOperand, final );
        }
        protected static void GetVariableInfo(AstBuilder builder, out AstNode identifierNode, out AstNode typeNode, out AstNode flagNode)
        {

            if (builder.Nodes.Count == 2)
            {
                typeNode = builder.Nodes[0];
                identifierNode = builder.Nodes[1];
                flagNode = null;
            }
            else if (builder.Nodes.Count == 3)
            {
                flagNode = builder.Nodes[0];
                typeNode = builder.Nodes[1];
                identifierNode = builder.Nodes[2];
            }
            else
            {
                identifierNode = null;
                typeNode = null;
                flagNode = null;
            }
            if (identifierNode is AstUnaryOperator unaryOperator)
            {
                var (lastOperand, finalPointer) = SwitchLastOperand(unaryOperator, typeNode);
                identifierNode = lastOperand;
                typeNode = finalPointer;

            }


        }


    }
}
