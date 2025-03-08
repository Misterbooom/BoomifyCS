using System;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    public abstract class BifyValue : IValue
    {

        protected LLVMValueRef llvmValue;
        protected string typeName;

        public BifyValue(LLVMValueRef value, string typeName)
        {
            this.llvmValue = value;
            this.typeName = typeName;
        }

        public LLVMValueRef GetLLVMValue() => llvmValue;

        public string GetTypeName() => typeName;
        public virtual bool CompareType(IValue other) => GetType() == other.GetType();
        public virtual BifyValue Call(BifyValue[] args)
        {
            Traceback.Instance.ThrowException(new BifyTypeError($"{typeName} is not callable"));
            return null;
        }

        public virtual BifyValue Add(BifyValue other, LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyArithmeticError($"{typeName} doesn't support Add"));
            return null;
        }

        public virtual BifyValue Sub(BifyValue other, LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyArithmeticError($"{typeName} doesn't support Sub"));
            return null;
        }

        public virtual BifyValue Mul(BifyValue other, LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyArithmeticError($"{typeName} doesn't support Mul"));
            return null;
        }

        public virtual BifyValue Div(BifyValue other, LLVMBuilderRef builder)
        {
            Traceback.Instance.ThrowException(new BifyArithmeticError($"{typeName} doesn't support Div"));
            return null;
        }
    }
}
