using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLVMSharp.Interop;

namespace BoomifyCS.Objects
{
    class BifyVoid : BifyObject
    {
        public static new LLVMTypeRef LLVMType => LLVMTypeRef.Void;
        public BifyVoid() : base()
        {
        }
        public override BifyString Repr()
        {
            return new BifyString("BifyVoid()");
        }
        

    }
}
