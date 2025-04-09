using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.Builtin;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    class ArrayValue : BifyValue
    {
        public BifyValue[] Items;
        public ArrayValue(LLVMValueRef value, ArrayType type) : base(value, type) { }

        public override BifyValue Index(BifyValue indexValue, LLVMBuilderRef builder)
        {
            if (indexValue.GetBifyType() is not IntegerType indexType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"Array Index operator only supports integer types, but received type: {indexValue.GetBifyType().Name}"));
            }
            var checkArrayIndex = new CheckArrayIndex();
            var maxIndex = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, ((ArrayType)GetBifyType()).ElementCount - 1, false);
            var checkIndex = checkArrayIndex.Call([ new IntegerType().CreateValueRef(maxIndex), indexValue ]);

            unsafe
            {
                LLVMValueRef zeroIndex = LLVM.ConstInt(LLVM.Int32Type(), 0, 0);

                LLVMValueRef[] indices = new LLVMValueRef[] {  indexValue.GetLLVMValue() };

                LLVMValueRef gep = builder.BuildInBoundsGEP2(((ArrayType)GetBifyType()).ItemType.LLVMType, GetLLVMValue(), indices, "arrayIndex");

                ArrayType arrayType = (ArrayType)GetBifyType();
                
                return new BifyPointerType(arrayType.ItemType).CreateValueRef(gep);
            }
        }
        public BifyValue ZeroIndex(LLVMBuilderRef builder)
        {
            unsafe
            {
                LLVMValueRef zeroIndex = LLVM.ConstInt(LLVM.Int32Type(), 0, 0);
                LLVMValueRef[] indices = new LLVMValueRef[] { new IntegerValue(zeroIndex).GetLLVMValue() };
                LLVMValueRef gep = builder.BuildInBoundsGEP2(((ArrayType)GetBifyType()).ItemType.LLVMType, GetLLVMValue(), indices, "arrayIndex");
                ArrayType arrayType = (ArrayType)GetBifyType();
                BifyDebug.Log($"Zero index: {gep}");
                return new BifyPointerType(arrayType.ItemType).CreateValueRef(gep);
            }
        }
    }

    class ArrayType : BifyType
    {
        public BifyType ItemType { get; private set; }
        public uint ElementCount { get; private set; }

        public ArrayType(BifyType itemType, uint elementCount)
            : base(itemType.Name + "[]", LLVMTypeRef.CreatePointer(LLVMTypeRef.CreateArray(itemType.LLVMType, elementCount),0))
        {
            ItemType = itemType;
            ElementCount = elementCount;
        }

        public void SetElementCount(uint elementCount)
        {
            ElementCount = elementCount;
            LLVMType = LLVMTypeRef.CreateArray(ItemType.LLVMType, elementCount);
        }

        public override BifyValue Create(object value)
        { 
            BifyValue[] bifyValues = (BifyValue[])value;
            LLVMValueRef[] values = bifyValues.Select(item => item.GetLLVMValue()).ToArray();

            LLVMValueRef arrayConst = LLVMValueRef.CreateConstArray(LLVMType, values);

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
    }
   class CheckArrayIndex: BifyFunction
    {
        private bool needToInit = true;
        public CheckArrayIndex() : base(null, null, null, null)
        {
            var arguments = new Dictionary<string, BifyType> {

                {"maxIndex", new IntegerType()},
                {"index", new IntegerType()}
            };
          
            FunctionArgs = new FunctionArgs(null);
            FunctionArgs.SetArguments(arguments);
            ReturnType = new VoidType();
            IsVariadic = false;
        }
        private void Init()
        {
            TypeRef = LLVMTypeRef.CreateFunction(
                ReturnType.LLVMType,
                FunctionArgs.LLVMTypes,
                false);
            var compiler = AssemblyCompiler.Instance;
            llvmValue = compiler.Module.AddFunction("arrayIndexCheck", TypeRef);
            var entry = llvmValue.AppendBasicBlock("entry");
            compiler.Builder.PositionAtEnd(entry);
            LLVMValueRef maxIndex = llvmValue.GetParam(0);
            LLVMValueRef arrayIndex = llvmValue.GetParam(1);

            var thenBlock = llvmValue.AppendBasicBlock("then");
            var elseBlock = llvmValue.AppendBasicBlock("else");
            LLVMValueRef condition = compiler.Builder.BuildICmp(LLVMIntPredicate.LLVMIntUGT, arrayIndex, maxIndex, "indexCheck");
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
                llvmValue = existedFunction;
                TypeRef = LLVMTypeRef.CreateFunction(ReturnType.LLVMType,existedFunction.TypeOf.ParamTypes);
                BifyDebug.Log($"Setting func value: {llvmValue} type: {TypeRef}");
            }
            if (args.Length != 2)
            {
                Traceback.Instance.ThrowException(new BifyArgumentError($"Expected 2 arguments but got {args.Length}."));
            }
            var res = AssemblyCompiler.Instance.Builder.BuildCall2(TypeRef, llvmValue, args.Select(item => item.GetLLVMValue()).ToArray());
            return new VoidType().Create(null);



        }
    }
}
