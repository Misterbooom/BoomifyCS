using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    class StringValue : BifyValue
    {

        public StringValue(LLVMValueRef value) : base(value, "string")
        {
        }
    }
    class StringType:BifyType
    {
        public StringType() : base("string", LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0))
        {
        }
        public override BifyValue Create(object value)
        {
            return new StringValue(AssemblyCompiler.Instance
                .builder.BuildGlobalStringPtr((string)value, (string)value));
        }
        public override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new StringValue(value);
        }


    }
}
