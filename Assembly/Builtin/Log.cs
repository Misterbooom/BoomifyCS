using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly.Builtin
{
    class Log:BifyFunction
    {
        public Log() : base(null, null, null,null)
        {
            FunctionArgs = new FunctionArgs(null);
            FunctionArgs.SetArguments(new Dictionary<string, BifyType> { {"arg",new AnyType()}});
            ReturnType = new VoidType();
            IsVariadic = true;
        }
        public override BifyValue Call(BifyValue[] args)
        {
            Console.WriteLine("=== Log Output Start ===");
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine($"    [{i}] => {args[i]}");
            }
            Console.WriteLine("=== Log Output End ===");

            return ReturnType.CreateValueRef(null);
        }
        public static void RealTimeLog(string message)
        {
            AssemblyCompiler.Instance.VariableManager.GetBifyValue("explode").Call([new ConstStringType().Create(message)]);
        }
    }
}
