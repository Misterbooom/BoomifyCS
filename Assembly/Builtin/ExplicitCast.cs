using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;
using NUnit.Framework.Constraints;

namespace BoomifyCS.Assembly.Builtin
{
    class ExplicitCastHandler
    {
        private BifyValue bifyValue;
        public ExplicitCastHandler(BifyValue bifyValue)
        {
            this.bifyValue = bifyValue;
        }
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
            var res = builder.BuildSExt(value.GetLLVMValue(), LLVMTypeRef.Int32, "char_to_int");
            return new IntegerValue(res);
        }

        private BifyValue HandlePointerCast(BifyPointerType desiredPtr, LLVMBuilderRef builder)
        {
            if (desiredPtr.PointedType.CompareType(bifyValue.GetBifyType()))
                return desiredPtr.CreateValueRef(bifyValue.GetLLVMValue());

            if (bifyValue.GetBifyType() is NullType)
                return NullType.Create(desiredPtr);

            if (bifyValue.GetBifyType() is ArrayType arrayType)
                return HandleArrayPointerCast(arrayType, desiredPtr, builder);

            return null;
        }

        private BifyValue HandleArrayPointerCast(ArrayType arrayType, BifyPointerType desiredPtr, LLVMBuilderRef builder)
        {
            if (desiredPtr.PointedType.CompareType(arrayType.ItemType))
            {
                var arr = new AllocaType(arrayType).CreateValueRef(builder.BuildAlloca(arrayType.LLVMType, "arr"));

                builder.BuildStore(bifyValue.GetLLVMValue(), arr.GetLLVMValue());


                var res = ((ArrayValue)arrayType.CreateValueRef(arr.GetLLVMValue())).ZeroIndex(builder);
                BifyDebug.Log($"Casting arr to pointer: {res};Array el pointer: {arrayType.ItemType}");
                return res;
            }


            string expected = arrayType.ItemType.Name;
            string provided = desiredPtr.PointedType.Name;
            Traceback.Instance.ThrowException(
                new BifyTypeError(
                    $"Cannot obtain pointer to array item: type mismatch. " +
                    $"Expected element type '{expected}' but pointer targets '{provided}'."
                )
            );
            return null;
        }

        private BifyValue CastIntegerToFloat(BifyType desiredType, LLVMBuilderRef builder)
        {
            LLVMValueRef floatValue = builder.BuildSIToFP(bifyValue.GetLLVMValue(), desiredType.LLVMType, "cast_int_to_float");
            return new FloatValue(floatValue);
        }

        private BifyValue CastFloatToInteger(BifyType desiredType, LLVMBuilderRef builder)
        {
            LLVMValueRef intValue = builder.BuildFPToSI(bifyValue.GetLLVMValue(), desiredType.LLVMType, "cast_float_to_int");
            return new IntegerValue(intValue);
        }
    }
}
