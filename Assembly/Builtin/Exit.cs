using System.Collections.Generic;
using System.Linq;
using BoomifyCS.Assembly.BifyObject;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.Builtin
{
    internal class Exit : BifyFunction
    {
        private bool _needToInit = true;
        public Exit() : base(null, null, null, null)
        {
            FunctionArgs = new FunctionArgs(null);
            ReturnType = new VoidType();
            IsVariadic = false;
            FunctionArgs.SetArguments(new Dictionary<string, BifyType> { { "code", new IntegerType() } });
        }
        private void InitFunction()
        {
            var builder = AssemblyCompiler.Instance.Builder;
            var module = AssemblyCompiler.Instance.Module;
            var context = module.Context;
            TypeRef = LLVMTypeRef.CreateFunction(LLVMTypeRef.Void, new LLVMTypeRef[] { LLVMTypeRef.Int32 }, false);
            LlvmValue = module.AddFunction("exit", TypeRef);
        }
        public override BifyValue Call(BifyValue[] args)
        {
            if (_needToInit)
            {
                var entryBlock = AssemblyCompiler.Instance.Builder.InsertBlock;
                InitFunction();
                AssemblyCompiler.Instance.Builder.PositionAtEnd(entryBlock);
                _needToInit = false;
            }

            var res = AssemblyCompiler.Instance.Builder.BuildCall2(TypeRef,LlvmValue, args.Select(item => item.GetLlvmValue()).ToArray());
            return ReturnType.CreateValueRef(res);
        }

    }
}
