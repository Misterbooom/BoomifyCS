using System;
using System.Collections.Generic;
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
        protected static void GetVariableInfo(AstBuilder builder, out AstNode identifierNode, out AstNode typeNode, out AstFlag flagNode)
        {
            identifierNode = null;
            typeNode = null;
            var flagNodes = new List<AstNode>();

            int count = builder.Nodes.Count;
            flagNode = default;
            if (count < 2)
                return;

            typeNode = builder.Nodes[count - 2];
            identifierNode = builder.Nodes[count - 1];

            for (int i = 0; i < count - 2; i++)
            {
                var flagCandidate = builder.Nodes[i];
                flagNodes.Add(flagCandidate);
            }

            if (identifierNode is AstUnaryOperator unaryOperator)
            {
                var (lastOperand, finalPointer) = SwitchLastOperand(unaryOperator, typeNode);
                identifierNode = lastOperand;
                typeNode = finalPointer;
            }
            flagNode = new AstFlag(identifierNode.Token,flagNodes);
        }


    }
}
