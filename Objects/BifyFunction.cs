using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Objects;
using BoomifyCS.Lexer;
using LLVMSharp.Interop;
namespace BoomifyCS.Objects
{
    public class BifyFunction : BifyObject
    {
        public  string Name;
        public  BifyObject returnObject;
        public List<string> arguments = [];
        public Type returnType;
        public LLVMTypeRef functionType; 
        public LLVMValueRef functionValue = null;
        public BifyFunction(string name) : base()
        {
            Name = name;
            returnObject = new BifyNull();
            ExpectedArgCount = -1;
        }
        public BifyFunction(string name,Type type) : base()
        {
            Name = name;
            returnObject = new BifyNull();
            ExpectedArgCount = -1;
            returnType = type;
        }
        public override BifyObject Call(List<BifyObject> arguments) => returnObject;
        
        public virtual LLVMValueRef LLVMBuild()
        {
            throw new NotImplementedException("LLVMCALL not implemented");
        }
        public override BifyString Repr()
        {
            if (arguments.Count > 0)
            {
                return new BifyString($"<{Name} Function, Args - [{string.Join(",",arguments)}]>");
            }
            return new BifyString($"<{Name} Function>");
        }
        public override string ToString() => ObjectToString().Value;
    }
}
