using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLVMSharp;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    class PointerValue:BifyValue
    {
        public PointerValue(LLVMValueRef value,BifyType type):base(value,new BifyPointerType(type))
        {

        }
    }
    class BifyPointerType : BifyType
    {
        public BifyType BifyType;
        public BifyPointerType(BifyType bifyType = null) : base(bifyType.Name, bifyType.LLVMType)
        {
            this.BifyType = bifyType;
        }
        public override BifyValue Create(object value)
        {
            throw new NotImplementedException("Create method not implemented in Pointer type");
        }
        public override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            throw new NotImplementedException("Create by valueRef method not implemented in Pointer type");

        }

    }
}
