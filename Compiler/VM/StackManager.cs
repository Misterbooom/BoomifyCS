using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Objects;

namespace BoomifyCS.Compiler.VM
{
    public class StackManager
    {
        private Stack<BifyObject> globalStack = new();
        private readonly Stack<Stack<BifyObject>> scopeStack = new();
        public StackManager() { }

        public void Push(BifyObject value)
        {
            globalStack.Push(value);
        }
        
        public BifyObject Pop()
        {
            return globalStack.Pop();
        }
        public void PushScope()
        {
            scopeStack.Push(new Stack<BifyObject>(globalStack.Reverse()));

            globalStack.Clear();
        }

        public void PopScope()
        {
            globalStack = scopeStack.Pop();
        }
        public BifyObject Peek()
        {
            return globalStack.Peek();
        }

        public int Count()
        {
            return globalStack.Count;
        }

        public void Clear()
        {
            globalStack.Clear();
        }

        public void Print()
        {
            Console.WriteLine("Stack:");
            foreach (var item in globalStack)
            {
                Console.WriteLine(item.Repr());
            }
            Console.WriteLine("End Of Stack");
        }
    }
}
