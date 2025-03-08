using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    public abstract class BifyType:BifyValue
    {
        public string Name { get; }
        public LLVMTypeRef LLVMType { get; }

        protected BifyType(string name, LLVMTypeRef llvmType):base(null,name)
        {
            Name = name;
            LLVMType = llvmType;
        }
        public override bool CompareType(IValue other)
        {
            return other.GetLLVMValue().TypeOf == LLVMType || other is BifyType type && type.LLVMType == LLVMType || other.GetTypeName() == Name;
        }
        public abstract BifyValue Create(object value);
        public abstract BifyValue CreateByValueRef(LLVMValueRef value);

    }
}
