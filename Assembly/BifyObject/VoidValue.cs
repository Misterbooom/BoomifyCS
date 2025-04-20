using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{

    class VoidType : BifyType
    {
        public VoidType() : base("void", LLVMTypeRef.Void)
        { }
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
    class NullType : BifyPointerType
    {
        public NullType(BifyType pointedType) : base(pointedType) { }

        public static BifyValue Create(BifyType targetPointerType)
        {
            if (targetPointerType is BifyPointerType pointerType)
            {
                return new NullValue(pointerType, LLVMValueRef.CreateConstPointerNull(pointerType.LLVMType))
                {
                    ValueFlag = ValueFlag.Constant,
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

    class NullValue : PointerValue
    {
        public NullValue(BifyPointerType type, LLVMValueRef valueRef) : base(valueRef, type) { }
        public NullValue() : base(LLVMValueRef.CreateConstPointerNull(LLVMTypeRef.CreatePointer(LLVMTypeRef.Void, 0)), new NullType(new VoidType())) { }
    }
    class TypeValue: BifyValue
    {
        public TypeValue(BifyType bifyType) : base(null, bifyType)
        {

        }
    }


}
