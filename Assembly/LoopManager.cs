using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly
{
    class LoopContext
    {
        public LLVMBasicBlockRef ConditionBB;
        public LLVMBasicBlockRef MergeBB;
        public LLVMBasicBlockRef ContinueBB;
        public LoopContext(LLVMBasicBlockRef conditionBB, LLVMBasicBlockRef mergeBB, LLVMBasicBlockRef continueBB)
        {
            ConditionBB = conditionBB;
            MergeBB = mergeBB;
            ContinueBB = continueBB;
        }

    }
    class LoopManager
    {
        private Stack<LoopContext> loopsStack = [];
        private Stack<int> branches = [];
        public LoopManager()
        {
        }
        public void AddLoop(LoopContext loopContext)
        {
            loopsStack.Push(loopContext);
        }
        public LoopContext GetCurrentLoop()
        {
            if (loopsStack.Count == 0)
            {
                return null;
            }
            return loopsStack.Peek();
        }
        public void PopLoop()
        {
            loopsStack.Pop();
        }
        public void AddBranch()
        {
            branches.Push(1);
        }
        public void PopBranch()
        {
            branches.Pop();
        }
        public void ClearBranches()
        {
            branches.Clear();
        }
        public int GetCurrentBranch()
        {
            if (branches.Count == 0)
            {
                return 0;
            }
            return branches.Peek();
        }
    }
}
