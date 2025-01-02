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

            
            LLVMTypeRef lLVMType = compiler.variableManager.AllocateLocal(identifier, typeIdentifier);

            var varAlloca = compiler.builder.BuildAlloca(lLVMType, identifier);
            
            compiler.Visit(astVarDecl.AssignmentNode.Right);

            compiler.builder.BuildStore(astVarDecl.AssignmentNode.Right.LlvmValue,varAlloca);


        }

        
    }
  

}

