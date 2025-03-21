using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    class BoolValue:BifyValue
    {
        public BoolValue(LLVMValueRef value) : base(value,new BoolType())
        {
        }
        public override BifyValue Equal(BifyValue other, LLVMBuilderRef builder)
        {
            if (!this.CompareType(other))
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"Cannot compare {this.GetTypeName()} with {other.GetTypeName()}"));
                return null;
            }
            var value = builder.BuildICmp(LLVMIntPredicate.LLVMIntEQ, 
                this.GetLLVMValue(), other.GetLLVMValue(), "equal");
            return new BoolValue(value);
        }
        public override BifyValue NotEqual(BifyValue other, LLVMBuilderRef builder)
        {
            if (!this.CompareType(other))
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"Cannot compare {this.GetTypeName()} with {other.GetTypeName()}"));
                return null;
            }
            var value = builder.BuildICmp(LLVMIntPredicate.LLVMIntNE,
                this.GetLLVMValue(), other.GetLLVMValue(), "not_equal");
            return new BoolValue(value);
        }
        public override BifyValue Not(LLVMBuilderRef builder)
        {
            var value = builder.BuildNot(this.GetLLVMValue(), "not");
            return new BoolValue(value);
        }
    }
    class BoolType : BifyType
    {
        public BoolType() : base("bool",LLVMTypeRef.Int1)
        {
        }
        public override BifyValue Create(object value)
        {
            return new BoolValue(LLVMValueRef.CreateConstInt(LLVMType, (ulong)(int)value, false));
        }
        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new BoolValue(value);
        }
        public override uint Size()
        {
            return 1;
        }
    }
}
