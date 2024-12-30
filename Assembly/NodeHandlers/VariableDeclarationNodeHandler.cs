using System;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class VariableDeclarationNodeHandler : NodeHandler
    {
        public VariableDeclarationNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            AstVarDecl variableDeclaration = (AstVarDecl)node;
            string variableName = variableDeclaration.AssignmentNode.Left.Token.Value;  
            string variableType = variableDeclaration.Type.Token.Value;  
            AstNode initialValueNode = variableDeclaration.AssignmentNode.Right; 

            int offset = compiler.variableManager.AllocateLocal(variableName,variableType);
            compiler.Visit(initialValueNode);
            string assemblerCode = $"mov [rbp - {offset}],{compiler.LastUsedRegister}";
            this.compiler.assemblerCode.AddInstruction(assemblerCode);
        }

        
    }
  

}

