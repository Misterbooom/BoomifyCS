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
            PointerValue pointerValue = GetPointerValue(assignmentOperatorNode);
            BifyType targetType = ((BifyPointerType)pointerValue.GetBifyType()).PointedType;
            BifyValue operandValue = GetOperandValue(assignmentOperatorNode);
            BifyValue dereferencedValue = pointerValue.Dereference();

            BifyValue resultValue = Calculate(dereferencedValue, operandValue, assignmentOperatorNode.Token.Type)
                .ExplicitCast(targetType, compiler.Builder);
            compiler.Builder.BuildStore(resultValue.GetLLVMValue(), pointerValue.GetLLVMValue());
        }

        private BifyValue Calculate(BifyValue lhs, BifyValue rhs, TokenType token)
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

        private PointerValue GetPointerValue(AstAssignmentOperator assignmentOperatorNode)
        {
            compiler.Flag |= NodeVisitFlag.ASSIGNMENT_INDEX;
            compiler.Visit(assignmentOperatorNode.IdentifierNode);
            IValue iValue = compiler.StackIValuePop();
            if (iValue is BifyType bifyType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"{bifyType.Name} cannot be used as type."));
                return null;
            }
            PointerValue targetPointer = iValue as PointerValue;
            if (targetPointer == null)
            {
                throw new NotSupportedException($"Assignment operator is not supported for {((BifyValue)iValue).GetBifyType().GetType()} not a {iValue}");
            }
            if (targetPointer.ValueFlag.HasFlag(ValueFlag.Constant))
            {
                new BifyTypeError("Assigning to constant variable!").Throw();
            }
            return targetPointer;
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
