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