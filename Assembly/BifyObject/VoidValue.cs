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

        public override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new NullValue();
        }
    }
    class NullType : BifyType
    {
        public NullType() : base("null", LLVMTypeRef.Void)
        { }
        public override BifyValue Create(object value)
        {
            return new NullValue();
        }
        public override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new NullValue();
        }
    }
    class NullValue : BifyValue
    {
        public NullValue() : base(LLVMValueRef.CreateConstNull(LLVMTypeRef.Void), new NullType())
        { }
        public override BifyValue Add(BifyValue other, LLVMBuilderRef builder)
        {
            return null;
        }
    }

}
