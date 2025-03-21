using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    class ArrayValue : BifyValue
    {
        public BifyValue[] Items;
        public ArrayValue(LLVMValueRef value, ArrayType type) : base(value, type) { }

        public override BifyValue Index(BifyValue indexValue, LLVMBuilderRef builder)
        {
            unsafe
            {
                LLVMValueRef zeroIndex = LLVM.ConstInt(LLVM.Int32Type(), 0, 0);

                LLVMValueRef[] indices = new LLVMValueRef[] { zeroIndex, indexValue.GetLLVMValue() };

                LLVMValueRef gep = builder.BuildInBoundsGEP2(GetBifyType().LLVMType, GetLLVMValue(), indices, "arrayIndex");

                ArrayType arrayType = (ArrayType)GetBifyType();
                //LLVMValueRef loadedValue = builder.BuildLoad2(arrayType.ItemType.LLVMType, gep, "loadArrayElem");
                
                return new BifyPointerType(arrayType.ItemType).CreateValueRef(gep);
            }
        }
    }

    class ArrayType : BifyType
    {
        public BifyType ItemType { get; private set; }
        public uint ElementCount { get; private set; }

        public ArrayType(BifyType itemType, uint elementCount)
            : base(itemType.Name + "[]", LLVMTypeRef.CreateArray(itemType.LLVMType, elementCount))
        {
            ItemType = itemType;
            ElementCount = elementCount;
        }

        public void SetElementCount(uint elementCount)
        {
            ElementCount = elementCount;
            LLVMType = LLVMTypeRef.CreateArray(ItemType.LLVMType, elementCount);
        }

        public override BifyValue Create(object value)
        { 
            BifyValue[] bifyValues = (BifyValue[])value;
            LLVMValueRef[] values = bifyValues.Select(item => item.GetLLVMValue()).ToArray();

            LLVMValueRef arrayConst = LLVMValueRef.CreateConstArray(LLVMType, values);

            return new ArrayValue(arrayConst, this);
        }
        public override uint Size()
        {
            return ItemType.Size() * ElementCount;
        }
        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new ArrayValue(value, this);
        }
    }
}
