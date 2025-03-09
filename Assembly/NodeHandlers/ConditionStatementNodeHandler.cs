using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class ConditionStatementNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            if (node is AstIf astIf)
            {
                HandleIfStatement(astIf);
            }
        }
        private void HandleIfStatement(AstIf node)
        {
        }
    }
}
