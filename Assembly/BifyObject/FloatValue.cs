using LLVMSharp.Interop;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly.BifyObject
{
    public class FloatValue : BifyValue
    {
        public FloatValue(LLVMValueRef value) : base(value, new FloatType()) { }

        public override BifyValue Equal(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOEQ,
                    this.GetLLVMValue(), other.GetLLVMValue(), "floateqtmp");
                return new BoolValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOEQ,
                    this.GetLLVMValue(), castValue, "floateqtmp");
                return new BoolValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"Cannot compare {this.GetTypeName()} with {other.GetTypeName()}"));
                return null;
            }
        }

        public override BifyValue NotEqual(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealONE,
                    this.GetLLVMValue(), other.GetLLVMValue(), "floatnetmp");
                return new BoolValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealONE,
                    this.GetLLVMValue(), castValue, "floatnetmp");
                return new BoolValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"Cannot compare {this.GetTypeName()} with {other.GetTypeName()}"));
                return null;
            }
        }

        public override BifyValue GreaterThan(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOGT,
                    this.GetLLVMValue(), other.GetLLVMValue(), "floatgttmp");
                return new BoolValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOGT,
                    this.GetLLVMValue(), castValue, "floatgttmp");
                return new BoolValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"Cannot compare {this.GetTypeName()} with {other.GetTypeName()}"));
                return null;
            }
        }

        public override BifyValue LessThan(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOLT,
                    this.GetLLVMValue(), other.GetLLVMValue(), "floatlttmp");
                return new BoolValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOLT,
                    this.GetLLVMValue(), castValue, "floatlttmp");
                return new BoolValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"Cannot compare {this.GetTypeName()} with {other.GetTypeName()}"));
                return null;
            }
        }

        public override BifyValue GreaterThanOrEqual(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOGE,
                    this.GetLLVMValue(), other.GetLLVMValue(), "floatgetmp");
                return new BoolValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOGE,
                    this.GetLLVMValue(), castValue, "floatgetmp");
                return new BoolValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"Cannot compare {this.GetTypeName()} with {other.GetTypeName()}"));
                return null;
            }
        }

        public override BifyValue LessThanOrEqual(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOLE,
                    this.GetLLVMValue(), other.GetLLVMValue(), "floatletmp");
                return new BoolValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOLE,
                    this.GetLLVMValue(), castValue, "floatletmp");
                return new BoolValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"Cannot compare {this.GetTypeName()} with {other.GetTypeName()}"));
                return null;
            }
        }

        public override BifyValue Add(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFAdd(this.GetLLVMValue(), other.GetLLVMValue(), "floataddtmp");
                return new FloatValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFAdd(this.GetLLVMValue(), castValue, "floataddtmp");
                return new FloatValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyArithmeticError("Type mismatch in float addition."));
                return null;
            }
        }

        public override BifyValue Sub(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFSub(this.GetLLVMValue(), other.GetLLVMValue(), "floatsubtmp");
                return new FloatValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFSub(this.GetLLVMValue(), castValue, "floatsubtmp");
                return new FloatValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyArithmeticError("Type mismatch in float subtraction."));
                return null;
            }
        }

        public override BifyValue Mul(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFMul(this.GetLLVMValue(), other.GetLLVMValue(), "floatmultmp");
                return new FloatValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFMul(this.GetLLVMValue(), castValue, "floatmultmp");
                return new FloatValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyArithmeticError("Type mismatch in float multiplication."));
                return null;
            }
        }

        public override BifyValue Div(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFDiv(this.GetLLVMValue(), other.GetLLVMValue(), "floatdivtmp");
                return new FloatValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFDiv(this.GetLLVMValue(), castValue, "floatdivtmp");
                return new FloatValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyArithmeticError("Type mismatch in float division."));
                return null;
            }
        }
    }

    public class FloatType : BifyType
    {
        public FloatType() : base("float", LLVMTypeRef.Float) { }

        public override BifyValue Create(object value)
        {
            unsafe
            {
                if (value is System.Single)
                {
                    LLVMValueRef llvmValue = LLVMValueRef.CreateConstReal(LLVM.FloatType(), (float)value);
                    return new FloatValue(llvmValue);
                }
                else if (value is System.Int32)
                {
                    LLVMValueRef llvmValue = LLVMValueRef.CreateConstReal(LLVM.FloatType(), (float)(int)value);
                    return new FloatValue(llvmValue);
                }
                else
                {
                    Traceback.Instance.ThrowException(new BifyTypeError("Invalid type for float."));
                    return null;
                }
            }
        }

        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new FloatValue(value);
        }

        public override uint Size()
        {
            return 4;
        }
    }
}
