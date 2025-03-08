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
            FunctionArgs.SetArguments(new Dictionary<string, string> { { "format", "string" } });
            ReturnType = new IntegerType();
            IsVariadic = true;
        }

        public void InitFunction()
        {
            var returnType = LLVMTypeRef.Int32;
            var argTypes = new[] { LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0) };
            TypeRef = LLVMTypeRef.CreateFunction(returnType, argTypes, true);

            llvmValue = AssemblyCompiler.Instance.module.AddFunction("printf", TypeRef);
            llvmValue.Linkage = LLVMLinkage.LLVMExternalLinkage;
        }

        public override BifyValue Call(BifyValue[] args)
        {
            if (needToInit)
            {
                InitFunction();
                needToInit = false;
            }


            var formatString = (StringValue)args[0];
            var llvmArgs = new LLVMValueRef[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].GetLLVMValue().TypeOf == LLVMTypeRef.Float)
                {
                    llvmArgs[i] = AssemblyCompiler.Instance.builder.BuildFPExt(args[i].GetLLVMValue(),
                        LLVMTypeRef.Double,"float_to_double");
                }
                else
                {
                    llvmArgs[i] = args[i].GetLLVMValue();

                }
            }

            AssemblyCompiler.Instance.builder.BuildCall2(
                TypeRef,
                llvmValue,
                llvmArgs,
                    "printfCall"
                );

            return new IntegerType().Create(0);
        }
    }
}