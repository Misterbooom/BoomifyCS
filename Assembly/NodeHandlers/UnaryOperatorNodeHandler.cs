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

            compiler.Visit(unaryOperator.Operand);
            IValue operandValue = compiler.StackIValuePop();

            if (operandValue is not BifyValue originalValue)
            {
                Traceback.Instance.ThrowException(new BifyTypeError("Unary operations can only be applied to runtime values."));
                return;
            }

            BifyValue targetPointer = compiler.VariableManager.GetBifyValue(unaryOperator.Operand.Token.Value);
            if (targetPointer == null || targetPointer.GetBifyType() is not AllocaType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"Cannot apply unary operator to non-pointer value '{targetPointer.GetTypeName()}'"));
                return;
            }

            BifyValue one = new IntegerType().Create(1);
            BifyValue newValue = unaryOperator.Token.Type switch
            {
                TokenType.INCREMENT => originalValue.Add(one, compiler.Builder),
                TokenType.DECREMENT => originalValue.Sub(one, compiler.Builder),
                _ => throw new NotImplementedException($"Unsupported unary operator '{unaryOperator.Token.Type}'")
            };

            compiler.Builder.BuildStore(newValue.GetLLVMValue(), targetPointer.GetLLVMValue());
            compiler.StackPush(newValue);
        }
    }
}
