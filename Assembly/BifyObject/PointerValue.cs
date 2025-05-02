using System;
using System.Collections.Generic;
using System.Linq;
using LLVMSharp.Interop;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Exceptions;
using BoomifyCS.Assembly.Builtin;

namespace BoomifyCS.Assembly.BifyObject
{
    class AllocaPointer : PointerValue
    {
        public AllocaPointer(LLVMValueRef value, BifyPointerType type) : base(value, type) { }
    }

    class AllocaType : BifyPointerType
    {
        public AllocaType(BifyType pointedType) : base(pointedType) {
        }
        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new AllocaPointer(value, this);
        }
    }

    class PointerValue : BifyValue
    {
        private static NullPointerCheck nullPointerCheck = new NullPointerCheck();

        public PointerValue(LLVMValueRef value, BifyPointerType pointerType)
            : base(value, pointerType) { }

        private void CheckNull(string operation)
        {
#if DEBUG_COMPILE
            nullPointerCheck.Call(new BifyValue[] { this });
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
            LLVMValueRef[] indices = new LLVMValueRef[] { other.GetLLVMValue() };
            BifyPointerType pointerType = (BifyPointerType)GetBifyType();
            LLVMValueRef newPtr = builder.BuildGEP2(pointerType.PointedType.LLVMType, GetLLVMValue(), indices, "ptr_add");
            return new PointerValue(newPtr, (BifyPointerType)this.type);
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
            LLVMValueRef zero = LLVMValueRef.CreateConstInt(other.GetBifyType().LLVMType, 0, false);
            LLVMValueRef negOffset = builder.BuildSub(zero, other.GetLLVMValue(), "neg_offset");
            LLVMValueRef[] indices = new LLVMValueRef[] { negOffset };
            BifyPointerType pointerType = (BifyPointerType)GetBifyType();
            LLVMValueRef newPtr = builder.BuildGEP2(pointerType.PointedType.LLVMType, GetLLVMValue(), indices, "ptr_sub");
            return GetBifyType().CreateValueRef(newPtr);
        }

        public BifyValue Dereference()
        {
            CheckNull("pointer dereference");
            var pointerType = (BifyPointerType)GetBifyType();
            LLVMValueRef loadedValue = AssemblyCompiler.Instance.Builder.BuildLoad2(pointerType.PointedType.LLVMType, GetLLVMValue(), "dereferenced_ptr");

            var dereferenced = pointerType.PointedType.CreateValueRef(loadedValue);
            dereferenced.ValueFlag = ValueFlag.None;
            return dereferenced;
        }

        public override BifyValue Index(BifyValue indexValue, LLVMBuilderRef builder)
        {
            CheckNull("pointer indexing");
            if (indexValue.GetBifyType() is not IntegerType)
            {
                Traceback.Instance.ThrowException(
                    new BifyTypeError("Pointer arithmetic requires an integer offset in indexing."));
                return null;
            }
            LLVMValueRef[] indices = new LLVMValueRef[] { indexValue.GetLLVMValue() };
            BifyPointerType pointerType = (BifyPointerType)GetBifyType();
            LLVMValueRef newPtr = builder.BuildGEP2(pointerType.PointedType.LLVMType, GetLLVMValue(), indices, $"ptr_index_{pointerType.PointedType.Name}");
            return pointerType.CreateValueRef(newPtr);
        }
    }

    class BifyPointerType : BifyType
    {
        public BifyType PointedType { get; private set; }

        public BifyPointerType(BifyType pointedType)
            : base(pointedType.Name + "*", LLVMTypeRef.CreatePointer(pointedType.LLVMType, 0))
        {
            PointedType = pointedType;
        }
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

    class NullPointerCheck : BifyFunction
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
            TypeRef = LLVMTypeRef.CreateFunction(ReturnType.LLVMType, FunctionArgs.LLVMTypes, false);
            var compiler = AssemblyCompiler.Instance;
            llvmValue = compiler.Module.AddFunction("nullPointerCheck", TypeRef);
            var entry = llvmValue.AppendBasicBlock("entry");
            compiler.Builder.PositionAtEnd(entry);
            LLVMValueRef ptrArg = llvmValue.GetParam(0);
            LLVMValueRef nullConst = new NullValue().GetLLVMValue();
            LLVMValueRef condition = compiler.Builder.BuildICmp(LLVMIntPredicate.LLVMIntEQ, ptrArg, nullConst, "nullCheck");
            var thenBlock = llvmValue.AppendBasicBlock("then");
            var elseBlock = llvmValue.AppendBasicBlock("else");
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
                llvmValue = existedFunction;
                TypeRef = LLVMTypeRef.CreateFunction(ReturnType.LLVMType, existedFunction.TypeOf.ParamTypes);
            }
            if (args.Length != 1)
            {
                Traceback.Instance.ThrowException(new BifyArgumentError($"Expected 1 argument but got {args.Length}."));
            }
            AssemblyCompiler.Instance.Builder.BuildCall2(TypeRef, llvmValue, args.Select(item => item.GetLLVMValue()).ToArray());
            return new VoidType().Create(null);
        }
    }
}
