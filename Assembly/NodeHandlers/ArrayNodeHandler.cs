using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class ArrayNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstArray astArray = (AstArray)node;
            uint argCount = CountArgs(astArray.ArgumentsNode);

            if (argCount == 0 && Compiler.StackCount == 0)
            {
                Traceback.Instance.ThrowException(new BifyTypeError("Cannot construct an array: no type provided."));
                return;
            }

            ArrayType arrayType;
            if (Compiler.StackCount == 0)
            {
                Compiler.Visit(astArray.ArgumentsNode);
                IValue rawTypeValue = Compiler.StackElementAt(Compiler.StackCount - 1);
                if (rawTypeValue is not BifyValue typeValue)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError("Cannot construct an array: no type provided."));
                    return;
                }
                arrayType = new ArrayType(typeValue.GetBifyType(), 0);
            }
            else
            {
                BifyType typeOnStack = (BifyType)Compiler.StackIValuePop();
                if (typeOnStack is not ArrayType declaredArrayType)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError(
                        "Cannot construct an array: the type provided is not an array type. " +
                        "Make sure to define the array type correctly before initialization."));
                    return;
                }
                arrayType = declaredArrayType;
                Compiler.Visit(astArray.ArgumentsNode);
            }

            BifyValue[] values = BuildArrayFromStack(argCount, arrayType);
            ValidateArgsType(values, arrayType.ItemType);

            if (arrayType.ElementCount == 0)
            {
                arrayType.SetElementCount(argCount);
            }

            Compiler.StackPush(arrayType.Create(values));
        }

        private BifyValue[] BuildArrayFromStack(uint argCount, ArrayType arrayType)
        {
            BifyValue[] values = new BifyValue[argCount];
            for (int i = (int)argCount - 1; i >= 0; i--)
            {
                IValue value = Compiler.StackIValuePop();
                if (value is BifyType bifyType)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError(
                        $"Type mismatch at argument {argCount - i}: Expected {arrayType.ItemType.Name} but got {bifyType.Name}."));
                    return [];
                }
                BifyValue bifyValue = (BifyValue)value;

                if (!arrayType.ItemType.CompareType(bifyValue.GetBifyType()))
                {
                    BifyValue casted = bifyValue.ExplicitCast(arrayType.ItemType, AssemblyCompiler.Instance.Builder);
                    if (casted == null || Traceback.Instance.GetError() != null)
                    {
                        Traceback.Instance.ThrowException(new BifyTypeError(
                            $"Type mismatch at argument {argCount - i}: Expected {arrayType.ItemType.Name} but got {bifyValue.GetTypeName()}."));
                        return [];
                    }
                    bifyValue= casted;
                }
                values[i] = bifyValue;
            }
            return values;
        }

        private static void ValidateArgsType(BifyValue[] values, BifyType expectedType)
        {
            for (int i = 0; i < values.Length; i++)
            {
                BifyValue arg = values[i];
                if (!expectedType.CompareType(arg.GetBifyType()))
                {
                    Traceback.Instance.Catch(typeof(BifyTypeError));
                    BifyValue casted = arg.ExplicitCast(expectedType, AssemblyCompiler.Instance.Builder);
                    if (casted == null || Traceback.Instance.GetError() != null)
                    {
                        Traceback.Instance.ThrowException(new BifyTypeError(
                            $"Array element type mismatch at index {i}: expected '{expectedType.Name}', but got '{arg.GetTypeName()}'."));
                        return;
                    }
                    values[i] = casted;
                }
            }
        }

        private static uint CountArgs(AstNode node)
        {
            if (node == null) return 0;
            if (node is AstBinaryOp binaryOp && binaryOp.Token.Type == TokenType.COMMA)
                return CountArgs(binaryOp.Left) + CountArgs(binaryOp.Right);
            return 1;
        }
    }
}
