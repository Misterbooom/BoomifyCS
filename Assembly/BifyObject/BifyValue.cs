using System;
using System.Threading.Tasks.Sources;
using BoomifyCS.Assembly.Builtin;
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
        
        public virtual BifyValue GetAttribute(string name,BifyType other, LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyAttributeError($"{GetTypeName() + ToString()} doesn't support dot operator."));
            return null;
        }
      
        public BifyValue ExplicitCast(BifyType desiredType,LLVMBuilderRef builder)
        {
            return new ExplicitCastHandler(this).PerformExplicitCast(desiredType,builder);
        }
        public virtual BifyValue Index(BifyValue indexValue, LLVMBuilderRef builder)
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
            return $"{ValueFlag} {GetTypeName()}({llvmValue})";
        }
    }
}
