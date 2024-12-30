using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;

namespace BoomifyCS.Assembly
{
    abstract class NodeHandler
    {
        protected AssemblyCompiler compiler;

        protected NodeHandler(AssemblyCompiler compiler)
        {
            this.compiler = compiler;
        }

        public abstract void HandleNode(AstNode token);
    }
}
