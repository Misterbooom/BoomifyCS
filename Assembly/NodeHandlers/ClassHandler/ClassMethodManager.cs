using System;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers.ClassHandler
{
    class ClassMethodManager(AstClass classNode, ClassType classType)
    {
        AssemblyCompiler Compiler => AssemblyCompiler.Instance;

        public void AddMethodsToClass(ref ClassType classType)
        {
            foreach (AstNode child in ((AstBlock)classNode.BodyNode).ChildNodes)
            {
                if (child is AstFunctionDecl function)
                {
                    ClassMethod method = HandleMethod(function);
                    classType.AddMethod(method);
                }
            }
        }

       

        private ClassMethod HandleMethod(AstFunctionDecl methodNode)
        {
            string functionName = methodNode.FunctionNameNode.Name;

            Compiler.Visit(methodNode.TypeNode);
            IValue value = Compiler.StackIValuePop();
            if (value is not BifyType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"{value.GetType().Name.ToLower()} cannot be used as type."));
                Environment.Exit(-1);
            }

            BifyType functionReturnType = (BifyType)value;
            Compiler.ReturnType = functionReturnType;

            var functionArgs = new FunctionArgs(methodNode.ArgumentsNode);
            var functionPathChecker = new FunctionPathChecker(functionReturnType, methodNode.BlockNode);

            functionArgs.PrependArgument("this", classType);

            var functionType = LLVMTypeRef.CreateFunction(
                functionReturnType.LlvmType,
                functionArgs.LlvmTypes
            );

            var function = Compiler.Module.AddFunction($"{classNode.NameNode.Token.Value}.{functionName}", functionType);

            var entry = function.AppendBasicBlock("entry");
            Compiler.Builder.PositionAtEnd(entry);
            Compiler.SetFunctionEntryBb(entry);

            Compiler.VariableManager.EnterLocalScope();

            var bifyFunction = new BifyFunction(function, functionArgs, functionReturnType, functionType);
            FlagProcessor.SetFlags(FlagContext.METHOD, bifyFunction.GetBifyType(), methodNode.FlagNode.Flags);

            AddFunctionArgsToScope(bifyFunction);

            if (!functionPathChecker.AllPathsReturn && functionReturnType.CompareType(new VoidType()))
            {
                AstBlock blockNode = (AstBlock)methodNode.BlockNode;
                blockNode.ChildNodes.Add(new AstReturn(new Lexer.Token(Lexer.TokenType.RETURN, "return"), null));
            }

            Compiler.Visit(methodNode.BlockNode);
            Compiler.ClearStack();

            Compiler.VariableManager.ExitLocalScope();

            bifyFunction.IsMethod = true;
            return new ClassMethod(functionName, bifyFunction);
        }

        private unsafe void AddFunctionArgsToScope(BifyFunction function)
        {
            for (uint i = 0; i < function.FunctionArgs.ArgsNames.Length; i++)
            {
                BifyValue paramValue = function.FunctionArgs.BifyTypes[i].CreateValueRef(LLVM.GetParam(function.GetLlvmValue(), i));

                var alloca = Compiler.Builder.BuildAlloca(paramValue.GetBifyType().LlvmType, function.FunctionArgs.ArgsNames[i]);
                Compiler.Builder.BuildStore(paramValue.GetLlvmValue(), alloca);

                BifyValue allocaPointer = new AllocaType(paramValue.GetBifyType()).CreateValueRef(alloca);
                allocaPointer.ValueFlag = paramValue.ValueFlag;

                Compiler.VariableManager.RegisterLocalVariable(function.FunctionArgs.ArgsNames[i], allocaPointer);
            }
        }
    }
}
