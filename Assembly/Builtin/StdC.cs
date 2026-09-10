using System.Collections.Generic;
using System.Linq;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.Builtin
{
    internal class StdC
    {
        public static void RaiseError(BifyError bifyError, BifyValue? line = null)
        {
            var printErrorFunc = StdC.DeclarFunction("printError",
            [new ConstStringType(), new ConstStringType(), new ConstStringType(), new BifyObject.IntegerType()], new VoidType()

            );
            printErrorFunc.Call([
                new ConstStringType().Create(bifyError.GetType().Name.Replace("Bify","")),
                new ConstStringType().Create(bifyError.Message),
                new ConstStringType().Create(Traceback.Instance.FilePath),
                line ?? new BifyObject.IntegerType().Create(Traceback.Instance.Line)
            ]);

        }
        public static CFunction DeclarFunction(string name, BifyType[] typeRefs, BifyType returnType)
        {
            LLVMTypeRef functionType = LLVMTypeRef.CreateFunction(returnType.LlvmType, typeRefs.Select(item => item.LlvmType).ToArray(), false);
            var func = new CFunction(name, returnType, typeRefs, functionType);
            return func;
        }


    }

    internal class CFunction : BifyFunction
    {
        private readonly string _name;
        public CFunction(string name, BifyType returnType, BifyType[] typeRefs, LLVMTypeRef functionType) : base(null, null, returnType, functionType)
        {
            var arguments = new Dictionary<string, BifyType>();
            this._name = name;
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
            var existedFunction = AssemblyCompiler.Instance.Module.GetNamedFunction(_name);
            if (existedFunction == null)
            {
                LlvmValue = AssemblyCompiler.Instance.Module.AddFunction(_name, TypeRef);
            }
            else
            {
                LlvmValue = existedFunction;
            }
            var res = AssemblyCompiler.Instance.Builder.BuildCall2(TypeRef, LlvmValue, args.Select(item => item.GetLlvmValue()).ToArray());
            return ReturnType.CreateValueRef(res);
        }
    }
}