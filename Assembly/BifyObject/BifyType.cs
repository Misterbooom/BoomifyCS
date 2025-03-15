using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    public abstract class BifyType:IValue
    {
        public string Name { get; }
        public LLVMTypeRef LLVMType { get; }

        protected BifyType(string name, LLVMTypeRef llvmType)
        {
            Name = name;
            LLVMType = llvmType;
        }
       
        public LLVMValueRef GetLLVMValue()
        {
            return null;
        }
        public virtual bool CompareType(BifyType other)
        {
            return this.GetType() == other.GetType();
        }
        public abstract BifyValue Create(object value);
        public abstract BifyValue CreateByValueRef(LLVMValueRef value);

    }
}
