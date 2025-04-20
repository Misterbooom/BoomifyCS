using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLVMSharp;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    class ClassValue:BifyValue
    {
        public ClassValue(LLVMValueRef value, BifyType type) : base(value, type) { }

    }
    class ClassType: BifyType
    {
        public ClassType(string name, LLVMTypeRef structType): base(name,structType) { 
        }

        public override uint Size() 
        {
            return 0;
        }
        public override BifyValue Create(object value)
        {
            throw new NotImplementedException();
        }
        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new ClassValue(value, this);
        } 
    }
}
