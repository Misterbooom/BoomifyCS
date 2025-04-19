using System;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class AssignmentOperatorNodeHandler : NodeHandler
    {
        public AssignmentOperatorNodeHandler(AssemblyCompiler compiler)
            : base(compiler)
        {
        }

        public override void HandleNode(AstNode node)
        {
            AstAssignmentOperator assignmentOperatorNode = (AstAssignmentOperator)node;
            AllocaPointer allocaPointer = GetAllocaPointer(assignmentOperatorNode);
            BifyType targetType = ((AllocaType)allocaPointer.GetBifyType()).PointedType;
            BifyValue operandValue = GetOperandValue(assignmentOperatorNode);
            BifyValue dereferencedValue = allocaPointer.Dereference();

            BifyValue resultValue = Calculate(dereferencedValue, operandValue, assignmentOperatorNode.Token.Type)
                .ExplicitCast(targetType,compiler.Builder);
            compiler.Builder.BuildStore(resultValue.GetLLVMValue(), allocaPointer.GetLLVMValue());



        }
        private BifyValue Calculate(BifyValue lhs,BifyValue rhs,TokenType token)
        {
            switch (token)
            {
                case TokenType.ADDE:
                    return lhs.Add(rhs, compiler.Builder);
                case TokenType.SUBE:
                    return lhs.Sub(rhs, compiler.Builder);
                case TokenType.MULE:
                    return lhs.Mul(rhs, compiler.Builder);
                case TokenType.DIVE:
                    return lhs.Div(rhs, compiler.Builder);
                case TokenType.ASSIGN:
                    return rhs;
                default:
                    throw new NotSupportedException($"Operator {token} is not supported.");
            }
        }
        private AllocaPointer GetAllocaPointer(AstAssignmentOperator assignmentOperatorNode)
        {
            compiler.Flag |= NodeVisitFlag.ASSIGNMENT_INDEX;
            compiler.Visit(assignmentOperatorNode.IdentifierNode);
            IValue iValue = compiler.StackIValuePop();
            if (iValue is BifyType bifyType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"{bifyType.Name} cannot be used as type."));
                return null;
            }
            AllocaPointer targetAlloca = iValue as AllocaPointer;
            if (targetAlloca == null)
            {
                throw new NotSupportedException($"Assignment operator is not supported for {((BifyValue)iValue).GetBifyType().GetType()}");
            }
            if (targetAlloca.ValueFlag.HasFlag(ValueFlag.Constant))
            {
                new BifyTypeError("Assigning to constant variable!").Throw();
            }
            return targetAlloca;
        }
        private BifyValue GetOperandValue(AstAssignmentOperator assignmentOperatorNode)
        {
            compiler.Visit(assignmentOperatorNode.ValueNode);
            IValue iValue = compiler.StackIValuePop();
            if (iValue is BifyType bifyType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"{bifyType.Name} cannot be used as type."));
                return null;
            }
            return (BifyValue)iValue;

        }

    }
}
