using System;
using System.Collections.Generic;
using System.Linq;
using LLVMSharp.Interop;
using BoomifyCS.Exceptions;
using BoomifyCS.Assembly.Builtin;

namespace BoomifyCS.Assembly.BifyObject
{
    internal class AllocaPointer(LLVMValueRef value, BifyPointerType type) : PointerValue(value, type);

    internal class AllocaType(BifyType pointedType) : BifyPointerType(pointedType)
    {
        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new AllocaPointer(value, this);
        }
    }

    internal class PointerValue(LLVMValueRef value, BifyPointerType pointerType) : BifyValue(value, pointerType)
    {
        private static readonly NullPointerCheck NullPointerCheck = new NullPointerCheck();
        public void CheckNull(string operation)
        {
#if (DEBUG_COMPILE)
            NullPointerCheck.Call(new BifyValue[] { this });
#endif
        }

        public override BifyValue Add(BifyValue other, LLVMBuilderRef builder)
        {
            CheckNull("pointer addition");
            if (!(other.GetBifyType() is IntegerType))
            {
                Traceback.Instance.ThrowException(
                    new BifyTypeError("Pointer arithmetic requires an integer offset in addition."));
                return null;
            }
            LLVMValueRef[] indices = new LLVMValueRef[] { other.GetLlvmValue() };
            BifyPointerType pointerType = (BifyPointerType)GetBifyType();
            LLVMValueRef newPtr = builder.BuildGEP2(pointerType.PointedType.LlvmType, GetLlvmValue(), indices, "ptr_add");
            return new PointerValue(newPtr, (BifyPointerType)this.Type);
        }

        public override BifyValue Sub(BifyValue other, LLVMBuilderRef builder)
        {
            CheckNull("pointer subtraction");
            if (!(other.GetBifyType() is IntegerType))
            {
                Traceback.Instance.ThrowException(
                    new BifyTypeError("Pointer arithmetic requires an integer offset in subtraction."));
                return null;
            }
            LLVMValueRef zero = LLVMValueRef.CreateConstInt(other.GetBifyType().LlvmType, 0, false);
            LLVMValueRef negOffset = builder.BuildSub(zero, other.GetLlvmValue(), "neg_offset");
            LLVMValueRef[] indices = new LLVMValueRef[] { negOffset };
            BifyPointerType pointerType = (BifyPointerType)GetBifyType();
            LLVMValueRef newPtr = builder.BuildGEP2(pointerType.PointedType.LlvmType, GetLlvmValue(), indices, "ptr_sub");
            return GetBifyType().CreateValueRef(newPtr);
        }

        public BifyValue Dereference(bool checkOnNull)
        {
            if (checkOnNull)
                CheckNull("pointer dereference");
            var pointerType = (BifyPointerType)GetBifyType();
            LLVMValueRef loadedValue = AssemblyCompiler.Instance.Builder.BuildLoad2(pointerType.PointedType.LlvmType, GetLlvmValue(), "dereferenced_ptr");
            
            var dereferenced = pointerType.PointedType.CreateValueRef(loadedValue);
            dereferenced.ValueFlag = ValueFlag.NONE;
            return dereferenced;
        }

        public override BifyValue Index(BifyValue indexValue, LLVMBuilderRef builder, bool loadPointer = true)
        {
            if (indexValue.GetBifyType() is not IntegerType)
            {
                Traceback.Instance.ThrowException(
                    new BifyTypeError("Pointer arithmetic requires an integer offset in indexing."));
                return null;
            }

            BifyPointerType pointerType = (BifyPointerType)GetBifyType();

            if (pointerType.PointedType is ArrayType arrayType)
            {
                var checkArrayIndex = new CheckArrayIndex();
                var maxIndex = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, arrayType.ElementCount - 1, false);
                checkArrayIndex.Call([new IntegerType().CreateValueRef(maxIndex), indexValue]);

                LLVMValueRef zeroIndex = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 0, false);
                LLVMValueRef[] arrayIndices = [ zeroIndex, indexValue.GetLlvmValue() ];
        
                LLVMValueRef arrayGep = builder.BuildInBoundsGEP2(arrayType.LlvmType, GetLlvmValue(), arrayIndices, "arrayIndex");

                if (loadPointer)
                {
                    LLVMValueRef loaded = builder.BuildLoad2(arrayType.ItemType.LlvmType, arrayGep, "loaded_item");
                    return arrayType.ItemType.CreateValueRef(loaded);
                }
        
                return new BifyPointerType(arrayType.ItemType).CreateValueRef(arrayGep);
            }

            CheckNull("pointer indexing");
    
            LLVMValueRef[] ptrIndices = [ indexValue.GetLlvmValue() ];
            LLVMValueRef gep = builder.BuildGEP2(pointerType.PointedType.LlvmType, GetLlvmValue(), ptrIndices, $"ptr_index_{pointerType.PointedType.Name}");

            if (loadPointer)
            {
                LLVMValueRef loaded = builder.BuildLoad2(pointerType.PointedType.LlvmType, gep, "loaded_ptr_item");
                return pointerType.PointedType.CreateValueRef(loaded);
            }
    
            return new BifyPointerType(pointerType.PointedType).CreateValueRef(gep);
        }
    }

    internal class BifyPointerType(BifyType pointedType)
        : BifyType(pointedType.Name + "*", LLVMTypeRef.CreatePointer(pointedType.LlvmType, 0))
    {
        public BifyType PointedType { get; private set; } = pointedType;

        public override BifyValue DefaultValue()
        {
            return NullType.Create(this);
        }
        public override bool CompareType(BifyType other)
        {
            if (other is BifyPointerType otherPtr)
                return this.PointedType.CompareType(otherPtr.PointedType);
            return false;
        }

        public override BifyValue Create(object value)
        {
            throw new NotImplementedException("Create method not implemented in Pointer type");
        }
        
        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new PointerValue(value, this);
        }

        public override uint Size()
        {
            return PointedType.Size();
        }
    }

    internal class NullPointerCheck : BifyFunction
    {
        public NullPointerCheck() : base(null, null, null, null)
        {
            var arguments = new Dictionary<string, BifyType> { { "ptr", new BifyPointerType(new AnyType()) } };
            FunctionArgs = new FunctionArgs(null);
            FunctionArgs.SetArguments(arguments);
            ReturnType = new VoidType();
            IsVariadic = false;
        }

        private void Init()
        {
            TypeRef = LLVMTypeRef.CreateFunction(ReturnType.LlvmType, FunctionArgs.LlvmTypes, false);
            var compiler = AssemblyCompiler.Instance;
            LlvmValue = compiler.Module.AddFunction("nullPointerCheck", TypeRef);
            var entry = LlvmValue.AppendBasicBlock("entry");
            compiler.Builder.PositionAtEnd(entry);
            LLVMValueRef ptrArg = LlvmValue.GetParam(0);
            LLVMValueRef nullConst = new NullValue().GetLlvmValue();
            LLVMValueRef condition = compiler.Builder.BuildICmp(LLVMIntPredicate.LLVMIntEQ, ptrArg, nullConst, "nullCheck");
            var thenBlock = LlvmValue.AppendBasicBlock("then");
            var elseBlock = LlvmValue.AppendBasicBlock("else");
            compiler.Builder.BuildCondBr(condition, thenBlock, elseBlock);
            compiler.Builder.PositionAtEnd(thenBlock);
            StdC.RaiseError(new BifyNullError("Null pointer encountered."));
            compiler.Builder.BuildUnreachable();
            compiler.Builder.PositionAtEnd(elseBlock);
            compiler.Builder.BuildRetVoid();
        }

        public override BifyValue Call(BifyValue[] args)
        {
            var existedFunction = AssemblyCompiler.Instance.Module.GetNamedFunction("nullPointerCheck");
            if (existedFunction == null)
            {
                var insertBlock = AssemblyCompiler.Instance.Builder.InsertBlock;
                Init();
                AssemblyCompiler.Instance.Builder.PositionAtEnd(insertBlock);
            }
            else
            {
                LlvmValue = existedFunction;
                TypeRef = LLVMTypeRef.CreateFunction(ReturnType.LlvmType, existedFunction.TypeOf.GetParamTypes());
            }
            if (args.Length != 1)
            {
                Traceback.Instance.ThrowException(new BifyArgumentError($"Expected 1 argument but got {args.Length}."));
            }
            AssemblyCompiler.Instance.Builder.BuildCall2(TypeRef, LlvmValue, args.Select(item => item.GetLlvmValue()).ToArray());
            return new VoidType().Create(null);
        }
    }
}
