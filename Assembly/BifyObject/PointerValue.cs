using System;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    class PointerValue : BifyValue
    {
        public PointerValue(LLVMValueRef value, BifyPointerType pointerType)
            : base(value, pointerType)
        {
        }

        // Pointer addition: pointer + integer offset
        public override BifyValue Add(BifyValue other, LLVMBuilderRef builder)
        {
            if (!(other.GetBifyType() is IntegerType))
            {
                Traceback.Instance.ThrowException(
                    new BifyTypeError("Pointer arithmetic requires an integer offset."));
                return null;
            }

            LLVMValueRef[] indices = new LLVMValueRef[] { other.GetLLVMValue() };

            LLVMValueRef newPtr = builder.BuildGEP2(
                ((BifyPointerType)this.type).PointedType.LLVMType,
                this.GetLLVMValue(),
                indices,
                "ptr_add");

            return new PointerValue(newPtr, (BifyPointerType)this.type);
        }

        public override BifyValue Sub(BifyValue other, LLVMBuilderRef builder)
        {
            if (!(other.GetBifyType() is IntegerType))
            {
                Traceback.Instance.ThrowException(
                    new BifyTypeError("Pointer arithmetic requires an integer offset."));
                return null;
            }

            LLVMValueRef zero = LLVMValueRef.CreateConstInt(other.GetBifyType().LLVMType, 0, false);
            LLVMValueRef negOffset = builder.BuildSub(zero, other.GetLLVMValue(), "neg_offset");

            LLVMValueRef[] indices = new LLVMValueRef[] { negOffset };

            LLVMValueRef newPtr = builder.BuildGEP2(
                ((BifyPointerType)this.type).PointedType.LLVMType,
                this.GetLLVMValue(),
                indices,
                "ptr_sub");

            return new PointerValue(newPtr, (BifyPointerType)this.type);
        }
        public BifyValue Dereference()
        {
            var loadedValue = AssemblyCompiler.Instance.builder.BuildLoad2(GetBifyType().LLVMType, this.GetLLVMValue());
            return ((BifyPointerType)GetBifyType()).
                PointedType.
                CreateValueRef(loadedValue);
        }
    }


    class BifyPointerType : BifyType
    {
        public BifyType PointedType { get; private set; }

        public BifyPointerType(BifyType pointedType)
            : base(pointedType.Name + "*", LLVMTypeRef.CreatePointer(pointedType.LLVMType, 0))
        {
            this.PointedType = pointedType;
        }

        public override bool CompareType(BifyType other)
        {
            if (other is BifyPointerType otherPtr)
            {
                return this.PointedType.CompareType(otherPtr.PointedType);
            }
            return false;
        }

        public override BifyValue Create(object value)
        {
            throw new NotImplementedException("Create method not implemented in Pointer type");
        }

        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new PointerValue(value, this);
        }
        public override uint Size()
        {
            return PointedType.Size();
        }
    }
}
