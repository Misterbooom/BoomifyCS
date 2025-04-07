using System;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class VariableDeclarationNodeHandler : NodeHandler
    {
        public VariableDeclarationNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            var varDeclNode = (AstVarDecl)node;
            var varName = varDeclNode.AssignmentNode.Left.Token.Value;

            compiler.Visit(varDeclNode.Type);
            var value = compiler.StackIValuePop();

            if (value is not BifyType bifyType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"{((BifyValue)value).GetTypeName()} cannot be used as type."));
                return;
            }

            if (bifyType is ArrayType)
            {
                compiler.StackPush(bifyType);
            }

            compiler.Visit(varDeclNode.AssignmentNode.Right);
            var loadedIValue = compiler.StackIValuePop();

            if (loadedIValue is  BifyType incorrectValue)
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"{incorrectValue.Name} cannot be used as variable value."));
                return;
            }

            var loadedValue = loadedIValue as BifyValue;
            BifyDebug.Log($"Var type - {bifyType} Var value type - {loadedValue.GetTypeName()} LLVMVarType: {bifyType.LLVMType} pointer to i8");

            var variableValue = loadedValue.AutoCast(bifyType, compiler.Builder);
            var alloca = compiler.Builder.BuildAlloca(bifyType.LLVMType, varName);

            compiler.Builder.BuildStore(variableValue.GetLLVMValue(), alloca);
            var allocaPointer = new AllocaType(bifyType).CreateValueRef(alloca);

            if (varDeclNode.Flag?.Token.Type == TokenType.CONST)
            {
                allocaPointer.ValueFlag = ValueFlag.Constant;
            }

            allocaPointer.ValueFlag |= ValueFlag.Variable;
            compiler.VariableManager.RegisterLocalVariable(varName, allocaPointer);
        }
    }
}

