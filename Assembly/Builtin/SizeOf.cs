using System.Collections.Generic;
using BoomifyCS.Assembly.BifyObject;

namespace BoomifyCS.Assembly.Builtin
{
    internal class SizeOf:BifyFunction
    {
        public SizeOf() : base(null, null, null, null)
        {
            FunctionArgs = new FunctionArgs(null);
            FunctionArgs.SetArguments(new Dictionary<string, BifyType> { { "value",new AnyType() } });
            ReturnType = new IntegerType();
        }
        public override BifyValue Call(BifyValue[] args)
        {
            return new IntegerType().Create((int)args[0].GetBifyType().Size());
        }
    }
}
