using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class FunctionDeclarationNodeHandler : NodeHandler
    {
        public FunctionDeclarationNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            AstFunctionDecl functionDeclNode = node as AstFunctionDecl;
            string functionName = functionDeclNode.functionNameNode.Name;
            compiler.Visit(functionDeclNode.typeNode);
            IValue value = compiler.StackIValuePop();
            if (value is not BifyType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"{value.GetType().Name.ToLower()} cannot be used as type."));
                return;
            }
            BifyType functionReturnType = (BifyType)value;
            compiler.ReturnType = functionReturnType;

            var functionArgs = new FunctionArgs(functionDeclNode.argumentsNode);
            var functionPathChecker = new FunctionPathChecker(functionReturnType, functionDeclNode.blockNode);
            var functionType = LLVMTypeRef.CreateFunction(functionReturnType.LLVMType, functionArgs.LLVMTypes);


            var function = compiler.Module.AddFunction(functionName, functionType);
            var subroutineType = compiler.DebugBuilder.CreateSubroutineType(
                    compiler.DebugBuilder.CreateParametersType(functionArgs.BifyTypes)
                );

            var debugInfo = compiler.DebugBuilder.CreateFunctionDebugInfo(
                functionName, functionName, (uint)Traceback.Instance.Line,
                subroutineType
            );
            function.SetMetadata((uint)LLVMMetadataKind.LLVMDISubprogramMetadataKind,
                compiler.Context.Handle.MetadataAsValue(debugInfo));

            //compiler.ErrorBB = function.AppendBasicBlock("error");
            var entry = function.AppendBasicBlock("entry");
            compiler.Builder.PositionAtEnd(entry);

            if (functionName == "main")
            {
                CallNodeHandler.pushFrame.Call([
                        new IntegerType().Create(Traceback.Instance.Line),
                        new ConstStringType().Create(Traceback.Instance.FilePath)
                    ]);
            }
            compiler.VariableManager.EnterLocalScope();
            var bifyFunction = new BifyFunction(function, functionArgs, functionReturnType, functionType);
            compiler.VariableManager.RegisterGlobalVariable(functionName, bifyFunction);

            AddFunctionArgsToScope(bifyFunction);




            compiler.Visit(functionDeclNode.blockNode);
            if (!functionPathChecker.AllPathsReturn && functionReturnType.CompareType(new VoidType()))
            {
                compiler.Builder.BuildRetVoid();
            }
            //function.VerifyFunction(LLVMVerifierFailureAction.LLVMAbortProcessAction);
            compiler.VariableManager.ExitLocalScope();
            compiler.ClearStack();

        }

        private void AddFunctionArgsToScope(BifyFunction function)
        {
            for (uint i = 0; i < function.FunctionArgs.ArgsNames.Length; i++)
            {
                unsafe
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
}
