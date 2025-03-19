using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    public abstract class BifyType : IValue
    {
        public string Name { get; }
        public LLVMTypeRef LLVMType { get; protected set; }

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
        public abstract uint Size();
    }
    class AnyType : BifyType
    {
        public AnyType() : base("any", LLVMTypeRef.Void) { }
        public override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            throw new NotImplementedException();
        }
        public override BifyValue Create(object value)
        {
            throw new NotImplementedException();
        }
        public override bool CompareType(BifyType other)
        {
            return true;
        }
        public override uint Size()
        {
            throw new NotImplementedException();
        }

    }
}
