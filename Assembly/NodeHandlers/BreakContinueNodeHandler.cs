using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class BreakContinueNodeHandler:NodeHandler
    {
        public BreakContinueNodeHandler(AssemblyCompiler compiler) : base(compiler) { }
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
            var loop = compiler.LoopManager.GetCurrentLoop();
            if (loop == null)
            {
                Traceback.Instance.ThrowException(new BifySyntaxError("Break statement must be inside a loop."));
                return;
            }
            compiler.Builder.BuildBr(loop.MergeBB);
            compiler.LoopManager.AddBranch();
        }
        private void HandleContinue()
        {
            var loop = compiler.LoopManager.GetCurrentLoop();
            if (loop == null)
            {
                Traceback.Instance.ThrowException(new BifySyntaxError("Continue statement must be inside a loop."));
                return;
            }
            compiler.Builder.BuildBr(loop.ContinueBB);
            compiler.LoopManager.AddBranch();
        }
    }
}
