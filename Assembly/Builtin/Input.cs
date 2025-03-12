using System;
using System.Collections.Generic;
using System.Linq;
using BoomifyCS.Assembly.BifyObject;
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
            ReturnType = new ConstStringType();
            IsVariadic = false;
            // Expect one string parameter for the prompt.
            FunctionArgs.SetArguments(new Dictionary<string, string> { { "prompt", "str" } });
        }

        private void InitFunction()
        {
            var builder = AssemblyCompiler.Instance.builder;
            var module = AssemblyCompiler.Instance.module;
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

            var bufferType = LLVMTypeRef.CreateArray(LLVMTypeRef.Int8, 256);
            var textVar = builder.BuildAlloca(bufferType, "text");

            var zero = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 0, false);
            var textPtr = builder.BuildGEP2(bufferType, textVar, new LLVMValueRef[] { zero, zero }, "textPtr");

            var formatStr = builder.BuildGlobalStringPtr("%s", "scanf_fmt");

            builder.BuildCall2(scanfType, scanfFunc, new LLVMValueRef[] { formatStr, textPtr }, "callscanf");

            builder.BuildRet(textPtr);
        }


        public override BifyValue Call(BifyValue[] args)
        {
            if (needToInit)
            {
                var entryBlock = AssemblyCompiler.Instance.builder.InsertBlock;
                InitFunction();
                needToInit = false;
                AssemblyCompiler.Instance.builder.PositionAtEnd(entryBlock);
            }

            var printFunction = AssemblyCompiler.Instance.variableManager.GetBifyValue("explode");
            var prompt = (ConstStringValue)args[0];
            printFunction.Call([prompt]);

            return new ConstStringType().CreateByValueRef(
                AssemblyCompiler.Instance.builder.BuildCall2(TypeRef, llvmValue, args.Select(i => i.GetLLVMValue()).ToArray())
                );
        }
    }
}
