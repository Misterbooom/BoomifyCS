using System;
using System.Collections.Generic;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    public class CharValue(LLVMValueRef value) : BifyValue(value, new CharType())
    {
        public override BifyValue Equal(BifyValue other, LLVMBuilderRef builder)
        {
            if (!CompareType(other))
            {
                Traceback.Instance.ThrowException(
                    new BifyTypeError($"Cannot compare {this.GetTypeName()} with {other.GetTypeName()}"));
                return null;
            }

            var compareValue = builder.BuildICmp(LLVMIntPredicate.LLVMIntEQ, GetLlvmValue(), other.GetLlvmValue());
            return new BoolType().CreateValueRef(compareValue);
        }

        public override BifyValue NotEqual(BifyValue other, LLVMBuilderRef builder)
        {
            if (!CompareType(other))
            {
                Traceback.Instance.ThrowException(
                    new BifyTypeError($"Cannot  compare {this.GetTypeName()} with {other.GetTypeName()} {this}"));
                return null;
            }

            var compareValue = builder.BuildICmp(LLVMIntPredicate.LLVMIntNE, GetLlvmValue(), other.GetLlvmValue());
            return new BoolType().CreateValueRef(compareValue);
        }

        public override BifyValue GreaterThan(BifyValue other, LLVMBuilderRef builder)
        {
            if (!this.CompareType(other))
            {
                Traceback.Instance.ThrowException(
                    new BifyTypeError($"Type error in '>' operation: cannot compare '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                                       $"Operands must be of compatible character types."));
                return null;
            }

            LLVMValueRef result = builder.BuildICmp(LLVMIntPredicate.LLVMIntSGT, GetLlvmValue(), other.GetLlvmValue(), "chartgttmp");
            return new BoolValue(result);
        }

        public override BifyValue LessThan(BifyValue other, LLVMBuilderRef builder)
        {
            if (!this.CompareType(other))
            {
                Traceback.Instance.ThrowException(
                    new BifyTypeError($"Type error in '<' operation: cannot compare '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                                       $"Operands must be of compatible character types."));
                return null;
            }

            LLVMValueRef result = builder.BuildICmp(LLVMIntPredicate.LLVMIntSLT, GetLlvmValue(), other.GetLlvmValue(), "charlttmp");
            return new BoolValue(result);
        }

        public override BifyValue LessThanOrEqual(BifyValue other, LLVMBuilderRef builder)
        {
            if (!this.CompareType(other))
            {
                Traceback.Instance.ThrowException(
                    new BifyTypeError($"Type error in '<=' operation: cannot compare '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                                       $"Operands must be of compatible character types."));
                return null;
            }

            LLVMValueRef result = builder.BuildICmp(LLVMIntPredicate.LLVMIntSLE, GetLlvmValue(), other.GetLlvmValue(), "charletmp");
            return new BoolValue(result);
        }

        public override BifyValue GreaterThanOrEqual(BifyValue other, LLVMBuilderRef builder)
        {
            if (!this.CompareType(other))
            {
                Traceback.Instance.ThrowException(
                    new BifyTypeError($"Type error in '>=' operation: cannot compare '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                                       $"Operands must be of compatible character types."));
                return null;
            }

            LLVMValueRef result = builder.BuildICmp(LLVMIntPredicate.LLVMIntSGE, GetLlvmValue(), other.GetLlvmValue(), "chargtmp");
            return new BoolValue(result);
        }

        public override BifyValue Add(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildAdd(GetLlvmValue(), other.GetLlvmValue(), "charaddtmp");
                return new CharValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(
                    new BifyArithmeticError($"Arithmetic error in addition: cannot add '{this.GetTypeName()}' and '{other.GetTypeName()}'. " +
                                              $"Operands must both be characters."));
                return null;
            }
        }

        public override BifyValue Sub(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildSub(GetLlvmValue(), other.GetLlvmValue(), "charsubtmp");
                return new CharValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(
                    new BifyArithmeticError($"Arithmetic error in subtraction: cannot subtract '{other.GetTypeName()}' from '{this.GetTypeName()}'. " +
                                              $"Operands must both be characters."));
                return null;
            }
        }

        public override BifyValue Mul(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildMul(GetLlvmValue(), other.GetLlvmValue(), "charmultmp");
                return new CharValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(
                    new BifyArithmeticError($"Arithmetic error in multiplication: cannot multiply '{this.GetTypeName()}' with '{other.GetTypeName()}'. " +
                                              $"Operands must both be characters."));
                return null;
            }
        }

        public override BifyValue Div(BifyValue other, LLVMBuilderRef builder)
        {
            if (this.CompareType(other))
            {
                LLVMValueRef result = builder.BuildSDiv(GetLlvmValue(), other.GetLlvmValue(), "chardivtmp");
                return new CharValue(result);
            }
            else
            {
                Traceback.Instance.ThrowException(
                    new BifyArithmeticError($"Arithmetic error in division: cannot divide '{this.GetTypeName()}' by '{other.GetTypeName()}'. " +
                                              $"Operands must both be characters."));
                return null;
            }
        }
    }

    internal class CharType() : BifyType("char", LLVMTypeRef.Int8)
    {
        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new CharValue(value);
        }
        public override BifyValue DefaultValue()
        {
            return Create(0);
        }

        public override BifyValue Create(object value)
        {
            unsafe
            {
                if (value is int intValue)
                {
                    return new CharValue(LLVM.ConstInt(LLVMTypeRef.Int8, (ulong)intValue, 1));
                }
                else if (value is char charValue)
                {
                    return new CharValue(LLVM.ConstInt(LLVMTypeRef.Int8, (ulong)charValue, 1));
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

    internal class ConstStringType() : BifyType("string", LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0))
    {
        private static readonly Dictionary<string, LLVMValueRef> GlobalStringCache = new();

        public override BifyValue Create(object value)
        {
            string stringValue = (string)value;

            if (GlobalStringCache.TryGetValue(stringValue, out LLVMValueRef existingValue))
            {
                var pointerType = new BifyPointerType(new CharType());
                return pointerType.CreateValueRef(existingValue);
            }

            var newStringValue = AssemblyCompiler.Instance.Builder.BuildGlobalStringPtr(stringValue, stringValue);
            GlobalStringCache[stringValue] = newStringValue;

            var newPointerType = new BifyPointerType(new CharType());
            return newPointerType.CreateValueRef(newStringValue);
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
