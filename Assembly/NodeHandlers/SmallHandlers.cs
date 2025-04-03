using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class ModuleHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstModule moduleNode = (AstModule)node;
            foreach (AstNode child in moduleNode.ChildNodes)
            {
                compiler.Visit(child);
            }
        }
    }
    class BlockNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstBlock blockNode = (AstBlock)node;
            var locals = compiler.VariableManager.GetLocals();
            compiler.VariableManager.EnterLocalScope();
            compiler.VariableManager.SetCurrentLocalScope(locals);
            for (int i = 0; i < blockNode.ChildNodes.Count; i++)
            {
                AstNode child = blockNode.ChildNodes[i];
                compiler.NextNode = blockNode.ChildNodes.ElementAtOrDefault(i + 1);
                compiler.Visit(child);
                if (child is AstContinue || child is AstBreak || child is AstReturn)
                {
                    return;
                }
            }
            compiler.VariableManager.ExitLocalScope();
        }
    }
    class ArrayNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstArray astArray = (AstArray)node;
            BifyType bifyType = (BifyType)compiler.StackIValuePop();

            if (bifyType is not ArrayType arrayType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError("Invalid array type"));
                return;
            }
            uint argCount = CountArgs(astArray.ArgumentsNode);
            compiler.Visit(astArray.ArgumentsNode);

            List<BifyValue> bifyValues = new List<BifyValue>();
            for (int i = 0; i < argCount; i++)
            {
                bifyValues.Add(compiler.StackPop());
            }
            bifyValues.Reverse();
            ValidateArgsType(bifyValues.ToArray(), arrayType.ItemType);
            BifyDebug.Log(string.Join(", ", bifyValues));
            if (arrayType.ElementCount == 0)
            {
                arrayType.SetElementCount(argCount);
            }
            compiler.StackPush(arrayType.Create(bifyValues.ToArray()));

        }
        private static void ValidateArgsType(BifyValue[] values, BifyType expectedType)
        {
            for (int i = 0; i < values.Length; i++)
            {
                var providedArg = values[i];
                if (!expectedType.CompareType(providedArg.GetBifyType()))
                {
                    Traceback.Instance.Catch(typeof(BifyTypeError));
                    BifyValue castedArg = providedArg.AutoCast(expectedType, AssemblyCompiler.Instance.Builder);
                    if (castedArg == null || Traceback.Instance.GetError() != null)
                    {
                        string expectedTypeName = expectedType.Name;
                        string providedTypeName = providedArg.GetTypeName();
                        string errorMessage = $"Type mismatch at argument {i + 1}: Expected {expectedTypeName} but got {providedTypeName}.";
                        Traceback.Instance.ThrowException(new BifyTypeError(errorMessage));
                        return;
                    }
                    else
                    {
                        values[i] = castedArg;
                    }
                }
            }
        }
        private static uint CountArgs(AstNode node)
        {
            if (node == null) return 0;
            if (node is AstBinaryOp binaryOp && binaryOp.Token.Type == TokenType.COMMA)
            {
                return CountArgs(binaryOp.Left) + CountArgs(binaryOp.Right);
            }
            return 1;
        }
    }
    class IdentifierNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            var variable = compiler.VariableManager.GetVariable(node.Token.Value);

            if (variable is BifyType bifyType)
            {
                compiler.StackPush(bifyType);
                return;
            }
            else if (((BifyValue)variable).GetBifyType() is AllocaType allocaType)
            {
                LLVMValueRef valueRef = compiler.Builder.BuildLoad2(allocaType.PointedType.LLVMType, variable.GetLLVMValue(), "loaded_" + node.Token.Value);
                compiler.StackPush(allocaType.PointedType.CreateValueRef(valueRef));
            }
            else
            {
                compiler.StackPush(variable);
            }
        }
    }
    class ConstantNodeHandler : NodeHandler
    {
        public ConstantNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            unsafe
            {
                if (node is AstNumber astNumber)
                {


                    compiler.StackPush(new IntegerType().Create(astNumber.Value));

                }
                else if (node is AstFloat astFloat)
                {
                    compiler.StackPush(new FloatType().Create(astFloat.Value));
                }
                else if (node is AstString astString)
                {
                    if (astString.Token.Type == TokenType.STRING)
                        compiler.StackPush(new ConstStringType().Create((string)astString.Value));
                    else
                    {
                        if (astString.Token.Value.Length > 1)
                        {
                            Traceback.Instance.ThrowException(new BifyTypeError("Char type can only have one character"));
                            return;
                        }
                        compiler.StackPush(new CharType().Create(astString.Token.Value[0]));
                    }
                }
            }
        }
    }
}
