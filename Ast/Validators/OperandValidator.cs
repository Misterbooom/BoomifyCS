using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Ast.Validators
{
    public class OperandValidator
    {
        public static void Validate(AstNode left, AstNode right, AstNode operatorNode)
        {
            if (left == null || right == null)
            {
                BifyArithmeticError error = new(ErrorMessage.NotEnoughOperands(operatorNode.Token.Value), "", operatorNode.Token.Value);
                Traceback.Traceback.Instance.ThrowException(error, operatorNode.Token.Column);
            }
            CheckOperandType(left, operatorNode);
            CheckOperandType(right, operatorNode);


        }
        private static void CheckOperandType(AstNode node, AstNode operatorNode)
        {
            if (!IsOperand(node))
            {
                BifyArithmeticError error = new(ErrorMessage.InvalidOperandType(node.GetType().Name.Replace("Ast","")), "", node.Token.Value);
                Traceback.Traceback.Instance.ThrowException(error,node.Token.Column);

            }
        }

        private static bool IsOperand(AstNode node)
        {
            return node is AstConstant ||
                   node is AstIdentifier ||
                   node is AstCall ||
                   node is AstUnaryOperator || 
                   node is AstArray ||
                   node is AstAssignment || 
                   node is AstBinaryOp; 
        }

    }
}
