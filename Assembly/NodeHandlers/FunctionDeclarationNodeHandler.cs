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
                functionName,functionName,(uint)Traceback.Instance.Line,
                subroutineType
            );
            function.SetMetadata((uint)LLVMMetadataKind.LLVMDISubprogramMetadataKind,
                compiler.Context.Handle.MetadataAsValue(debugInfo));

            //compiler.ErrorBB = function.AppendBasicBlock("error");
            var entry = function.AppendBasicBlock("entry");


            compiler.Builder.PositionAtEnd(entry);
            compiler.VariableManager.EnterLocalScope();
            var bifyFunction = new BifyFunction(function, functionArgs, functionReturnType, functionType);
            compiler.VariableManager.RegisterGlobalVariable(functionName, bifyFunction);

            SetFunctionArgsName(function, functionArgs.ArgsNames);
            AddFunctionArgsToScope(bifyFunction);

           


            compiler.Visit(functionDeclNode.blockNode);
            if (!functionPathChecker.AllPathsReturn && functionReturnType.CompareType(new VoidType()))
            {
                compiler.Builder.BuildRetVoid();
            }
            //function.VerifyFunction(LLVMVerifierFailureAction.LLVMAbortProcessAction);
            compiler.VariableManager.ExitLocalScope();

        }
        private void SetFunctionArgsName(LLVMValueRef function, string[] argsNames)
        {
            for (uint i = 0; i < function.ParamsCount; i++)
            {
                unsafe
                {
                    byte[] nameBytes = Encoding.ASCII.GetBytes(argsNames[i] + "\0");
                    fixed (byte* pName = nameBytes)
                    {
                        LLVM.SetValueName(function.GetParam(i), (sbyte*)pName);
                    }
                }
            }
        }
        private void AddFunctionArgsToScope(BifyFunction function)
        {
            for (uint i = 0; i < function.FunctionArgs.ArgsNames.Length; i++)
            {
                unsafe
                {
                    BifyValue value = function.FunctionArgs.BifyTypes[i].CreateValueRef(LLVM.GetParam(function.GetLLVMValue(), i));
                    BifyDebug.Log($"Arg val: {value}");
                    //BifyValue pointerValue = new AllocaType(value.GetBifyType()).CreateValueRef(value.GetLLVMValue());
                    compiler.VariableManager.RegisterLocalVariable(function.FunctionArgs.ArgsNames[i], value);
                }
            }
        }
    }
}
