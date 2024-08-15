using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Objects;

namespace BoomifyCS.Interpreter.VM
{
    public class StackManager
    {
        private readonly Stack<BifyObject> globalStack = new();
        private readonly Stack<BifyObject> localStack = new();
        private string context = "global";

        public StackManager() { }

        public void SetContext(string context)
        {
            if (context == "global" || context == "local")
            {
                this.context = context;
            }
            else
            {
                throw new ArgumentException("Invalid context. Use 'local' or 'global'.");
            }
        }

        public void Push(BifyObject value)
        {
            if (context == "global")
            {
                globalStack.Push(value);
            }
            else if (context == "local")
            {
                localStack.Push(value);
            }
        }

        public BifyObject Pop()
        {
            if (context == "global")
            {
                return globalStack.Pop();
            }
            else if (context == "local")
            {
                return localStack.Pop();
            }
            throw new InvalidOperationException("Invalid stack context.");
        }

        public BifyObject Peek()
        {
            if (context == "global")
            {
                return globalStack.Peek();
            }
            else if (context == "local")
            {
                return localStack.Peek();
            }
            throw new InvalidOperationException("Invalid stack context.");
        }

        public int Count()
        {
            if (context == "global")
            {
                return globalStack.Count;
            }
            else if (context == "local")
            {
                return localStack.Count;
            }
            throw new InvalidOperationException("Invalid stack context.");
        }

        public void Clear()
        {
            if (context == "global")
            {
                globalStack.Clear();
            }
            else if (context == "local")
            {
                localStack.Clear();
            }
            throw new InvalidOperationException("Invalid stack context.");
        }
        public void Print()
        {
            Stack<BifyObject> stackToPrint = context == "global" ? globalStack : localStack;

            Console.WriteLine($"Contents of the {context} stack:");

            foreach (var item in stackToPrint)
            {
                Console.WriteLine(item.Repr());
            }
        }
    }
}
