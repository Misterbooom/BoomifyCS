using System;
using System.Threading.Tasks.Sources;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    
    public abstract class BifyValue : IValue
    {
        public ValueFlag ValueFlag
        {
            get => type.ValueFlag;
            set
            {
                type.ValueFlag = value;
            }
        }
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
        public virtual bool CompareType(Type other) => GetBifyType().CompareType(other);
        public virtual bool CompareType(BifyValue other) => GetBifyType().CompareType(other.GetBifyType());
        public BifyValue AutoCast(BifyType desiredType, LLVMBuilderRef builder)
        {
            if (desiredType.CompareType(this.type))
                return this;

            if (desiredType is BifyPointerType desiredPtr)
            {
               if (desiredPtr.PointedType.CompareType(GetBifyType()))
                {
                    return desiredPtr.CreateValueRef(this.GetLLVMValue());
                }
                Traceback.Instance.ThrowException(
                    new BifyTypeError($"Cannot auto cast {this.GetTypeName()} to {desiredType.Name}")
                );
             
            }

            if (this.type is IntegerType && desiredType is FloatType)
            {
                LLVMValueRef floatValue = builder.BuildSIToFP(this.GetLLVMValue(), desiredType.LLVMType, "cast_int_to_float");
                return new FloatValue(floatValue);
            }
            else if (this.type is FloatType && desiredType is IntegerType)
            {
                LLVMValueRef intValue = builder.BuildFPToSI(this.GetLLVMValue(), desiredType.LLVMType, "cast_float_to_int");
                return new IntegerValue(intValue);
            }

            Traceback.Instance.ThrowException(
                new BifyTypeError($"Cannot auto cast {this.GetTypeName()} to {desiredType.Name}")
            );
            return null;
        }
        public virtual BifyValue Index(BifyValue indexValue,LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyTypeError($"{GetTypeName()} doesn't support Index"));
            return null;

        }
        public virtual BifyValue Not(LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyTypeError($"{GetTypeName()} doesn't support Not"));
            return null;
        }
        public virtual BifyValue Equal(BifyValue other, LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyTypeError($"{GetTypeName()} doesn't support Equal"));

            return new BoolType().Create(other.llvmValue == llvmValue ? 1 : 0);
        }
        public virtual BifyValue NotEqual(BifyValue other, LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyTypeError($"{GetTypeName()} doesn't support NotEqual"));
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
        public override string ToString()
        {
            return $"{GetTypeName()}({llvmValue})";
        }
    }
}
