using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using System.Data;

namespace BoomifyCS.AssemblyCompiler
{
    class NodeHandlerFactory
    {
        public static NodeHandler CreateHandler(AstNode node, AssemblyCompiler compiler)
        {
            return node switch
            {
                _ => throw new SyntaxErrorException($"Unhandled node - {nameof(node)}"),
            };
        }
    }
}
