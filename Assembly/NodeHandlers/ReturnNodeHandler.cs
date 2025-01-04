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
            AstReturn astReturn = node as AstReturn; 
            if (astReturn.ArgumentsNode == null) {
                compiler.builder.BuildRetVoid();
                return;
            }
            compiler.Visit(astReturn.ArgumentsNode);
            compiler.builder.BuildRet(astReturn.ArgumentsNode.LlvmValue);
        }
    }
}
