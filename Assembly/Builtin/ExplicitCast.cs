using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.Builtin
{
    internal class ExplicitCastHandler(BifyValue bifyValue)
    {
        public BifyValue PerformExplicitCast(BifyType desiredType, LLVMBuilderRef builder)
        {
            if (desiredType.CompareType(bifyValue.GetBifyType()))
                return bifyValue;

            if (desiredType is BifyPointerType desiredPtr)
                return HandlePointerCast(desiredPtr, builder);

            if (bifyValue.GetBifyType() is IntegerType && desiredType is FloatType)
                return CastIntegerToFloat(desiredType, builder);

            if (bifyValue.GetBifyType() is CharType && desiredType is IntegerType)
                return HandleCharCast((CharValue)bifyValue,builder);

            if (bifyValue.GetBifyType() is FloatType && desiredType is IntegerType)
                return CastFloatToInteger(desiredType, builder);

            Traceback.Instance.ThrowException(
                new BifyTypeError($"Cannot explicit cast {bifyValue.GetTypeName()} to {desiredType.Name}")
            );
            return null;
        }
        private BifyValue HandleCharCast(CharValue value, LLVMBuilderRef builder)
        {
            var res = builder.BuildSExt(value.GetLlvmValue(), LLVMTypeRef.Int32, "char_to_int");
            return new IntegerValue(res);
        }

        private BifyValue HandlePointerCast(BifyPointerType desiredPtr, LLVMBuilderRef builder)
        {
            if (desiredPtr.PointedType.CompareType(bifyValue.GetBifyType()))
                return desiredPtr.CreateValueRef(bifyValue.GetLlvmValue());

            if (bifyValue.GetBifyType() is NullType)
                return NullType.Create(desiredPtr);
            if (bifyValue.GetBifyType().CompareType(typeof(AnyType)))
            {
                return desiredPtr.CreateValueRef(bifyValue.GetLlvmValue());
            }
            if (desiredPtr is AnyType)
            {
                return new AnyType().CreateValueRef(bifyValue.GetLlvmValue());
            }

            return null;
        }

     

        private BifyValue CastIntegerToFloat(BifyType desiredType, LLVMBuilderRef builder)
        {
            LLVMValueRef floatValue = builder.BuildSIToFP(bifyValue.GetLlvmValue(), desiredType.LlvmType, "cast_int_to_float");
            return new FloatValue(floatValue);
        }

        private BifyValue CastFloatToInteger(BifyType desiredType, LLVMBuilderRef builder)
        {
            LLVMValueRef intValue = builder.BuildFPToSI(bifyValue.GetLlvmValue(), desiredType.LlvmType, "cast_float_to_int");
            return new IntegerValue(intValue);
        }
    }
}
