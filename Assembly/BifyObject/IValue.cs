using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly
{
    public enum ValueFlag
    {
        None,
        Constant,
        Variable
    }
    public interface  IValue
    {
        public abstract LLVMValueRef GetLLVMValue();

    }

}
