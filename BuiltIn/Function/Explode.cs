using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly;
using BoomifyCS.Objects;
using LLVMSharp.Interop;

namespace BoomifyCS.BuiltIn.Function
{
    class Explode : BifyFunction
    {
        AssemblyCompiler compiler;
        public static new LLVMTypeRef LLVMType => LLVMTypeRef.Void;
        public Explode() : base("explode")
        {
            ExpectedArgCount = -1;
        }
        public Explode(AssemblyCompiler compiler) : base("explode")
        {
            ExpectedArgCount = -1;
            this.compiler = compiler;
        }

        public override BifyObject Call(List<BifyObject> arguments)
        {
            foreach (BifyObject bifyObject in arguments)
            {
                Console.Write($"{bifyObject} ");
            }
            Console.WriteLine();
            return new BifyNull();
        }
        public override BifyString ObjectToString()
        {
            return new BifyString("Explode() ");
        }
        public override LLVMValueRef LLVMBuild()
        {
            if (functionValue != null)
            {
                return functionValue;
            }

            // Save the current builder position
            var currentBlock = compiler.builder.InsertBlock;

            // Declare printf
            var printfType = LLVMTypeRef.CreateFunction(
                LLVMTypeRef.Int32,
                new LLVMTypeRef[] { LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0) },
                true // Variadic
            );
            var printfFunction = compiler.module.AddFunction("printf", printfType);

            // Declare the explode function
            functionType = LLVMTypeRef.CreateFunction(
                LLVMTypeRef.Void,
                new LLVMTypeRef[] {
            LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0), // Format string
            LLVMTypeRef.Int32                                // Argument (e.g., %d)
                },
                false // Not variadic
            );
            functionValue = compiler.module.AddFunction("explode", functionType);
            var entryBlock = functionValue.AppendBasicBlock("entry");
            compiler.builder.PositionAtEnd(entryBlock);

            // Build the explode function body
            var formatArg = functionValue.GetParam(0); // Format string argument
            var intArg = functionValue.GetParam(1);    // Integer argument

            compiler.builder.BuildCall2(
                printfType,
                printfFunction,
                new LLVMValueRef[] { formatArg, intArg },
                "callPrintf"
            );
            compiler.builder.BuildRetVoid();

            // Restore the original builder position
            if (currentBlock.Handle != IntPtr.Zero) // Ensure a valid insertion block exists
            {
                compiler.builder.PositionAtEnd(currentBlock);
            }

            return functionValue;
        }


    }
}
