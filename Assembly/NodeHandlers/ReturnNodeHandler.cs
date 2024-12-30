using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class ReturnNodeHandler(AssemblyCompiler compiler): NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstReturn returnNode = (AstReturn)node;
            compiler.Visit(returnNode.ArgumentsNode);
            compiler.assemblerCode.AddInstruction($"mov rax, {compiler.LastUsedRegister}");
            compiler.assemblerCode.AddInstruction("mov rsp, rbp");
            compiler.assemblerCode.AddInstruction("pop rbp");
            compiler.assemblerCode.AddInstruction("ret");
            compiler.ReturnStatement = true;
        }
    }
}
