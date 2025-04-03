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
    class CharValue : BifyValue
    {
        static LLVMTypeRef strCompType;
        static LLVMValueRef strComp;
        public CharValue(LLVMValueRef value) : base(value, new CharType())
        {
            if (strCompType.Handle == IntPtr.Zero)
            {
                strCompType = LLVMTypeRef.CreateFunction(LLVMTypeRef.Int1,
                    new LLVMTypeRef[] { LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0), LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0) }
                );
            }
            if (strComp.Handle == IntPtr.Zero)
            {
                strComp = AssemblyCompiler.Instance.Module.AddFunction("strcmp", strCompType);
            }
        }

        public override BifyValue Equal(BifyValue other, LLVMBuilderRef builder)
        {
            if (!CompareType(other))
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"Cannot compare {this.GetTypeName()} with {other.GetTypeName()}"));
                return null;
            }
            //var value = builder.BuildCall2(strCompType, strComp, new LLVMValueRef[] { GetLLVMValue(), other.GetLLVMValue() }, "str_comp");
            //var castedValue = builder.BuildIntCast(value, LLVMTypeRef.Int1, "int32_to_int1");
            var compareValue = builder.BuildICmp(LLVMIntPredicate.LLVMIntEQ,GetLLVMValue(),other.GetLLVMValue());
            return new BoolType()
                .CreateValueRef(compareValue);

        }
        public override BifyValue NotEqual(BifyValue other, LLVMBuilderRef builder)
        {
            if (!CompareType(other))
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"Cannot compare {this.GetTypeName()} with {other.GetTypeName()}"));
                return null;
            }
            //var value = builder.BuildCall2(strCompType, strComp, new LLVMValueRef[] { GetLLVMValue(), other.GetLLVMValue() }, "str_comp");
            //var castedValue = builder.BuildIntCast(value, LLVMTypeRef.Int1, "int32_to_int1");
            var compareValue = builder.BuildICmp(LLVMIntPredicate.LLVMIntNE, GetLLVMValue(), other.GetLLVMValue());
            return new BoolType()
                .CreateValueRef(compareValue);

        }
    }
    class CharType : BifyType
    {
        public CharType() : base("char", LLVMTypeRef.Int8)
        {

        }
        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new CharValue(value);
        }
        public override BifyValue Create(object value)
        {
            unsafe
            {
                if (value is int intValue)
                {
                    return new CharValue(LLVM.ConstInt(LLVMTypeRef.Int8, (ulong)intValue, 0));
                }
                else if (value is char charValue)
                {
                    return new CharValue(LLVM.ConstInt(LLVMTypeRef.Int8, (ulong)charValue, 0));
                }
                else
                {
                    throw new Exception("Invalid value type for char");
                }
            }
        }
        
        public override uint Size()
        {
            return 1;
        }
    }
    class ConstStringType : BifyType
    {
        public ConstStringType()
            : base("string", LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0))
        {
        }

        public override BifyValue Create(object value)
        {
            var stringValue = AssemblyCompiler.Instance.Builder.BuildGlobalStringPtr(
                ((string)value), (string)value);
            var pointerType = new BifyPointerType(new CharType());
            return pointerType.CreateValueRef(stringValue);
        }


        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new BifyPointerType(new CharType()).CreateValueRef(value);
        }   
        public override uint Size()
        {
            return 1;
        }
    }


}
