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
            AstVarDecl varDeclNode = (AstVarDecl)node;

            string varName = varDeclNode.AssignmentNode.Left.Token.Value;
            compiler.Visit(varDeclNode.Type);


            IValue value = compiler.StackIValuePop();
            if (value is not BifyType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"{((BifyValue)value).GetTypeName()} cannot be used as type."));
                return;
            }
            BifyType bifyType = (BifyType)value;
            if (bifyType is ArrayType)
            {
                compiler.StackPush(bifyType);
            }
            compiler.Visit(varDeclNode.AssignmentNode.Right);
            BifyValue loadedValue = compiler.StackPop();

            BifyDebug.Log($"Var type - {bifyType} Var value type - {loadedValue.GetTypeName()} LLVMVarType: {bifyType.LLVMType} pointer to i8");

            BifyValue variableValue = loadedValue.AutoCast(bifyType, compiler.Builder);



            var alloca = compiler.Builder.BuildAlloca(bifyType.LLVMType, varName);

            compiler.Builder.BuildStore(variableValue.GetLLVMValue(), alloca);
            var allocaPointer = new AllocaType(bifyType).CreateValueRef(alloca);
            //BifyDebug.Log($"Testing pointer tpye; {new AllocaType(bifyType).PointedType}");
            if (varDeclNode.Flag?.Token.Type == TokenType.CONST)
            {
                allocaPointer.ValueFlag = ValueFlag.Constant;
            }
            allocaPointer.ValueFlag |= ValueFlag.Variable;
            compiler.VariableManager.RegisterLocalVariable(varName, allocaPointer);




        }

    }
}

