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
    struct ClassMethod
    {
        public string Name;
        public BifyFunction Function;
    }
    class ClassMethodManager
    {
        private AstClass classNode;
        private ClassType classType;
        AssemblyCompiler compiler = AssemblyCompiler.Instance;
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
                    methods.Add(HandleMethod(function));
            }
            return methods.ToArray();
        }
        public ClassMethod HandleMethod(AstFunctionDecl methodNode)
        {
            string functionName = methodNode.functionNameNode.Name;
            functionName = $"{classNode.NameNode.Token.Value}.{functionName}";
            compiler.Visit(methodNode.typeNode);
            IValue value = compiler.StackIValuePop();
            if (value is not BifyType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"{value.GetType().Name.ToLower()} cannot be used as type."));
                Environment.Exit(-1);
            }
            BifyType functionReturnType = (BifyType)value;
            compiler.ReturnType = functionReturnType;

            var functionArgs = new FunctionArgs(methodNode.argumentsNode);
            var functionPathChecker = new FunctionPathChecker(functionReturnType, methodNode.blockNode);

            functionArgs.PrependArgument("this", classType);

            var functionType = LLVMTypeRef.CreateFunction(
                functionReturnType.LLVMType,
                functionArgs.LLVMTypes
            );

            var function = compiler.Module.AddFunction(functionName, functionType);
            //var subroutineType = compiler.DebugBuilder.CreateSubroutineType(
            //        compiler.DebugBuilder.CreateParametersType(functionArgs.BifyTypes)
            //    );

            //var debugInfo = compiler.DebugBuilder.CreateFunctionDebugInfo(
            //    functionName, functionName, (uint)Traceback.Instance.Line,
            //    subroutineType
            //);
            //function.SetMetadata((uint)LLVMMetadataKind.LLVMDISubprogramMetadataKind,
            //    compiler.Context.Handle.MetadataAsValue(debugInfo));

            //compiler.ErrorBB = function.AppendBasicBlock("error");
            var entry = function.AppendBasicBlock("entry");
            compiler.Builder.PositionAtEnd(entry);

            compiler.VariableManager.EnterLocalScope();
            var bifyFunction = new BifyFunction(function, functionArgs, functionReturnType, functionType);
            compiler.VariableManager.RegisterGlobalVariable(functionName, bifyFunction);

            AddFunctionArgsToScope(bifyFunction);

            compiler.Visit(methodNode.blockNode);
            if (!functionPathChecker.AllPathsReturn && functionReturnType.CompareType(new VoidType()))
            {
                compiler.Builder.BuildRetVoid();
            }
            compiler.ClearStack();

            // Fix for Problem 2: Ensure a ClassMethod is returned
            return new ClassMethod
            {
                Name = functionName,
                Function = bifyFunction
            };
        }

        private unsafe void AddFunctionArgsToScope(BifyFunction function)
        {
            for (uint i = 0; i < function.FunctionArgs.ArgsNames.Length; i++)
            {
                BifyValue paramValue = function.FunctionArgs.BifyTypes[i].CreateValueRef(LLVM.GetParam(function.GetLLVMValue(), i));
                var alloca = compiler.Builder.BuildAlloca(paramValue.GetBifyType().LLVMType, function.FunctionArgs.ArgsNames[i]);
                compiler.Builder.BuildStore(paramValue.GetLLVMValue(), alloca);
                BifyValue allocaPointer = new AllocaType(paramValue.GetBifyType()).CreateValueRef(alloca);
                compiler.VariableManager.RegisterLocalVariable(function.FunctionArgs.ArgsNames[i], allocaPointer);
            }
        }

    }
}
