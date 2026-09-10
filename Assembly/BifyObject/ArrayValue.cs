using System;
using System.Collections.Generic;
using System.Linq;
using BoomifyCS.Assembly.Builtin;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    internal class ArrayValue(LLVMValueRef value, ArrayType type) : BifyValue(value, type)
    {
        public BifyValue[] Items = [];

        public override BifyValue Index(BifyValue indexValue, LLVMBuilderRef builder, bool loadPointer = true)
        {
            if (indexValue.GetBifyType() is not IntegerType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError(
                    $"Array Index operator only supports integer types, but received type: {indexValue.GetBifyType().Name}"));
            }

            var checkArrayIndex = new CheckArrayIndex();
            var maxIndex = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, ((ArrayType)GetBifyType()).ElementCount - 1, false);
            checkArrayIndex.Call([new IntegerType().CreateValueRef(maxIndex), indexValue]);

            LLVMValueRef[] indices = [ indexValue.GetLlvmValue() ];
            ArrayType arrayType = (ArrayType)GetBifyType();

            LLVMValueRef gep = builder.BuildInBoundsGEP2(arrayType.ItemType.LlvmType, GetLlvmValue(), indices, "arrayIndex");

            if (loadPointer)
            {   
                LLVMValueRef loaded = builder.BuildLoad2(arrayType.ItemType.LlvmType, gep);
                return arrayType.ItemType.CreateValueRef(loaded);
            }

            var pointerType = new BifyPointerType(arrayType.ItemType);
            return pointerType.CreateValueRef(gep);
        }

        public BifyValue ZeroIndex(LLVMBuilderRef builder)
        {
            unsafe
            {
                LLVMValueRef zeroIndex = LLVM.ConstInt(LLVM.Int32Type(), 0, 0);
                LLVMValueRef[] indices = new LLVMValueRef[] { new IntegerValue(zeroIndex).GetLlvmValue() };
                LLVMValueRef gep = builder.BuildInBoundsGEP2(((ArrayType)GetBifyType()).ItemType.LlvmType,
                    GetLlvmValue(), indices, "arrayIndex");
                ArrayType arrayType = (ArrayType)GetBifyType();
                return new BifyPointerType(arrayType.ItemType).CreateValueRef(gep);
            }
        }
    }

    internal class ArrayType(BifyType itemType, uint elementCount) : BifyType(itemType.Name + $"[{elementCount}]",
        LLVMTypeRef.CreateArray(itemType.LlvmType, elementCount))
    {
        public BifyType ItemType { get; private set; } = itemType;
        public uint ElementCount { get; private set; } = elementCount;

        public void SetElementCount(uint elementCount)
        {
            ElementCount = elementCount;
            LlvmType = LLVMTypeRef.CreateArray(ItemType.LlvmType, elementCount);
            Name = itemType.Name + $"[{elementCount}]";
        }

        public override bool CompareType(BifyType other)
        {
            return base.CompareType(other) && ((ArrayType)other).ElementCount == ElementCount; 
        }

        public override BifyValue Create(object value)
        {
            BifyValue[] bifyValues = (BifyValue[])value;
            LLVMValueRef[] values = bifyValues.Select(item => item.GetLlvmValue()).ToArray();

            LLVMValueRef arrayConst = LLVMValueRef.CreateConstArray(LlvmType, values);

            return new ArrayValue(arrayConst, this);
        }

        public override uint Size()
        {
            return ItemType.Size() * ElementCount;
        }

        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new ArrayValue(value, this);
        }

        public override BifyValue DefaultValue()
        {
            LLVMTypeRef arrayType = LLVMTypeRef.CreateArray(ItemType.LlvmType, (uint)ElementCount);
            LLVMValueRef arrayConst = LLVMValueRef.CreateConstNull(arrayType);
            return new ArrayValue(arrayConst, this);
        }
    }

    internal class CheckArrayIndex : BifyFunction
    {
        private bool _needToInit = true;

        public CheckArrayIndex() : base(null, null, null, null)
        {
            var arguments = new Dictionary<string, BifyType>
            {
                { "maxIndex", new IntegerType() },
                { "index", new IntegerType() }
            };

            FunctionArgs = new FunctionArgs(null);
            FunctionArgs.SetArguments(arguments);
            ReturnType = new VoidType();
            IsVariadic = false;
        }

        private void Init()
        {
            TypeRef = LLVMTypeRef.CreateFunction(
                ReturnType.LlvmType,
                FunctionArgs.LlvmTypes,
                false);
            var compiler = AssemblyCompiler.Instance;
            LlvmValue = compiler.Module.AddFunction("arrayIndexCheck", TypeRef);
            var entry = LlvmValue.AppendBasicBlock("entry");
            compiler.Builder.PositionAtEnd(entry);
            LLVMValueRef maxIndex = LlvmValue.GetParam(0);
            LLVMValueRef arrayIndex = LlvmValue.GetParam(1);

            var thenBlock = LlvmValue.AppendBasicBlock("then");
            var elseBlock = LlvmValue.AppendBasicBlock("else");
            LLVMValueRef condition =
                compiler.Builder.BuildICmp(LLVMIntPredicate.LLVMIntUGT, arrayIndex, maxIndex, "indexCheck");
            compiler.Builder.BuildCondBr(condition, thenBlock, elseBlock);
            compiler.Builder.PositionAtEnd(thenBlock);
            StdC.RaiseError(new BifyIndexError("Array index out of bounds"));
            compiler.Builder.BuildUnreachable();
            compiler.Builder.PositionAtEnd(elseBlock);
            compiler.Builder.BuildRetVoid();
        }

        public override BifyValue Call(BifyValue[] args)
        {
            var existedFunction = AssemblyCompiler.Instance.Module.GetNamedFunction("arrayIndexCheck");
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

            if (args.Length != 2)
            {
                Traceback.Instance.ThrowException(
                    new BifyArgumentError($"Expected 2 arguments but got {args.Length}."));
            }

            var res = AssemblyCompiler.Instance.Builder.BuildCall2(TypeRef, LlvmValue,
                args.Select(item => item.GetLlvmValue()).ToArray());
            return new VoidType().Create(null);
        }
    }
}