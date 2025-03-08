using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    public class IntegerValue : BifyValue
    {
        public IntegerValue(LLVMValueRef value) : base(value, "int") { }


        public override BifyValue Add(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildAdd(this.GetLLVMValue(), other.GetLLVMValue(), "intaddtmp");
                return new IntegerValue(result);
            }
            else if (other is FloatValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(this.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFAdd(castValue, other.GetLLVMValue(), "floataddtmp");
                return new FloatValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyArithmeticError("Type mismatch in integer addition."));
                return null;
            }
        }

        public override BifyValue Sub(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildSub(this.GetLLVMValue(), other.GetLLVMValue(), "intsubtmp");
                return new IntegerValue(result);
            }
            else if (other is FloatValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(this.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFSub(castValue, other.GetLLVMValue(), "floatsubtmp");
                return new FloatValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyArithmeticError("Type mismatch in integer subtraction."));
                return null;
            }
        }

        public override BifyValue Mul(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildMul(this.GetLLVMValue(), other.GetLLVMValue(), "intmultmp");
                return new IntegerValue(result);
            }
            else if (other is FloatValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(this.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFMul(castValue, other.GetLLVMValue(), "floatmultmp");
                return new FloatValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyArithmeticError("Type mismatch in integer multiplication."));
                return null;
            }
        }

        public override BifyValue Div(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildSDiv(this.GetLLVMValue(), other.GetLLVMValue(), "intdivtmp");
                return new IntegerValue(result);
            }
            else if (other is FloatValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(this.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFDiv(castValue, other.GetLLVMValue(), "floatdivtmp");
                return new FloatValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyArithmeticError("Type mismatch in integer division."));
                return null;
            }
        }
    }

    public class IntegerType : BifyType
    {
        public IntegerType() : base("int", LLVMTypeRef.Int32) { }

        public override BifyValue Create(object value)
        {
            LLVMValueRef llvmValue = LLVMValueRef.CreateConstInt(LLVMType, (ulong)(int)value, false);
            return new IntegerValue(llvmValue);
        }
        public override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new IntegerValue(value);
        }
    }


}
