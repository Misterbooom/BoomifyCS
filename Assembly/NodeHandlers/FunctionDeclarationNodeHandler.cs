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
            string functionName = functionDeclNode.FunctionNameNode.Name;
            compiler.Visit(functionDeclNode.TypeNode);
            IValue value = compiler.StackIValuePop();
            if (value is not BifyType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"{value.GetType().Name.ToLower()} cannot be used as type."));
                return;
            }
            BifyType functionReturnType = (BifyType)value;
            compiler.ReturnType = functionReturnType;

            var functionArgs = new FunctionArgs(functionDeclNode.ArgumentsNode);
            var functionPathChecker = new FunctionPathChecker(functionReturnType, functionDeclNode.BlockNode);
            var functionType = LLVMTypeRef.CreateFunction(functionReturnType.LLVMType, functionArgs.LLVMTypes);


            var function = compiler.Module.AddFunction(functionName, functionType);
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
            compiler.SetFunctionEntryBB(entry);

            compiler.Builder.PositionAtEnd(entry);

//            if (functionName == "main")
//            {
//#if DEBUG_COMPILE
//                CallNodeHandler.pushFrame.Call([
//                        new IntegerType().Create(Traceback.Instance.Line),
//                        new ConstStringType().Create(Traceback.Instance.FilePath)
//                    ]);
//#endif
//            }
            compiler.VariableManager.EnterLocalScope();
            var bifyFunction = new BifyFunction(function, functionArgs, functionReturnType, functionType);
            FlagProcessor.SetFlags(FlagContext.Function, bifyFunction.GetBifyType(), ((AstFlag)functionDeclNode.FlagNode).Flags);

            compiler.VariableManager.RegisterGlobalVariable(functionName, bifyFunction);

            AddFunctionArgsToScope(bifyFunction);



            if (!functionPathChecker.AllPathsReturn && functionReturnType.CompareType(new VoidType()))
            {
                AstBlock blockNode = (AstBlock)functionDeclNode.BlockNode;
                blockNode.ChildNodes.Add(new AstReturn(new Lexer.Token(Lexer.TokenType.RETURN,"return"), null));
            }
            compiler.Visit(functionDeclNode.BlockNode);
           
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
                    allocaPointer.ValueFlag = paramValue.ValueFlag;
                    compiler.VariableManager.RegisterLocalVariable(function.FunctionArgs.ArgsNames[i], allocaPointer);
                }
            }
        }
    }
}
