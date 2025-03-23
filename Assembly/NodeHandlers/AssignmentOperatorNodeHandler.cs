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
            AstAssignmentOperator assignmentOp = node as AstAssignmentOperator;

            compiler.Visit(assignmentOp.ValueNode);
            BifyValue rhsValue = compiler.StackPop();

            compiler.Flag |= NodeVisitFlag.DONT_LOAD_INDEX;
            compiler.Visit(assignmentOp.IdentifierNode);
            BifyValue lhsPointer = compiler.StackPop();

            if (lhsPointer.ValueFlag.HasFlag(ValueFlag.Constant))
            {
                Traceback.Instance.ThrowException(new BifyTypeError("Cannot assign to const variable"));
                return;
            }
            Console.WriteLine($"Variable Flag - {lhsPointer.ValueFlag}");

            BifyType targetValueType = lhsPointer.GetBifyType();
            if (targetValueType is BifyPointerType pointerType)
                targetValueType = pointerType.PointedType;

            rhsValue = rhsValue.AutoCast(targetValueType, compiler.Builder);
            BifyValue computedValue;
            BifyPointerType lhsPointerType = lhsPointer.GetBifyType() as BifyPointerType;
            BifyDebug.Log($"pointer type: {lhsPointerType.Name} {targetValueType.LLVMType}");
            BifyValue lhsLoadedValue = targetValueType.CreateValueRef(
                compiler.Builder.BuildLoad2(targetValueType.LLVMType, lhsPointer.GetLLVMValue(), "load_lhs"));

            switch (assignmentOp.Token.Type)
            {
                case TokenType.ADDE:
                    computedValue = lhsLoadedValue.Add(rhsValue, compiler.Builder);
                    break;
                case TokenType.SUBE:
                    computedValue = lhsLoadedValue.Sub(rhsValue, compiler.Builder);
                    break;
                case TokenType.MULE:
                    computedValue = lhsLoadedValue.Mul(rhsValue, compiler.Builder);
                    break;
                case TokenType.DIVE:
                    computedValue = lhsLoadedValue.Div(rhsValue, compiler.Builder);
                    break;
                case TokenType.ASSIGN:
                    computedValue = targetValueType.CreateValueRef(rhsValue.GetLLVMValue());
                    break;
                default:
                    throw new NotImplementedException($"{assignmentOp.Token}");
            }
            BifyDebug.Log($"Computed Value - {computedValue}");
            compiler.Builder.BuildStore(computedValue.GetLLVMValue(), lhsPointer.GetLLVMValue());
        }
    }
}
