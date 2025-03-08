using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using LLVMSharp.Interop;
using BoomifyCS.Exceptions;
using BoomifyCS.Assembly.BifyObject;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class CallNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstCall callNode = node as AstCall;
            string callableName = callNode.CallableName.Token.Value;
            BifyFunction callable = compiler.variableManager.GetVariable(callableName) as BifyFunction;
            compiler.Visit(callNode.ArgumentsNode);
            List<BifyValue> providedArgs = new List<BifyValue>();
            for (int i = 0; i < CountArgs(callNode.ArgumentsNode); i++)
            {
                BifyValue arg = compiler.stack.Pop();
                providedArgs.Add(arg);
            }
            providedArgs.Reverse();
            BifyDebug.Log($"Type: {callable.TypeRef}, callable: {callable.GetLLVMValue()}, ");

            ValidateArguments([.. providedArgs], callable.FunctionArgs.BifyTypes, callable.IsVariadic);

            var call = callable.Call(providedArgs.ToArray());
            compiler.stack.Push(callable.ReturnType.CreateByValueRef(call.GetLLVMValue()));



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

        private void ValidateArguments(BifyValue[] providedArgs, BifyType[] expectedArgsType, bool isVariadic)
        {
            if (!isVariadic && providedArgs.Length != expectedArgsType.Length)
            {
                Traceback.Instance.ThrowException(new BifyArgumentError($"Expected {providedArgs.Length} arguments but got {expectedArgsType.Length}"));
                return;
            }
            else if (isVariadic && providedArgs.Length < expectedArgsType.Length)
            {
                Traceback.Instance.ThrowException(new BifyArgumentError($"Expected at least {expectedArgsType.Length} arguments but got {providedArgs.Length}"));
                return;
            }
            for (int i = 0; i < expectedArgsType.Length; i++)
            {
                if (!expectedArgsType[i].CompareType(providedArgs[i]))
                {
                    string expectedTypeName = expectedArgsType[i].GetTypeName();
                    string providedTypeName = providedArgs[i].GetTypeName();
                    string errorMessage = $"Type mismatch at argument {i + 1}: Expected {expectedTypeName} but got {providedTypeName}.";
                    Traceback.Instance.ThrowException(new BifyTypeError(errorMessage));
                }
            }

    }
}
}
