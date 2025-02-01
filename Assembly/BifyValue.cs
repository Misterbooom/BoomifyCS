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
        public BifyValue(BifyObject bifyObject)
        {
            this.bifyObject = bifyObject;
        }
        public BifyObject GetBifyObject()
        {
            return bifyObject;
        }
        
        public LLVMValueRef GetValueRef()
        {
            return bifyObject.ToLLVM();
        }
        public void SetBifyObject(BifyObject obj) {
            bifyObject = obj;
        }
        public override string  ToString()
        {
            return $"BifyValue({bifyObject.Repr()})";

        }
    }
}
