using System;
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
            AstVarDecl astVarDecl = node as AstVarDecl;
            string identifier = astVarDecl.AssignmentNode.Left.Token.Value;
            string typeIdentifier = astVarDecl.Type.Token.Value;
            Variable typeVar = compiler.variableManager.AllocateLocal(identifier, typeIdentifier);
            var varAlloca = compiler.builder.BuildAlloca(typeVar.LlvmType, identifier);
            compiler.Visit(astVarDecl.AssignmentNode.Right);
            BifyValue varValue = compiler.stack.Pop();
            if (varValue.GetBifyObject().GetType() != typeVar.Type)
            {
                Traceback.Instance.ThrowException(
                    new BifyCastError($"Cannot assign {varValue.GetBifyObject().GetName()} to {typeVar.Name}")
                    );
                return;
            } 

            compiler.builder.BuildStore(varValue.GetValueRef(), varAlloca);
            compiler.variableManager.SetLocalValue(identifier, varValue.GetValueRef());
            compiler.variableManager.SetLocalBifyObject(identifier, varValue.GetBifyObject());


        }


    }


}

