using System;
using System.Collections.Generic;
using System.Linq;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Exceptions;
using LLVMSharp;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.Builtin
{
    class Input : BifyFunction
    {
        bool needToInit = true;

        public Input() : base(null, null, null, null)
        {
            FunctionArgs = new FunctionArgs(null);
            ReturnType = new BifyPointerType(new CharType());
            IsVariadic = false;
            FunctionArgs.SetArguments(new Dictionary<string, BifyType> { { "prompt", new BifyPointerType(new CharType())} });
        }

        private void InitFunction()
        {
            var builder = AssemblyCompiler.Instance.Builder;
            var module = AssemblyCompiler.Instance.Module;
            var context = module.Context;

            var scanfType = LLVMTypeRef.CreateFunction(
                LLVMTypeRef.Int32,
                new LLVMTypeRef[] { LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0) },
                true);
            var scanfFunc = module.AddFunction("scanf", scanfType);
            scanfFunc.Linkage = LLVMLinkage.LLVMExternalLinkage;

            TypeRef = LLVMTypeRef.CreateFunction(LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0), FunctionArgs.LLVMTypes, false);
            llvmValue = module.AddFunction("input", TypeRef);

            var entry = llvmValue.AppendBasicBlock("entry");
            builder.PositionAtEnd(entry);
            var printFunction = AssemblyCompiler.Instance.VariableManager.GetBifyValue("explode");
            var prompt = new BifyPointerType(new CharType()).CreateValueRef(llvmValue.GetParam(0));
            printFunction.Call([prompt]);

            var bufferType = LLVMTypeRef.CreateArray(LLVMTypeRef.Int8, 256);
            var textVar = builder.BuildAlloca(bufferType, "text");

            var zero = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 0, false);
            var textPtr = builder.BuildGEP2(bufferType, textVar, new LLVMValueRef[] { zero, zero }, "textPtr");

            var formatStr = new ConstStringType().Create("%s").GetLLVMValue();

            builder.BuildCall2(scanfType, scanfFunc, new LLVMValueRef[] { formatStr, textPtr }, "callscanf");

            builder.BuildRet(textPtr);
        }


        public override BifyValue Call(BifyValue[] args)
        {
            if (needToInit)
            {
                var entryBlock = AssemblyCompiler.Instance.Builder.InsertBlock;
                InitFunction();
                needToInit = false;
                AssemblyCompiler.Instance.Builder.PositionAtEnd(entryBlock);
            }

            var res = AssemblyCompiler.Instance.Builder.BuildCall2(TypeRef, llvmValue, args.Select(i => i.GetLLVMValue()).ToArray(),"_");
            BifyDebug.Log($"Input function log :{res}");
            return new ConstStringType().CreateValueRef(
                res
                );
        }
    }
}
