using System;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
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
                Traceback.Instance.ThrowException(new BifyTypeError($"{value.GetType().Name.ToLower()} cannot be used as type."));
                return;
            }
            BifyType bifyType = (BifyType)value;
            if (bifyType is ArrayType)
            {
                compiler.StackPush(bifyType);
            }
            compiler.Visit(varDeclNode.AssignmentNode.Right);
            BifyValue loadedValue = compiler.StackPop();
            BifyDebug.Log($"Var type - {bifyType.Name} Var value type - {loadedValue.GetTypeName()} LLVMVarType: {bifyType.LLVMType} pointer to i8");

            BifyValue variableValue = loadedValue.AutoCast(bifyType, compiler.builder);



            var alloca = compiler.builder.BuildAlloca(bifyType.LLVMType, varName);

            compiler.builder.BuildStore(variableValue.GetLLVMValue(), alloca);
            compiler.variableManager.RegisterLocalVariable(varName, 

                new BifyPointerType(variableValue.GetBifyType()).CreateByValueRef(alloca)
                );
         
        }

    }
}

