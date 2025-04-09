using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    public class IntegerValue : BifyValue
    {
        public IntegerValue(LLVMValueRef value) : base(value, new IntegerType()) { }

        public override BifyValue Equal(BifyValue other, LLVMBuilderRef builder)
        {
            if (!this.CompareType(other))
            {
                Traceback.Instance.ThrowException(new BifyTypeError(
                    $"Type error in '==' operation: cannot compare '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                    $"Ensure both operands are of compatible types."
                ));
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
                    $"Operands must be of the same type."
                ));
                return null;
            }
            var value = builder.BuildICmp(LLVMIntPredicate.LLVMIntNE,
                this.GetLLVMValue(), other.GetLLVMValue(), "not_equal");
            return new BoolValue(value);
        }

        public override BifyValue GreaterThan(BifyValue other, LLVMBuilderRef builder)
        {
            if (other.CompareType(typeof(FloatType)))
            {
                LLVMValueRef castValue = builder.BuildSIToFP(this.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOGT, castValue, other.GetLLVMValue(), "floatgttmp");
                return new BoolValue(result);
            }
            else if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildICmp(LLVMIntPredicate.LLVMIntSGT, this.GetLLVMValue(), other.GetLLVMValue(), "intgttmp");
                return new BoolValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError(
                    $"Type error in '>' operation: cannot compare '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                    $"Operands must be either both integers or an integer and a float."
                ));
                return null;
            }
        }

        public override BifyValue LessThan(BifyValue other, LLVMBuilderRef builder)
        {
            if (other.CompareType(typeof(FloatType)))
            {
                LLVMValueRef castValue = builder.BuildSIToFP(this.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOLT, castValue, other.GetLLVMValue(), "floatlttmp");
                return new BoolValue(result);
            }
            else if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildICmp(LLVMIntPredicate.LLVMIntSLT, this.GetLLVMValue(), other.GetLLVMValue(), "intlttmp");
                return new BoolValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError(
                    $"Type error in '<' operation: cannot compare '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                    $"Ensure both operands are of compatible types."
                ));
                return null;
            }
        }

        public override BifyValue LessThanOrEqual(BifyValue other, LLVMBuilderRef builder)
        {
            if (other.CompareType(typeof(FloatType)))
            {
                LLVMValueRef castValue = builder.BuildSIToFP(this.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOLE, castValue, other.GetLLVMValue(), "floatletmp");
                return new BoolValue(result);
            }
            else if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildICmp(LLVMIntPredicate.LLVMIntSLE, this.GetLLVMValue(), other.GetLLVMValue(), "intletmp");
                return new BoolValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError(
                    $"Type error in '<=' operation: cannot compare '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                    $"Operands must be of the same type or one must be a float."
                ));
                return null;
            }
        }

        public override BifyValue GreaterThanOrEqual(BifyValue other, LLVMBuilderRef builder)
        {
            if (other.CompareType(typeof(FloatType)))
            {
                LLVMValueRef castValue = builder.BuildSIToFP(this.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                LLVMValueRef result = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOGE, castValue, other.GetLLVMValue(), "floatgetmp");
                return new BoolValue(result);
            }
            else if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildICmp(LLVMIntPredicate.LLVMIntSGE, this.GetLLVMValue(), other.GetLLVMValue(), "intgetmp");
                return new BoolValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError(
                    $"Type error in '>=' operation: cannot compare '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                    $"Operands must be either both integers or an integer and a float."
                ));
                return null;
            }
        }

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
                Traceback.Instance.ThrowException(new BifyArithmeticError(
                    $"Arithmetic error in addition: cannot add '{this.GetTypeName()}' and '{other.GetTypeName()}'. " +
                    $"Operands must be of the same type or compatible numeric types."
                ));
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
                Traceback.Instance.ThrowException(new BifyArithmeticError(
                    $"Arithmetic error in subtraction: cannot subtract '{other.GetTypeName()}' from '{this.GetTypeName()}'. " +
                    $"Operands must be of the same type or compatible numeric types."
                ));
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
                Traceback.Instance.ThrowException(new BifyArithmeticError(
                    $"Arithmetic error in multiplication: cannot multiply '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                    $"Operands must be of the same type or compatible numeric types."
                ));
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
                Traceback.Instance.ThrowException(new BifyArithmeticError(
                    $"Arithmetic error in division: cannot divide '{this.GetTypeName()}' by '{other.GetTypeName()}'. " +
                    $"Operands must be of the same type or compatible numeric types."
                ));
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
        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new IntegerValue(value);
        }
        public override uint Size()
        {
            return 4;
        }
    }
}
