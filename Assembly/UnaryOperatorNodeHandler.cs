using System;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class UnaryOperatorNodeHandler : NodeHandler
    {
        public UnaryOperatorNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            AstUnaryOperator unaryOperator = (AstUnaryOperator)node;

            if (unaryOperator.Token.Type == TokenType.POINTER)
            {
                compiler.Visit(unaryOperator.Operand);
                IValue value = compiler.StackIValuePop();

                if (value is BifyType bifyType)
                {
                    compiler.StackPush(new BifyPointerType(bifyType));
                }
                else if (value is PointerValue pointer)
                {
                    compiler.StackPush(pointer.Dereference());
                }
                else
                {
                    Traceback.Instance.ThrowException(new BifyTypeError($"Cannot dereference {value.GetType().Name.ToLower()}"));
                }
                return;
            }

            compiler.Flag |= NodeVisitFlag.ASSIGNMENT_INDEX;
            compiler.Visit(unaryOperator.Operand);
            IValue operandValue = compiler.StackIValuePop();
            if (operandValue is AllocaPointer alloca)
            {
                HandleUnaryWithVariable(unaryOperator.Token.Type, alloca, unaryOperator.IsPrefix);
            }
        }
        private void HandleUnaryWithVariable(TokenType tokenType, AllocaPointer allocaPointer, bool isPrefix)
        {
            BifyValue loadedValue = allocaPointer.Dereference();
            BifyValue result = CalculateResult(tokenType, loadedValue);
            compiler.Builder.BuildStore(result.GetLLVMValue(), allocaPointer.GetLLVMValue());
            if (isPrefix)
            {
                compiler.StackPush(result);
            }
            else
            {
                compiler.StackPush(loadedValue);
            }
        }
        private BifyValue CalculateResult(TokenType tokenType, BifyValue operandValue)
        {
            BifyValue one = new IntegerType().Create(1);
            switch (tokenType)
            {
                case TokenType.INCREMENT:
                    return operandValue.Add(one, compiler.Builder);
                case TokenType.DECREMENT:
                    return operandValue.Sub(one, compiler.Builder);
                default:
                    Traceback.Instance.ThrowException(new BifyTypeError($"Invalid unary operator '{tokenType}'"));
                    return null;
            }
        }
    }
}
