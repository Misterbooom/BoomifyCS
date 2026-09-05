using LLVMSharp.Interop;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly.BifyObject
{
    public class FloatValue(LLVMValueRef value) : BifyValue(value, new FloatType())
    {
        public override BifyValue Equal(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOEQ,
                    this.GetLlvmValue(), other.GetLlvmValue(), "floateqtmp");
                return new BoolValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLlvmValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOEQ,
                    this.GetLlvmValue(), castValue, "floateqtmp");
                return new BoolValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError(
                    $"Type error in '==' operation: cannot compare '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                    "Expected operand types: both 'float' or 'float' and 'int'."));
                return null;
            }
        }

        public override BifyValue NotEqual(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealONE,
                    this.GetLlvmValue(), other.GetLlvmValue(), "floatnetmp");
                return new BoolValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLlvmValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealONE,
                    this.GetLlvmValue(), castValue, "floatnetmp");
                return new BoolValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError(
                    $"Type error in '!=' operation: cannot compare '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                    "Expected operand types: both 'float' or 'float' and 'int'."));
                return null;
            }
        }

        public override BifyValue GreaterThan(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOGT,
                    this.GetLlvmValue(), other.GetLlvmValue(), "floatgttmp");
                return new BoolValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLlvmValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOGT,
                    this.GetLlvmValue(), castValue, "floatgttmp");
                return new BoolValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError(
                    $"Type error in '>' operation: cannot compare '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                    "Expected operand types: both 'float' or 'float' and 'int'."));
                return null;
            }
        }

        public override BifyValue LessThan(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOLT,
                    this.GetLlvmValue(), other.GetLlvmValue(), "floatlttmp");
                return new BoolValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLlvmValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOLT,
                    this.GetLlvmValue(), castValue, "floatlttmp");
                return new BoolValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError(
                    $"Type error in '<' operation: cannot compare '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                    "Expected operand types: both 'float' or 'float' and 'int'."));
                return null;
            }
        }

        public override BifyValue GreaterThanOrEqual(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOGE,
                    this.GetLlvmValue(), other.GetLlvmValue(), "floatgetmp");
                return new BoolValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLlvmValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOGE,
                    this.GetLlvmValue(), castValue, "floatgetmp");
                return new BoolValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError(
                    $"Type error in '>=' operation: cannot compare '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                    "Expected operand types: both 'float' or 'float' and 'int'."));
                return null;
            }
        }

        public override BifyValue LessThanOrEqual(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOLE,
                    this.GetLlvmValue(), other.GetLlvmValue(), "floatletmp");
                return new BoolValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLlvmValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOLE,
                    this.GetLlvmValue(), castValue, "floatletmp");
                return new BoolValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError(
                    $"Type error in '<=' operation: cannot compare '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                    "Expected operand types: both 'float' or 'float' and 'int'."));
                return null;
            }
        }

        public override BifyValue Add(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFAdd(this.GetLlvmValue(), other.GetLlvmValue(), "floataddtmp");
                return new FloatValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLlvmValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFAdd(this.GetLlvmValue(), castValue, "floataddtmp");
                return new FloatValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyArithmeticError(
                    $"Arithmetic error in '+' operation: cannot add '{this.GetTypeName()}' and '{other.GetTypeName()}'. " +
                    "Expected operand types: both 'float' or 'float' and 'int'."));
                return null;
            }
        }

        public override BifyValue Sub(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFSub(this.GetLlvmValue(), other.GetLlvmValue(), "floatsubtmp");
                return new FloatValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLlvmValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFSub(this.GetLlvmValue(), castValue, "floatsubtmp");
                return new FloatValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyArithmeticError(
                    $"Arithmetic error in '-' operation: cannot subtract '{other.GetTypeName()}' from '{this.GetTypeName()}'. " +
                    "Expected operand types: both 'float' or 'float' and 'int'."));
                return null;
            }
        }

        public override BifyValue Mul(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFMul(this.GetLlvmValue(), other.GetLlvmValue(), "floatmultmp");
                return new FloatValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLlvmValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFMul(this.GetLlvmValue(), castValue, "floatmultmp");
                return new FloatValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyArithmeticError(
                    $"Arithmetic error in '*' operation: cannot multiply '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                    "Expected operand types: both 'float' or 'float' and 'int'."));
                return null;
            }
        }

        public override BifyValue Div(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildFDiv(this.GetLlvmValue(), other.GetLlvmValue(), "floatdivtmp");
                return new FloatValue(result);
            }
            else if (other is IntegerValue)
            {
                LLVMValueRef castValue = builder.BuildSIToFP(other.GetLlvmValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFDiv(this.GetLlvmValue(), castValue, "floatdivtmp");
                return new FloatValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyArithmeticError(
                    $"Arithmetic error in '/' operation: cannot divide '{this.GetTypeName()}' by '{other.GetTypeName()}'. " +
                    "Expected operand types: both 'float' or 'float' and 'int'."));
                return null;
            }
        }
    }

    public class FloatType() : BifyType("float", LLVMTypeRef.Float)
    {
        public override BifyValue DefaultValue()
        {
            return Create(0);
        }
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
                    Traceback.Instance.ThrowException(new BifyTypeError(
                        $"Type error in 'float' creation: expected a 'float' or 'int' value, but received '{value.GetType().Name}'."));
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
