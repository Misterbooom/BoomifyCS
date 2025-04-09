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
using System.Numerics;
using BoomifyCS.Assembly.Builtin;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class CallNodeHandler : NodeHandler
    {
        public static BifyFunction pushFrame = StdC.DeclarFunction("pushFrame",
           [new BifyObject.IntegerType(), new BifyObject.ConstStringType()], new VoidType()
           );
        public static BifyFunction popFrame = StdC.DeclarFunction("popFrame",
          [new BifyObject.IntegerType(), new BifyObject.ConstStringType()], new VoidType()
          );
        public CallNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            if (node is AstCall callNode)
            {
                string callableName = callNode.CallableName.Token.Value;
                if (compiler.VariableManager.GetVariable(callableName) is BifyFunction callable)
                {
                    if (callNode.ArgumentsNode != null)
                        compiler.Visit(callNode.ArgumentsNode);
                    List<BifyValue> providedArgs = new List<BifyValue>();

                    for (int i = 0; i < CountArgs(callNode.ArgumentsNode); i++)
                    {
                        BifyValue arg = compiler.StackPop();
                        providedArgs.Add(arg);
                    }
                    providedArgs.Reverse();


                    ValidateAndAutoCastArguments(providedArgs, callable.FunctionArgs.BifyTypes, callable.IsVariadic);

                    pushFrame.Call([
                        new IntegerType().Create(Traceback.Instance.Line),
                        new ConstStringType().Create(Traceback.Instance.FilePath)
                        ]);
                    var call = callable.Call(providedArgs.ToArray());
                    compiler.StackPush(callable.ReturnType.CreateValueRef(call.GetLLVMValue()));
                    popFrame.Call([]);
                }
                else
                {
                    Traceback.Instance.ThrowException(new BifyNameError($"Function '{callableName}' is not defined."));
                }
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError("Expected an AstCall node."));
            }
        }

        private static int CountArgs(AstNode node)
        {
            if (node == null) return 0;
            if (node is AstBinaryOp binaryOp && binaryOp.Token.Type == TokenType.COMMA)
            {
                return CountArgs(binaryOp.Left) + CountArgs(binaryOp.Right);
            }
            return 1;
        }

        private void ValidateAndAutoCastArguments(List<BifyValue> providedArgs, BifyType[] expectedArgsType, bool isVariadic)
        {
            if (!isVariadic && providedArgs.Count != expectedArgsType.Length)
            {
                Traceback.Instance.ThrowException(new BifyArgumentError($"Expected {expectedArgsType.Length} arguments but got {providedArgs.Count}."));
                return;
            }
            else if (isVariadic && providedArgs.Count < expectedArgsType.Length)
            {
                Traceback.Instance.ThrowException(new BifyArgumentError($"Expected at least {expectedArgsType.Length} arguments but got {providedArgs.Count}."));
                return;
            }

            for (int i = 0; i < expectedArgsType.Length; i++)
            {
                BifyType expectedType = expectedArgsType[i];
                BifyValue providedArg = providedArgs[i];

                if (!expectedType.CompareType(providedArg.GetBifyType()))
                {
                    Traceback.Instance.Catch(typeof(BifyTypeError));
                    BifyValue castedArg = providedArg.ExplicitCast(expectedType, compiler.Builder);
                    BifyDebug.Log($"Arg {i}: {castedArg}");

                    if (castedArg == null || Traceback.Instance.GetError() != null)
                    {
                        string expectedTypeName = expectedType.Name;
                        string providedTypeName = providedArg.GetTypeName();
                        string errorMessage = $"Type mismatch at argument {i + 1}: Expected {expectedTypeName} but got {providedTypeName}, and auto-casting failed.";
                        Traceback.Instance.ThrowException(new BifyTypeError(errorMessage));
                        return;
                    }
                    else
                    {
                        providedArgs[i] = castedArg;
                    }
                }
                BifyDebug.Log($"Arg {i}: {providedArg}");

            }
        }
    }

}
