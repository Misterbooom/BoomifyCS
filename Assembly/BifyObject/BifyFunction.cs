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
        public bool IsMethod = false;
        public BifyFunction(LLVMValueRef value, FunctionArgs args, BifyType returnType, LLVMTypeRef type, bool isVariadic = false) 
            : base(value,new FunctionType(type))
        {
            FunctionArgs = args;
            IsVariadic = isVariadic;
            ReturnType = returnType;
            TypeRef = type;
        }
        public override BifyValue Call(BifyValue[] args)
        {
            return ReturnType.CreateValueRef(
                AssemblyCompiler.Instance.Builder.BuildCall2(
                     TypeRef,
                     GetLLVMValue(),
                     args.Select(item => item.GetLLVMValue()).ToArray(),
                     ReturnType is not VoidType? "calltmp":"" 
                 )
            );
        }
    }
    class FunctionType : BifyType
    {
        public FunctionType(LLVMTypeRef type):base("callable", type)
        {
        }
        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            throw new NotImplementedException();
        }
        public override BifyValue Create(object value)
        {
            throw new NotImplementedException();
        }
        public override uint Size()
        {
            return 0;
        }

    }
}
