using System;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class AssignmentOperatorNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstAssignmentOperator assignmentOperatorNode = (AstAssignmentOperator)node;
            PointerValue pointerValue = GetPointerValue(assignmentOperatorNode);
            BifyType targetType = ((BifyPointerType)pointerValue.GetBifyType()).PointedType;
            BifyValue operandValue = GetOperandValue(assignmentOperatorNode);
            BifyValue dereferencedValue = pointerValue.Dereference();

            BifyValue resultValue = Calculate(dereferencedValue, operandValue, assignmentOperatorNode.Token.Type)
                .ExplicitCast(targetType, Compiler.Builder);
            Compiler.Builder.BuildStore(resultValue.GetLlvmValue(), pointerValue.GetLlvmValue());
        }

        private BifyValue Calculate(BifyValue lhs, BifyValue rhs, TokenType token)
        {
            switch (token)
            {
                case TokenType.ADDE:
                    return lhs.Add(rhs, Compiler.Builder);
                case TokenType.SUBE:
                    return lhs.Sub(rhs, Compiler.Builder);
                case TokenType.MULE:
                    return lhs.Mul(rhs, Compiler.Builder);
                case TokenType.DIVE:
                    return lhs.Div(rhs, Compiler.Builder);
                case TokenType.ASSIGN:
                    return rhs;
                default:
                    throw new NotSupportedException($"Operator {token} is not supported.");
            }
        }

        private PointerValue GetPointerValue(AstAssignmentOperator assignmentOperatorNode)
        {
            Compiler.Flag |= NodeVisitFlag.ASSIGNMENT_INDEX;
            Compiler.Visit(assignmentOperatorNode.IdentifierNode);
            IValue iValue = Compiler.StackIValuePop();
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

            var pointertype = targetPointer.GetBifyType() as BifyPointerType;
            if (pointertype.PointedType.ValueFlag.HasFlag(ValueFlag.CONSTANT))
            {
                new BifyTypeError("Assigning to constant variable!").Throw();
            }
            return targetPointer;
        }

        private BifyValue GetOperandValue(AstAssignmentOperator assignmentOperatorNode)
        {
            Compiler.Visit(assignmentOperatorNode.ValueNode);
            IValue iValue = Compiler.StackIValuePop();
            if (iValue is BifyType bifyType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"{bifyType.Name} cannot be used as type."));
                return null;
            }
            return (BifyValue)iValue;
        }
    }
}
