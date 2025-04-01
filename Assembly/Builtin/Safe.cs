using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Exceptions;
using LLVMSharp;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.Builtin
{
    class SafeDiv : BifyFunction
    {
        private static SafeDiv _instance;
        private List<BifyType> returnTypes = [];

        private SafeDiv(BifyType type) : base(null, null, null, null)
        {
            if (!returnTypes.Contains(type))
            {
                ReturnType = type;
                var entryBlock = AssemblyCompiler.Instance.Builder.InsertBlock;

                Init();
                AssemblyCompiler.Instance.Builder.PositionAtEnd(entryBlock);
                FunctionArgs = new FunctionArgs(null);
                FunctionArgs.SetArguments(new Dictionary<string, BifyType> { { "lhs", ReturnType }, { "rhs", ReturnType } });
                returnTypes.Add(type);
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
            TypeRef = LLVMTypeRef.CreateFunction(ReturnType.LLVMType, new LLVMTypeRef[] { ReturnType.LLVMType, ReturnType.LLVMType,integer.LLVMType });
            llvmValue = AssemblyCompiler.Instance.Module.AddFunction("safeDiv_" + ReturnType.Name , TypeRef);
            var builder = AssemblyCompiler.Instance.Builder;
            var entry = llvmValue.AppendBasicBlock("entry");

            BifyValue lhs = ReturnType.CreateValueRef(llvmValue.GetParam(0));
            BifyValue rhs = ReturnType.CreateValueRef(llvmValue.GetParam(1));

            BifyValue line = integer.CreateValueRef(llvmValue.GetParam(2));
            builder.PositionAtEnd(entry);
            BifyValue compareToZero = rhs.Equal(ReturnType.Create(0), builder);
            BifyDebug.Log($"Compare to zero: {compareToZero};LHS:{lhs}; RHS:{rhs}");
            var zeroBB = llvmValue.AppendBasicBlock("zero");
            var mergeBB = llvmValue.AppendBasicBlock("merge");
            builder.BuildCondBr(compareToZero.GetLLVMValue(), zeroBB, mergeBB);
            builder.PositionAtEnd(zeroBB);
            var printErrorFunc = StdC.DeclarFunction("printError",
                [new ConstStringType(), new ConstStringType(), new ConstStringType(),new BifyObject.IntegerType()], new VoidType()
                
                );

            var errorName = new ConstStringType().Create("ZeroDivisionError");
            var errorMessage = new ConstStringType().Create(ErrorMessage.DivisionByZero());
            var file = new ConstStringType().Create(Traceback.Instance.FileName);


            printErrorFunc.Call([errorName,errorMessage,file,line]);
            AssemblyCompiler.Instance.VariableManager.GetBifyValue("exit").Call([new BifyObject.IntegerType().Create(0)]);
            
            builder.BuildUnreachable();
            builder.PositionAtEnd(mergeBB);
            BifyValue result = lhs.Div(rhs, builder);
            builder.BuildRet(result.GetLLVMValue());





        }

        public override BifyValue Call(BifyValue[] args)
        {
            var res = AssemblyCompiler.Instance.Builder.
                BuildCall2(TypeRef, llvmValue,
                args.Select((item, index) =>
                {
                    if (index == 2)
                    {
                        return item.GetLLVMValue();
                    }
                    return item.AutoCast(ReturnType, AssemblyCompiler.Instance.Builder).GetLLVMValue();
                }).ToArray(),
                "calltmp"
            );
            return ReturnType.CreateValueRef(res);
        }
    }
}
