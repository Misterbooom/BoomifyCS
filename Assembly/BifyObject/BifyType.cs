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
        public ValueFlag ValueFlag = ValueFlag.None;

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
        public virtual bool CompareType(Type other)
        {
            return this.GetType() == other;
        }

        public BifyValue CreateValueRef(LLVMValueRef value)
        {
            var bifyValue = CreateByValueRef(value);
            bifyValue.ValueFlag = this.ValueFlag; 
            return bifyValue;
        }

        public abstract BifyValue Create(object value);
        
        protected abstract BifyValue CreateByValueRef(LLVMValueRef value);
        public abstract uint Size();
        public override string ToString()
        {
            return $"{Name} ({LLVMType})";
        }
    }
    class AnyType : BifyType
    {
        public AnyType() : base("any", LLVMTypeRef.Void) { }
        protected override BifyValue CreateByValueRef(LLVMValueRef value)
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
