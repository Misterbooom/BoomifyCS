using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.Builtin
{
    class StdC
    {
        public static void RaiseError(BifyError bifyError)
        {
            var printErrorFunc = StdC.DeclarFunction("printError",
            [new ConstStringType(), new ConstStringType(), new ConstStringType(), new BifyObject.IntegerType()], new VoidType()

            );
            printErrorFunc.Call([
                new ConstStringType().Create(bifyError.GetType().Name.Replace("Bify","")),
                new ConstStringType().Create(bifyError.Message),
                new ConstStringType().Create(Traceback.Instance.FilePath),
                new BifyObject.IntegerType().Create(Traceback.Instance.Line)
            ]);

        }
        public static CFunction DeclarFunction(string name, BifyType[] typeRefs, BifyType returnType)
        {
            LLVMTypeRef functionType = LLVMTypeRef.CreateFunction(returnType.LLVMType, typeRefs.Select(item => item.LLVMType).ToArray(), false);
            var func = new CFunction(name,returnType, typeRefs,functionType);
            return func;
        }
      

    }
    class CFunction : BifyFunction
    {
        private string name;
        public CFunction(string name,BifyType returnType, BifyType[] typeRefs,LLVMTypeRef functionType) : base(null, null, returnType, functionType)
        {
            var arguments = new Dictionary<string, BifyType>();
            this.name = name;
            for (int i = 0; i < typeRefs.Length; i++)
            {
                arguments.Add($"arg{i}", typeRefs[i]);
            }
            FunctionArgs = new FunctionArgs(null);
            FunctionArgs.SetArguments(arguments);
            ReturnType = returnType;
            IsVariadic = false;
        }
        public override BifyValue Call(BifyValue[] args)
        {
            var existedFunction = AssemblyCompiler.Instance.Module.GetNamedFunction(name);
            if (existedFunction == null)
            {
                llvmValue = AssemblyCompiler.Instance.Module.AddFunction(name, TypeRef);
            }
            var res = AssemblyCompiler.Instance.Builder.BuildCall2(TypeRef, llvmValue, args.Select(item => item.GetLLVMValue()).ToArray());
            return ReturnType.CreateValueRef(res);
        }
    }
}