using System;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    public abstract class BifyValue : IValue
    {

        protected LLVMValueRef llvmValue;
        protected BifyType type;

        public BifyValue(LLVMValueRef value, BifyType type)
        {
            this.llvmValue = value;
            this.type = type;
        }

        public LLVMValueRef GetLLVMValue() => llvmValue;
        public void SetLLVMValue(LLVMValueRef value) => llvmValue = value;
        public BifyType GetBifyType() => type;
        public string GetTypeName() => type.Name;
        public virtual bool CompareType(Type other) => GetBifyType().GetType() == other;
        public virtual bool CompareType(BifyValue other) => GetBifyType().CompareType(other.GetBifyType());
        public BifyValue AutoCast(BifyType desiredType, LLVMBuilderRef builder)
        {

            if (!desiredType.CompareType(type))
            {
                if (type is IntegerType && desiredType is FloatType)
                {
                    LLVMValueRef floatValue = builder.BuildSIToFP(this.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                    return new FloatValue(floatValue);
                }
                else if (type is FloatType && desiredType is IntegerType)
                {
                    LLVMValueRef intValue = builder.BuildFPToSI(this.GetLLVMValue(), LLVMTypeRef.Int32, "cast_float_to_int");
                    return new IntegerValue(intValue);
                }
                else
                {
                    Traceback.Instance.ThrowException(new BifyTypeError($"Cannot assign {this.GetTypeName()} to {desiredType.Name}"));
                    return null;
                }
            }
            return this;
        }
        public virtual BifyValue Not(LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyTypeError($"{GetTypeName()} doesn't support Not"));
            return null;
        }
        public virtual BifyValue Equal(BifyValue other, LLVMBuilderRef builder)
        {
            return new BoolType().Create(other.llvmValue == llvmValue ? 1 : 0);
        }
        public virtual BifyValue NotEqual(BifyValue other, LLVMBuilderRef builder)
        {
            return new BoolType().Create(1);
        }
        public virtual BifyValue LessThan(BifyValue other, LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyTypeError($"{GetTypeName()} doesn't support LessThan"));
            return null;
        }
        public virtual BifyValue GreaterThan(BifyValue other, LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyTypeError($"{GetTypeName()} doesn't support GreaterThan"));
            return null;
        }
        public virtual BifyValue LessThanOrEqual(BifyValue other, LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyTypeError($"{GetTypeName()} doesn't support LessThanOrEqual"));
            return null;
        }
        public virtual BifyValue GreaterThanOrEqual(BifyValue other, LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyTypeError($"{GetTypeName()} doesn't support GreaterThanOrEqual"));
            return null;
        }

        public virtual BifyValue Call(BifyValue[] args)
        {
            Traceback.Instance.ThrowException(new BifyTypeError($"{GetTypeName()} is not callable"));
            return null;
        }

        public virtual BifyValue Add(BifyValue other, LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyArithmeticError($"{GetTypeName()} doesn't support Add"));
            return null;
        }

        public virtual BifyValue Sub(BifyValue other, LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyArithmeticError($"{GetTypeName()} doesn't support Sub"));
            return null;
        }

        public virtual BifyValue Mul(BifyValue other, LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyArithmeticError($"{GetTypeName()} doesn't support Mul"));
            return null;
        }

        public virtual BifyValue Div(BifyValue other, LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyArithmeticError($"{GetTypeName()} doesn't support Div"));
            return null;
        }
    }
}
