using System.Collections.Generic;
using System.Linq;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.Builtin
{
    class SafeDiv : BifyFunction
    {
        private static SafeDiv _instance;
        private readonly List<BifyType> _returnTypes = [];

        private SafeDiv(BifyType type) : base(null, null, null, null)
        {
            if (!_returnTypes.Contains(type))
            {
                ReturnType = type;
                var entryBlock = AssemblyCompiler.Instance.Builder.InsertBlock;

                Init();
                AssemblyCompiler.Instance.Builder.PositionAtEnd(entryBlock);
                FunctionArgs = new FunctionArgs(null);
                FunctionArgs.SetArguments(new Dictionary<string, BifyType> { { "lhs", ReturnType }, { "rhs", ReturnType } });
                _returnTypes.Add(type);
            }

        }

        public static SafeDiv Instance(BifyType type)
        {

            if (_instance == null)
            {
                _instance = new SafeDiv(type);
            }
            return _instance;
        }
        public void Init()
        {
            var integer = new BifyObject.IntegerType();
            TypeRef = LLVMTypeRef.CreateFunction(ReturnType.LlvmType, new LLVMTypeRef[] { ReturnType.LlvmType, ReturnType.LlvmType, integer.LlvmType });
            LlvmValue = AssemblyCompiler.Instance.Module.AddFunction("safeDiv_" + ReturnType.Name, TypeRef);
            var builder = AssemblyCompiler.Instance.Builder;
            var entry = LlvmValue.AppendBasicBlock("entry");

            BifyValue lhs = ReturnType.CreateValueRef(LlvmValue.GetParam(0));
            BifyValue rhs = ReturnType.CreateValueRef(LlvmValue.GetParam(1));

            BifyValue line = integer.CreateValueRef(LlvmValue.GetParam(2));
            builder.PositionAtEnd(entry);
            BifyValue compareToZero = rhs.Equal(ReturnType.Create(0), builder);
            var zeroBb = LlvmValue.AppendBasicBlock("zero");
            var mergeBb = LlvmValue.AppendBasicBlock("merge");
            builder.BuildCondBr(compareToZero.GetLlvmValue(), zeroBb, mergeBb);
            builder.PositionAtEnd(zeroBb);




            var errorName = new ConstStringType().Create("ZeroDivisionError");
            var errorMessage = new ConstStringType().Create(ErrorMessage.DivisionByZero());
            var file = new ConstStringType().Create(Traceback.Instance.FilePath);
            Log.RealTimeLog($"Division by zero at {Traceback.Instance.FilePath}:{line}\n");
            StdC.RaiseError(new BifyZeroDivisionError("Division by zero!"), line);
            //AssemblyCompiler.Instance.VariableManager.GetBifyValue("exit").Call([new BifyObject.IntegerType().Create(0)]);

            builder.BuildUnreachable();
            builder.PositionAtEnd(mergeBb);
            BifyValue result = lhs.Div(rhs, builder);
            builder.BuildRet(result.GetLlvmValue());





        }

        public override BifyValue Call(BifyValue[] args)
        {
            var res = AssemblyCompiler.Instance.Builder.
                BuildCall2(TypeRef, LlvmValue,
                args.Select((item, index) =>
                {
                    if (index == 2)
                    {
                        return item.GetLlvmValue();
                    }
                    return item.ExplicitCast(ReturnType, AssemblyCompiler.Instance.Builder).GetLlvmValue();
                }).ToArray(),
                "calltmp"
            );
            //var meta = AssemblyCompiler.Instance.DEBUG_COMPILEBuilder.CreateDEBUG_COMPILELocation(
            //    (uint)Traceback.Instance.Line
            //    );
            return ReturnType.CreateValueRef(res);
        }
    }
}
