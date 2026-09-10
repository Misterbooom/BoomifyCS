using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Assembly.NodeHandlers
{
    internal class UnaryOperatorNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstUnaryOperator unaryOperator = (AstUnaryOperator)node;

            if (unaryOperator.Token.Type == TokenType.POINTER)
            {
                Compiler.Visit(unaryOperator.Operand);
                IValue value = Compiler.StackIValuePop();

                if (value is BifyType bifyType)
                {
                    Compiler.StackPush(new BifyPointerType(bifyType));
                }
                else if (value is PointerValue pointer)
                {
                    Compiler.StackPush(pointer.Dereference(true));
                }
                else
                {
                    Traceback.Instance.ThrowException(new BifyTypeError($"Cannot dereference {value.GetType().Name.ToLower()}"));
                }
                return;
            }

            Compiler.Flag |= NodeVisitFlag.ASSIGNMENT_INDEX;
            Compiler.Visit(unaryOperator.Operand);
            IValue operandValue = Compiler.StackIValuePop();
            if (operandValue is AllocaPointer alloca)
            {
                HandleUnaryWithVariable(unaryOperator.Token.Type, alloca, unaryOperator.IsPrefix);
            }
        }
        private void HandleUnaryWithVariable(TokenType tokenType, AllocaPointer allocaPointer, bool isPrefix)
        {
            BifyValue loadedValue = allocaPointer.Dereference(false);
            BifyValue result = CalculateResult(tokenType, loadedValue);
            Compiler.Builder.BuildStore(result.GetLlvmValue(), allocaPointer.GetLlvmValue());
            if (isPrefix)
            {
                Compiler.StackPush(result);
            }
            else
            {
                Compiler.StackPush(loadedValue);
            }
        }
        private BifyValue CalculateResult(TokenType tokenType, BifyValue operandValue)
        {
            BifyValue one = new IntegerType().Create(1);
            switch (tokenType)
            {
                case TokenType.INCREMENT:
                    return operandValue.Add(one, Compiler.Builder);
                case TokenType.DECREMENT:
                    return operandValue.Sub(one, Compiler.Builder);
                default:
                    Traceback.Instance.ThrowException(new BifyTypeError($"Invalid unary operator '{tokenType}'"));
                    return null;
            }
        }
    }
}
