using System;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{

    class VoidType() : BifyType("void", LLVMTypeRef.Void)
    {
        public override BifyValue Create(object value)
        {
            return new NullValue();
        }

        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new NullValue();
        }
        public override uint Size()
        {
            return 0;
        }
    }
    class NullType(BifyType pointedType) : BifyPointerType(pointedType)
    {
        public static BifyValue Create(BifyType targetPointerType)
        {
            if (targetPointerType is BifyPointerType pointerType)
            {
                return new NullValue(pointerType, LLVMValueRef.CreateConstPointerNull(pointerType.LlvmType))
                {
                    ValueFlag = ValueFlag.CONSTANT,
                };
            }
            throw new InvalidOperationException("Null can only be assigned to a pointer type.");
        }

        public override bool CompareType(BifyType other)
        {
            return other is BifyPointerType;
        }

        public override uint Size()
        {
            return 8;
        }
    }

    class NullValue(BifyPointerType type, LLVMValueRef valueRef) : PointerValue(valueRef, type)
    {
        public NullValue() : this(new NullType(new VoidType()), LLVMValueRef.CreateConstPointerNull(LLVMTypeRef.CreatePointer(LLVMTypeRef.Void, 0))) { }
    }
    class TypeValue(BifyType bifyType) : BifyValue(null, bifyType);


}
