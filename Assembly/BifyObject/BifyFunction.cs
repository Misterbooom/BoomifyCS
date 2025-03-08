using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLVMSharp;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    class BifyFunction : BifyValue
    {
        public FunctionArgs FunctionArgs;
        public BifyType ReturnType;
        public LLVMTypeRef TypeRef;
        public bool IsVariadic;
        public BifyFunction(LLVMValueRef value, FunctionArgs args, BifyType returnType, LLVMTypeRef type, bool isVariadic = false) : base(value, "callable")
        {
            FunctionArgs = args;
            IsVariadic = isVariadic;
            ReturnType = returnType;
            TypeRef = type;
        }
        public override BifyValue Call(BifyValue[] args)
        {
            return ReturnType.CreateByValueRef(
                AssemblyCompiler.Instance.builder.BuildCall2(
                     TypeRef,
                     GetLLVMValue(),
                     args.Select(item => item.GetLLVMValue()).ToArray(),
                     "calltmp"
                 )
            );
        }
    }
}
