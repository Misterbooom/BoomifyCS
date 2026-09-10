using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers
{
    internal class FunctionDeclarationNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstFunctionDecl functionDeclNode = node as AstFunctionDecl;
            string functionName = functionDeclNode.FunctionNameNode.Name;
            Compiler.Visit(functionDeclNode.TypeNode);
            IValue value = Compiler.StackIValuePop();
            if (value is not BifyType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"{value.GetType().Name.ToLower()} cannot be used as type."));
                return;
            }
            BifyType functionReturnType = (BifyType)value;
            Compiler.ReturnType = functionReturnType;

            var functionArgs = new FunctionArgs(functionDeclNode.ArgumentsNode);
            var functionPathChecker = new FunctionPathChecker(functionReturnType, functionDeclNode.BlockNode);
            var functionType = LLVMTypeRef.CreateFunction(functionReturnType.LlvmType, functionArgs.LlvmTypes);


            var function = Compiler.Module.AddFunction(functionName, functionType);
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
            Compiler.SetFunctionEntryBb(entry);

            Compiler.Builder.PositionAtEnd(entry);

//            if (functionName == "main")
//            {
//#if DEBUG_COMPILE
//                CallNodeHandler.pushFrame.Call([
//                        new IntegerType().Create(Traceback.Instance.Line),
//                        new ConstStringType().Create(Traceback.Instance.FilePath)
//                    ]);
//#endif
//            }
            Compiler.VariableManager.EnterLocalScope();
            var bifyFunction = new BifyFunction(function, functionArgs, functionReturnType, functionType);
            FlagProcessor.SetFlags(FlagContext.FUNCTION, bifyFunction.GetBifyType(), ((AstFlag)functionDeclNode.FlagNode).Flags);

            Compiler.VariableManager.RegisterGlobalVariable(functionName, bifyFunction);

            AddFunctionArgsToScope(bifyFunction);



            if (!functionPathChecker.AllPathsReturn && functionReturnType.CompareType(new VoidType()))
            {
                AstBlock blockNode = (AstBlock)functionDeclNode.BlockNode;
                blockNode.ChildNodes.Add(new AstReturn(new Lexer.Token(Lexer.TokenType.RETURN,"return"), null));
            }
            Compiler.Visit(functionDeclNode.BlockNode);
           
            //function.VerifyFunction(LLVMVerifierFailureAction.LLVMAbortProcessAction);
            Compiler.VariableManager.ExitLocalScope();
            Compiler.ClearStack();

        }

        private void AddFunctionArgsToScope(BifyFunction function)
        {
            for (uint i = 0; i < function.FunctionArgs.ArgsNames.Length; i++)
            {

                unsafe
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
}
