using System;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

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
                return (pointer, operand); 

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
            return (lastOperand, final is AstUnaryOperator unaryOperator ? unaryOperator.Operand : final);
        }


    }
}
