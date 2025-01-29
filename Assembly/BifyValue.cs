using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Objects;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly
{
    class BifyValue
    {
        BifyObject bifyObject;
        LLVMValueRef valueRef;
        public BifyValue(BifyObject bifyObject,LLVMValueRef valueRef)
        {
            this.bifyObject = bifyObject;
            this.valueRef = valueRef;
        }
        public BifyObject GetBifyObject()
        {
            return bifyObject;
        }
        public LLVMValueRef GetValueRef()
        {
            return valueRef;
        }


    }
}
