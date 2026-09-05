using System;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    [Flags]
    public enum ValueFlag
    {
        NONE = 0,

        CONSTANT = 1 << 0,
        VARIABLE = 1 << 1, 
    }
    public enum AccessLevel
    {
        PRIVATE,
        PUBLIC,
        PROTECTED
    }
    public interface  IValue
    {
        public abstract LLVMValueRef GetLlvmValue();

    }

}
