using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class CallNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstCall astCall = (AstCall)node;
            int expectedCount = CountArgs(astCall.ArgumentsNode);
            compiler.Visit(astCall.ArgumentsNode);
            string callableName = astCall.CallableName.Token.Value;
            Variable callableVar = compiler.variableManager.GetVariable(callableName);
            if (callableVar.BifyObject == null || callableVar.BifyObject is not BifyFunction)
            {
                Traceback.Instance.ThrowException(new BifyCastError($"{callableName} is not callable"));
                return;
            }
            BifyFunction bifyFunction = callableVar.BifyObject as BifyFunction;
            bifyFunction.LLVMBuild();
            List<BifyValue> bifyValues = [];
            for (int i = 0; i < expectedCount; i++)
            {
                if (compiler.stack.Peek() == null)
                {
                    throw new NullReferenceException("Expected value on stack");
                }
                bifyValues.Add(compiler.stack.Pop());
            }
            bifyValues.Reverse();
            CheckArguments(bifyValues,bifyFunction.ArgumentsType,bifyFunction.isVariadic);
            List<LLVMValueRef> valueRefs = [];
            foreach (BifyValue bifyValue in bifyValues)
            {
                valueRefs.Add(bifyValue.GetValueRef());
            }
            unsafe
            {
                compiler.builder.BuildCall2(
                 bifyFunction.functionType,
                 bifyFunction.functionValue,
                valueRefs.ToArray(),
                callableVar.LlvmType != LLVMTypeRef.Void ? "callTemp" : "");
            }

        }
        private static int CountArgs(AstNode node)
        {
            if (node == null)
            {
                return 0;
            }

            if (node is AstBinaryOp binaryOp && binaryOp.Token.Type == TokenType.COMMA)
            {
                return CountArgs(binaryOp.Left) + CountArgs(binaryOp.Right);
            }

            return 1;
        }
        private void CheckArguments(List<BifyValue> inputArgs, List<Type> targetArgs, bool isVariadic)
        {
            if (!isVariadic && inputArgs.Count != targetArgs.Count)
            {
                Traceback.Instance.ThrowException(new BifyArgumentError($"Expected {targetArgs.Count} arguments but got {inputArgs.Count}"));
            }
            else if (isVariadic && inputArgs.Count < targetArgs.Count)
            {
                Traceback.Instance.ThrowException(new BifyArgumentError($"Expected at least {targetArgs.Count} arguments but got {inputArgs.Count}"));
            }

            for (int i = 0; i < targetArgs.Count; i++)
            {
                if (inputArgs[i].GetBifyObject().GetType() != targetArgs[i])
                {
                    Traceback.Instance.ThrowException(
                        new BifyCastError($"Expected {targetArgs[i].Name.Replace("Bify","")} but got {inputArgs[i].GetBifyObject().GetName()} on index {i + 1}")
                    );
                }
            }
        }

    }
}