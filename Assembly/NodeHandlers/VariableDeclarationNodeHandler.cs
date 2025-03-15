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

            compiler.Visit(varDeclNode.AssignmentNode.Right);
            BifyValue variableValue = compiler.StackPop().AutoCast(bifyType,compiler.builder);

            

            var alloca = compiler.builder.BuildAlloca(bifyType.LLVMType, varName);
            
                compiler.builder.BuildStore(variableValue.GetLLVMValue(), alloca);
                compiler.variableManager.RegisterLocalVariable(varName, variableValue.GetBifyType().CreateByValueRef(alloca));


        }

    }
}

