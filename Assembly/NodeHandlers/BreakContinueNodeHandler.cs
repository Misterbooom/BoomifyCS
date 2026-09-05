using BoomifyCS.Ast;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class BreakContinueNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            if (node is AstBreak)
            {
                HandleBreak();
            }
            else if (node is AstContinue)
            {
                HandleContinue();
            }
        }
        private void HandleBreak()
        {
            var loop = Compiler.LoopManager.GetCurrentLoop();
            if (loop == null)
            {
                Traceback.Instance.ThrowException(new BifySyntaxError("Break statement must be inside a loop."));
                return;
            }
            Compiler.Builder.BuildBr(loop.MergeBb);
            Compiler.LoopManager.AddBranch();
        }
        private void HandleContinue()
        {
            var loop = Compiler.LoopManager.GetCurrentLoop();
            if (loop == null)
            {
                Traceback.Instance.ThrowException(new BifySyntaxError("Continue statement must be inside a loop."));
                return;
            }
            Compiler.Builder.BuildBr(loop.ContinueBb);
            Compiler.LoopManager.AddBranch();
        }
    }
}
