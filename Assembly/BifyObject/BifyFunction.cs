using System;
using System.Linq;
using LLVMSharp.Interop;
#nullable enable
namespace BoomifyCS.Assembly.BifyObject
{
    internal class BifyFunction(
        LLVMValueRef value,
        FunctionArgs args,
        BifyType returnType,
        LLVMTypeRef type,
        bool isVariadic = false)
        : BifyValue(value, new FunctionType(type))
    {
        public FunctionArgs FunctionArgs = args;
        public BifyType ReturnType = returnType;
        public LLVMTypeRef TypeRef = type;
        public bool IsVariadic = isVariadic;
        public bool IsMethod = false;
        public ClassValue? ParentClass;

        public override BifyValue Call(BifyValue[] args)
        {
            return ReturnType.CreateValueRef(
                AssemblyCompiler.Instance.Builder.BuildCall2(
                     TypeRef,
                     GetLlvmValue(),
                     args.Select(item => item.GetLlvmValue()).ToArray(),
                     ReturnType is not VoidType? "calltmp":"" 
                 )
            );
        }
        public override string ToString()
        {
            return $"{(IsMethod ? ParentClass?.GetTypeName() + "." : "")}{GetTypeName()}({FunctionArgs}) -> {ReturnType}";
        }
    }

    internal class FunctionType(LLVMTypeRef type) : BifyType("callable", type)
    {
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
