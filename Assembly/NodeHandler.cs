using BoomifyCS.Ast;

namespace BoomifyCS.Assembly
{
    abstract class NodeHandler(AssemblyCompiler compiler)
    {
        protected readonly AssemblyCompiler Compiler = compiler;

        public abstract void HandleNode(AstNode node);
    }
}
