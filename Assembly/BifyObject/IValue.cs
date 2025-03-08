using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly
{

    public interface IValue
    {
        LLVMValueRef GetLLVMValue();  
        string GetTypeName();         
    }

}
