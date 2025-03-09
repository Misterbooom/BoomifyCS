using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    class StringValue : BifyValue
    {

        public StringValue(LLVMValueRef value) : base(value, new StringType())
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
            var stringValue = AssemblyCompiler.Instance
                .builder.BuildGlobalStringPtr(((string)value).Replace(@"\n", "\n"), (string)value);
            
            return new StringValue(stringValue);
        }
        public override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new StringValue(value);
        }


    }
}
