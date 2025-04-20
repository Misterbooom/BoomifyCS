using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    class BoolValue : BifyValue
    {
        public BoolValue(LLVMValueRef value) : base(value, new BoolType())
        {
        }
        

        public override BifyValue Equal(BifyValue other, LLVMBuilderRef builder)
        {
            if (!this.CompareType(other))
            {
                Traceback.Instance.ThrowException(new BifyTypeError(
                    $"Type error in '==' operation: cannot compare '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                    "Both operands must be of type 'bool'."));
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
                Traceback.Instance.ThrowException(new BifyTypeError(
                    $"Type error in '!=' operation: cannot compare '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                    "Both operands must be of type 'bool'."));
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
        public BoolType() : base("bool", LLVMTypeRef.Int1)
        {
        }
        public override BifyValue DefaultValue()
        {
            return Create(false);
        }

        public override BifyValue Create(object value)
        {
            try
            {
                // Expecting value to be convertible to int.
                int intValue = Convert.ToInt32(value);
                return new BoolValue(LLVMValueRef.CreateConstInt(LLVMType, (ulong)intValue, false));
            }
            catch (Exception ex)
            {
                Traceback.Instance.ThrowException(new BifyTypeError(
                    $"Error in creating a 'bool' value: provided value '{value}' is not convertible to an integer. Details: {ex.Message}"
                ));
                return null;
            }
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
