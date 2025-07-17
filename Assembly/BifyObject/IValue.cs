using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly
{
    [Flags]
    public enum ValueFlag
    {
        None = 0,

        Constant = 1 << 0,
        Variable = 1 << 1, 

        Private = 0 << 2, 
        Public = 1 << 2,
        Protected = 2 << 2,

        AccessMask = 3 << 2 
    }

    public static partial class ValueFlagExtensions
    {
      
        public static ValueFlag WithAccess(this ValueFlag flags, ValueFlag access)
        {
            return (flags & ~ValueFlag.AccessMask) | (access & ValueFlag.AccessMask);
        }
    }

    public interface  IValue
    {
        public abstract LLVMValueRef GetLLVMValue();

    }

}
