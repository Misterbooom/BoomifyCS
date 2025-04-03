using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.Builtin
{
    class Explode : BifyFunction
    {
        private bool needToInit = true;

        public Explode() : base(null, null, null, null)
        {
            FunctionArgs = new FunctionArgs(null);
            FunctionArgs.SetArguments(new Dictionary<string, BifyType> { { "format", new BifyPointerType(new CharType()) } });

            ReturnType = new IntegerType();
            IsVariadic = true;
        }

        public void InitFunction()
        {
            var returnType = LLVMTypeRef.Int32;
            var argTypes = new[] { LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0) };
            TypeRef = LLVMTypeRef.CreateFunction(returnType, argTypes, true);

            llvmValue = AssemblyCompiler.Instance.Module.AddFunction("printf", TypeRef);
        }

        public override BifyValue Call(BifyValue[] args)
        {
            if (needToInit)
            {
                InitFunction();
                needToInit = false;
            }


            var formatString = (PointerValue)args[0];
            var llvmArgs = new LLVMValueRef[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].CompareType(typeof(FloatType)))
                {
                    unsafe
                    {
                        llvmArgs[i] = AssemblyCompiler.Instance.Builder.BuildFPExt(args[i].GetLLVMValue(),
                        LLVM.DoubleType(), "float_to_double");
                    }
                }
                else if (args[i].GetBifyType().CompareType(new BoolType()))
                {
                    llvmArgs[i] = AssemblyCompiler.Instance.Builder.BuildZExt(args[i].GetLLVMValue(),LLVMTypeRef.Int32,"int1_to_int32");
                }
                else
                {
                    llvmArgs[i] = args[i].GetLLVMValue();
                    BifyDebug.Log($"explode arg {i}:{args[i].GetBifyType()} {llvmArgs[i]}");


                }

            }

            AssemblyCompiler.Instance.Builder.BuildCall2(
                TypeRef,
                llvmValue,
                llvmArgs,
                    "printfCall"
                );

            return new IntegerType().Create(0);
        }
    }
}