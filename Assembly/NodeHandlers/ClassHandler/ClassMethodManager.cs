using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers.ClassHandler
{

    class ClassMethodManager
    {
        private AstClass classNode;
        private ClassType classType;
        AssemblyCompiler compiler => AssemblyCompiler.Instance;
        public ClassMethodManager(AstClass classNode, ClassType classType)
        {

            this.classNode = classNode;
            this.classType = classType;
        }
        public ClassMethod[] GetMethods()
        {
            List<ClassMethod> methods = [];
            foreach (AstNode child in ((AstBlock)classNode.BodyNode).ChildNodes)
            {
                if (child is AstFunctionDecl function)
                {
                    ClassMethod classMethod = HandleMethod(function);
                    if (methods.Any(m => m.Name == classMethod.Name))
                    {
                        ProcessMethodOverloading(classMethod,ref methods);
                    }
                    else
                    {
                        methods.Add(classMethod);
                    }
                }
            }
            return methods.ToArray();
        }
        private void ProcessMethodOverloading(ClassMethod newMethod, ref List<ClassMethod> existingMethods)
        {
            if (existingMethods.Any(newMethod.HasSameParameters)){
                BifyDebug.Log($"{newMethod.GetBifyFunction().FunctionArgs}");
                new BifyArgumentError($"Method '{newMethod.Name}' is overloaded with the same parameters.").Throw();
            }
            existingMethods.Add(newMethod);
        }
        
        public ClassMethod HandleMethod(AstFunctionDecl methodNode)
        {
            string functionName = methodNode.FunctionNameNode.Name;

            compiler.Visit(methodNode.TypeNode);
            IValue value = compiler.StackIValuePop();
            if (value is not BifyType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"{value.GetType().Name.ToLower()} cannot be used as type."));
                Environment.Exit(-1);
            }
            BifyType functionReturnType = (BifyType)value;
            compiler.ReturnType = functionReturnType;

            var functionArgs = new FunctionArgs(methodNode.ArgumentsNode);
            var functionPathChecker = new FunctionPathChecker(functionReturnType, methodNode.BlockNode);

            functionArgs.PrependArgument("this", classType);

            var functionType = LLVMTypeRef.CreateFunction(
                functionReturnType.LLVMType,
                functionArgs.LLVMTypes
            );

            var function = compiler.Module.AddFunction($"{classNode.NameNode.Token.Value}.{functionName}", functionType);
            //var subroutineType = compiler.DEBUG_COMPILEBuilder.CreateSubroutineType(
            //        compiler.DEBUG_COMPILEBuilder.CreateParametersType(functionArgs.BifyTypes)
            //    );

            //var DEBUG_COMPILEInfo = compiler.DEBUG_COMPILEBuilder.CreateFunctionDEBUG_COMPILEInfo(
            //    functionName, functionName, (uint)Traceback.Instance.Line,
            //    subroutineType
            //);
            //function.SetMetadata((uint)LLVMMetadataKind.LLVMDISubprogramMetadataKind,
            //    compiler.Context.Handle.MetadataAsValue(DEBUG_COMPILEInfo));

            //compiler.ErrorBB = function.AppendBasicBlock("error");
            var entry = function.AppendBasicBlock("entry");
            compiler.Builder.PositionAtEnd(entry);
            compiler.SetFunctionEntryBB(entry);

            compiler.VariableManager.EnterLocalScope();
            var bifyFunction = new BifyFunction(function, functionArgs, functionReturnType, functionType);
            bifyFunction.ValueFlag |= ValueFlag.Private;
            FlagProcessor.SetFlags(FlagContext.Method, bifyFunction.GetBifyType(), methodNode.FlagNode.Flags);

            AddFunctionArgsToScope(bifyFunction);
            if (!functionPathChecker.AllPathsReturn && functionReturnType.CompareType(new VoidType()))
            {
                AstBlock blockNode = (AstBlock)methodNode.BlockNode;
                blockNode.ChildNodes.Add(new AstReturn(new Lexer.Token(Lexer.TokenType.RETURN, "return"), null));
            }
            compiler.Visit(methodNode.BlockNode);

            compiler.ClearStack();
            BifyDebug.Log($"Variable manager before exit : {compiler.VariableManager}");

            compiler.VariableManager.ExitLocalScope();
            BifyDebug.Log($"Variable manager after exit : {compiler.VariableManager}");

            bifyFunction.IsMethod = true;
            return new ClassMethod(functionName, bifyFunction);
        }

        private unsafe void AddFunctionArgsToScope(BifyFunction function)
        {
            for (uint i = 0; i < function.FunctionArgs.ArgsNames.Length; i++)
            {
                BifyValue paramValue = function.FunctionArgs.BifyTypes[i].CreateValueRef(LLVM.GetParam(function.GetLLVMValue(), i));


                var alloca = compiler.Builder.BuildAlloca(paramValue.GetBifyType().LLVMType, function.FunctionArgs.ArgsNames[i]);
                compiler.Builder.BuildStore(paramValue.GetLLVMValue(), alloca);
                BifyValue allocaPointer = new AllocaType(paramValue.GetBifyType()).CreateValueRef(alloca);
                allocaPointer.ValueFlag = paramValue.ValueFlag;

                compiler.VariableManager.RegisterLocalVariable(function.FunctionArgs.ArgsNames[i], allocaPointer);
            }
        }

    }
}
