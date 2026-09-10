using System;
using System.Collections.Generic;
using System.IO;
using LLVMSharp;
using LLVMSharp.Interop;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using System.Linq;

namespace BoomifyCS.Assembly
{
    [Flags]
    internal enum NodeVisitFlag
    {
        NONE,
        ASSIGNMENT_INDEX = 1 << 0,
    }

    [Flags]
    internal enum CompilerFlags
    {
        NONE, 
        EXECUTE = 1 << 0
    }

    internal class AssemblyCompiler : IDisposable
    {
        private static AssemblyCompiler _instance;
        private static readonly object Lock = new();

        public AssemblyVariableManager VariableManager { get; }
        public LLVMContext Context { get; }
        public LLVMModuleRef Module { get; }
        public LLVMBuilderRef Builder { get; }
        public LLVMExecutionEngineRef Engine { get; }
        public LoopManager LoopManager { get; } = new();
        public NodeVisitFlag Flag { get; set; } = NodeVisitFlag.NONE;
        public BifyType ReturnType { get; set; }
        public bool IsLastNode;
        public DebugBuilder DebugBuilder { get; }
        public int StackCount => _stack.Count;
        public BifyType CurrentClass;

        public LLVMValueRef Function
        {
            get
            {
                unsafe
                {
                    return LLVM.GetBasicBlockParent(LLVM.GetInsertBlock(Builder));
                }
            }
        }

        public LLVMBasicBlockRef FunctionEntryBb { get; private set; }

        public LLVMBasicBlockRef ErrorBb;

        private readonly Stack<IValue> _stack = new();

        private AssemblyCompiler()
        {
            LLVM.InitializeAllTargetInfos();
            LLVM.InitializeAllTargets();
            LLVM.InitializeAllTargetMCs();
            LLVM.InitializeAllAsmParsers();
            LLVM.InitializeAllAsmPrinters();
            Context = new LLVMContext();
            Module = Context.Handle.CreateModuleWithName("test");
            Builder = Context.Handle.CreateBuilder();
            VariableManager = new AssemblyVariableManager();
            Engine = Module.CreateExecutionEngine();
            DebugBuilder = new DebugBuilder(Module);
        }

        public static AssemblyCompiler Instance
        {
            get
            {
                lock (Lock)
                {
                    _instance ??= new AssemblyCompiler();
                    return _instance;
                }
            }
        }

        public void PositionBeforeTerminator(LLVMBasicBlockRef block)
        {
            unsafe
            {
                if (block.Handle == IntPtr.Zero)
                {
                    throw new ArgumentException("Block handle is invalid.");
                }

                var terminator = LLVM.GetBasicBlockTerminator(block);
                if (terminator != null)
                {
                    Builder.PositionBefore(terminator);
                }
            }
        }

        public void SetFunctionEntryBb(LLVMBasicBlockRef entryBb)
        {
            FunctionEntryBb = entryBb;
            Builder.PositionAtEnd(entryBb);
        }

        public void StackPush(IValue value)
        {
            _stack.Push(value);
        }

        public void ClearStack()
        {
            _stack.Clear();
        }

        public IValue StackIValuePop()
        {
            return _stack.Pop();
        }

        public IValue StackElementAt(int index)
        {
            return _stack.ElementAt(index);
        }

        public T StackPop<T>(string errroMessage)
        {
            try
            {
                if (_stack.Count == 0)
                {
                    throw new InvalidOperationException("Stack is empty.");
                }

                T poppedValue = (T)_stack.Pop();
                if (poppedValue is not T)
                {
                    new BifyTypeError(errroMessage).Throw();
                    return default;
                }

                return poppedValue;
            }
            catch (InvalidCastException)
            {
                new BifyTypeError(errroMessage).Throw();
                return default;
            }
        }
        public T StackPeek<T>(string errroMessage)
        {
            try
            {
                if (_stack.Count == 0)
                {
                    throw new InvalidOperationException("Stack is empty.");
                }

                T peekedValue = (T)_stack.Peek();
                if (peekedValue is not T)
                {
                    new BifyTypeError(errroMessage).Throw();
                    return default;
                }

                return peekedValue;
            }
            catch (InvalidCastException)
            {
                new BifyTypeError(errroMessage).Throw();
                return default;
            }
        }

        public string StackToString()
        {
            if (_stack.Count == 0) return "Stack: [Empty]";
            return $"Stack ({_stack.Count} items):\n" + string.Join(Environment.NewLine, _stack.Select((v, i) => $"  [{i}] {v}"));
        }
        public void Visit(AstNode? node)
        {
            if (node == null)
            {
                return;
            }

            NodeHandler handler = NodeHandlerFactory.CreateHandler(node, this);
            handler.HandleNode(node);
        }

        public void Compile(AstNode node, string filePath, string outputFilePath, CompilerFlags flags)
        {
            Visit(node);
            // BifyDebug.Log($"Module:\n{Module}");
            LLVMValueRef mainFunction = Module.GetNamedFunction("main");
            if (mainFunction.Handle == IntPtr.Zero)
            {
                Console.WriteLine("Main function not found.");
                return;
            }

            string fileExtension = Path.GetExtension(outputFilePath);
            if (fileExtension == ".o")
            {
                GenerateObjectFile(filePath, outputFilePath);
            }
            else if (fileExtension == ".out" || fileExtension == "")
            {
                GenerateExecutable(filePath, outputFilePath, flags);
            }
            else if (fileExtension == ".ir")
            {
                File.WriteAllText(outputFilePath,Module.ToString());
            }
            else
            {
                Console.WriteLine($"Invalid output file extension: {fileExtension}");
            }
        }

        private void GenerateObjectFile(string filePath, string outputFilePath)
        {
            unsafe
            {
                try
                {
                    LLVMTargetMachineRef targetMachine = CreateTargetMachine();
                    targetMachine.EmitToFile(Module, outputFilePath, LLVMCodeGenFileType.LLVMObjectFile);
                    LLVM.DisposeTargetMachine(targetMachine);
                }
                catch (Exception e)
                {
                    BifyDebug.Log($"Failed to create target machine: {e.Message}");
                }

            }
        }

        // TODO: Add windows support
        private void GenerateExecutable(string filePath, string outputFilePath, CompilerFlags flags)
        {
            string tempObjFile = Path.GetTempFileName() + ".o";
            BifyDebug.Log("Generating object file:"  + tempObjFile);
            GenerateObjectFile(filePath, tempObjFile);
            BifyDebug.Log("Compiling object file: " + tempObjFile);
            LinkLibraries(tempObjFile, outputFilePath);
            
            
            if (File.Exists(tempObjFile))
            {
                File.Delete(tempObjFile);
            }

            if (flags.HasFlag(CompilerFlags.EXECUTE))
            {
                BifyDebug.Log("Running executable: " + outputFilePath);
                var processInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = outputFilePath,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new System.Diagnostics.Process { StartInfo = processInfo };

                process.Start();
    
    
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    BifyDebug.Log($"Process ended up with error. Code: {process.ExitCode}");
                }
            }
        }

        private void LinkLibraries(string objectFilePath, string outputFilePath)
        {
            string stdcPath = "/home/artur/RiderProjects/BoomifyCS/Assembly/stdc.c";
            
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "clang",
                Arguments = $"\"{objectFilePath}\" \"{stdcPath}\" -o \"{outputFilePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = System.Diagnostics.Process.Start(processInfo);
            process?.WaitForExit();

            if (process != null && process.ExitCode != 0)
            {
                string error = process.StandardError.ReadToEnd();
                throw new InvalidOperationException($"Clang compilation and linking failed:\n{error}");
            }
            Console.WriteLine("Successfully compiled.");
        }

        private unsafe LLVMTargetMachineRef CreateTargetMachine()
        {
            sbyte* targetTriple = LLVM.GetDefaultTargetTriple();
            LLVMTarget* target;
            sbyte* error = null;

            if (LLVM.GetTargetFromTriple(targetTriple, &target, &error) != 0)
            {
                string message = new string(error);
                LLVM.DisposeMessage(error);
                throw new InvalidOperationException($"Error selecting target: {message}");
            }

            LLVMTargetMachineRef targetMachine;

            fixed (byte* cpuPtr = "generic\0"u8)
            fixed (byte* featuresPtr = "\0"u8)
            {
                targetMachine = LLVM.CreateTargetMachine(
                    target,
                    targetTriple,
                    (sbyte*)cpuPtr,
                    (sbyte*)featuresPtr,
                    LLVMCodeGenOptLevel.LLVMCodeGenLevelDefault,
                    LLVMRelocMode.LLVMRelocDefault,
                    LLVMCodeModel.LLVMCodeModelDefault
                );
            }

            return targetMachine;
        }

        private bool _disposed = false;

        public void Dispose()
        {
            if (!_disposed)
            {
                Builder.Dispose();
                Module.Dispose();
                Engine.Dispose();
                _disposed = true;
            }
        }
    }
}