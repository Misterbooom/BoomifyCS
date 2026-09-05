using System;
using System.Collections.Generic;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using BoomifyCS.Exceptions;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Assembly.Builtin;
using System.Linq;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class CallNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public static readonly BifyFunction PushFrame = StdC.DeclarFunction("pushFrame",
            new BifyType[] { new IntegerType(), new ConstStringType() }, new VoidType());
        public static readonly BifyFunction PopFrame = StdC.DeclarFunction("popFrame",
            [], new VoidType());

        public override void HandleNode(AstNode node)
        {
            if (node is AstCall callNode)
            {
                Compiler.Visit(callNode.CallableName);
                IValue callableIValue = Compiler.StackIValuePop();
                if (callableIValue == null)
                {
                    throw new InvalidOperationException("Callable value is null. This should not happen.");
                }

                if (callableIValue is BifyFunction callable)
                {
                    HandleFunctionCall(callNode, callable);
                }
                else if (callableIValue is BifyMethodRef methodRef)
                {
                    if (callNode.ArgumentsNode != null)
                        Compiler.Visit(callNode.ArgumentsNode);

                    List<BifyValue> providedArgs = GetArguments(CountArgs(callNode.ArgumentsNode));
                    BifyFunction resolvedMethod = methodRef.Resolve(providedArgs.Select(i => i.GetBifyType()).ToArray());

                    providedArgs = providedArgs.Prepend(resolvedMethod.ParentClass).ToList();

                    HandleFunctionCall(callNode, resolvedMethod,providedArgs);
                }
                else
                {

                    Traceback.Instance.ThrowException(new BifyTypeError($"Cannot call a non-callable type '{((BifyValue)callableIValue).GetTypeName()}'."));
                }
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError("Expected an AstCall node."));
            }
        }
        private void HandleFunctionCall(AstCall callNode, BifyFunction callable)
        {
            if (callNode.ArgumentsNode != null)
                Compiler.Visit(callNode.ArgumentsNode);

            List<BifyValue> providedArgs = GetArguments(CountArgs(callNode.ArgumentsNode));
            if (callable.IsMethod)
            {
                providedArgs = providedArgs.Prepend(callable.ParentClass).ToList();
            }
            HandleFunctionCall(callNode, callable, providedArgs);
        }
        private void HandleFunctionCall(AstCall callNode, BifyFunction callable, List<BifyValue> providedArgs)
        {

            ValidateAndAutoCastArguments(providedArgs, callable.FunctionArgs.BifyTypes, callable.IsVariadic, callable.IsMethod);
#if (DEBUG_COMPILE)
            PushFrame.Call(new BifyValue[]
            {
                        new BifyObject.IntegerType().Create(Traceback.Instance.Line),
                        new ConstStringType().Create(Traceback.Instance.FilePath)
            });
#endif
            Console.WriteLine($"Function: {callable.GetType()}");
            foreach (var arg in providedArgs)
            {
                Console.WriteLine($"Arg: {arg}");
            }
            var call = callable.Call(providedArgs.ToArray());
            call.GetLlvmValue().Name = "callRet";
            Compiler.StackPush(callable.ReturnType.CreateValueRef(call.GetLlvmValue()));
#if (DEBUG_COMPILE)
            PopFrame.Call(new BifyValue[0]);
#endif
        }
        //private void HandleCast(AstCall callNode,BifyType callableType)
        //{
        //    if (callNode.ArgumentsNode != null)
        //        compiler.Visit(callNode.ArgumentsNode);
        //    List<BifyValue> providedArgs = GetArguments(CountArgs(callNode.ArgumentsNode));
        //    if (providedArgs.Count > 1)
        //    {
        //        Traceback.Instance.ThrowException(new BifyArgumentError($"Expected 1 argument but got {providedArgs.Count}."));
        //        return;
        //    }

        //    BifyValue castedValue = providedArgs[0].ExplicitCast(callableType, compiler.Builder);
        //    compiler.StackPush(castedValue);
        //}
        public List<BifyValue> GetArguments(int count)
        {
            List<BifyValue> providedArgs = new List<BifyValue>();
            for (int i = 0; i < count; i++)
            {
                IValue argIValue = Compiler.StackIValuePop();
                if (argIValue is BifyType type)
                {
                    providedArgs.Add(new TypeValue(type));
                    continue;
                }
                BifyValue arg = (BifyValue)argIValue;
                providedArgs.Add(arg);
            }
            providedArgs.Reverse();
            return providedArgs;
        }
        public static int CountArgs(AstNode node)
        {
            if (node == null) return 0;
            if (node is AstBinaryOp binaryOp && binaryOp.Token.Type == TokenType.COMMA)
            {
                return CountArgs(binaryOp.Left) + CountArgs(binaryOp.Right);
            }
            return 1;
        }

        public void ValidateAndAutoCastArguments(List<BifyValue> providedArgs, BifyType[] expectedArgsType, bool isVariadic, bool isMethod)
        {
            int decrease = isMethod ? 1 : 0;
            if (!isVariadic && providedArgs.Count != expectedArgsType.Length)
            {
                Traceback.Instance.ThrowException(new BifyArgumentError($"Expected {expectedArgsType.Length - decrease} arguments but got {providedArgs.Count - decrease}."));
                return;
            }
            else if (isVariadic && providedArgs.Count < expectedArgsType.Length)
            {
                Traceback.Instance.ThrowException(new BifyArgumentError($"Expected at least {expectedArgsType.Length - decrease} arguments but got {providedArgs.Count - decrease}."));
                return;
            }
            for (int i = 0; i < expectedArgsType.Length; i++)
            {
                BifyType expectedType = expectedArgsType[i];
                BifyValue providedArg = providedArgs[i];
                if (providedArg == null)
                {
                    throw new ArgumentException($"Argument {i + 1} is null.");
                }
                if (expectedType == null)
                {
                    throw new ArgumentException($"Expected type for argument {i + 1} is null.");
                }

                if (!expectedType.CompareType(providedArg.GetBifyType()))
                {
                    Traceback.Instance.Catch(typeof(BifyTypeError));
                    BifyValue castedArg = providedArg.ExplicitCast(expectedType, Compiler.Builder);
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
            }
        }
    }
}
