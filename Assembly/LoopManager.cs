using System.Collections.Generic;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly
{
    internal class LoopContext(LLVMBasicBlockRef conditionBb, LLVMBasicBlockRef mergeBb, LLVMBasicBlockRef continueBb)
    {
        public LLVMBasicBlockRef ConditionBb = conditionBb;
        public LLVMBasicBlockRef MergeBb = mergeBb;
        public LLVMBasicBlockRef ContinueBb = continueBb;
    }

    internal class LoopManager
    {
        private readonly Stack<LoopContext> _loopsStack = [];
        private readonly Stack<int> _branches = [];
        public LoopManager()
        {
        }
        public void AddLoop(LoopContext loopContext)
        {
            _loopsStack.Push(loopContext);
        }
        public LoopContext GetCurrentLoop()
        {
            if (_loopsStack.Count == 0)
            {
                return null;
            }
            return _loopsStack.Peek();
        }
        public void PopLoop()
        {
            _loopsStack.Pop();
        }
        public void AddBranch()
        {
            _branches.Push(1);
        }
        public void PopBranch()
        {
            _branches.Pop();
        }
        public void ClearBranches()
        {
            _branches.Clear();
        }
        public int GetCurrentBranch()
        {
            if (_branches.Count == 0)
            {
                return 0;
            }
            return _branches.Peek();
        }
    }
}
