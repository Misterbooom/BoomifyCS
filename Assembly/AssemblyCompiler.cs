using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using LLVMSharp;
using LLVMSharp.Interop;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly
{
    [Flags]
    enum NodeVisitFlag
    {
        NONE,
        DONT_LOAD_INDEX = 1 << 0,
    }

    class AssemblyCompiler : IDisposable
    {
        private static AssemblyCompiler _instance;
        private static readonly object _lock = new();

        public AssemblerCodeManager AssemblerCode { get; } = new();
        public AssemblyVariableManager VariableManager { get; }
        public LLVMContext Context { get; }
        public LLVMModuleRef Module { get; }
        public LLVMBuilderRef Builder { get; }
        public LLVMExecutionEngineRef Engine { get; }
        public LoopManager LoopManager { get; } = new();
        public AstNode NextNode { get; set; }
        public NodeVisitFlag Flag { get; set; } = NodeVisitFlag.NONE;
        public BifyType ReturnType { get; set; }

        private readonly Stack<IValue> _stack = new();

        private AssemblyCompiler()
        {
            LLVM.InitializeX86TargetMC();
            LLVM.InitializeX86Target();
            LLVM.InitializeX86TargetInfo();
            LLVM.InitializeX86AsmParser();
            LLVM.InitializeX86AsmPrinter();

            Context = new LLVMContext();
            Module = Context.Handle.CreateModuleWithName("test");
            Builder = Context.Handle.CreateBuilder();
            VariableManager = new AssemblyVariableManager();
            Engine = Module.CreateExecutionEngine();
        }

        public static AssemblyCompiler Instance
        {
            get
            {
                lock (_lock)
                {
                    _instance ??= new AssemblyCompiler();
                    return _instance;
                }
            }
        }

        public void StackPush(IValue value)
        {
            _stack.Push(value);
        }

        public BifyValue StackPop()
        {
            return (BifyValue)_stack.Pop();
        }

        public IValue StackIValuePop()
        {
            return _stack.Pop();
        }

        public void Visit(AstNode node)
        {
            NodeHandler handler = NodeHandlerFactory.CreateHandler(node, this);
            handler.HandleNode(node);
        }

        public void Compile(AstNode node)
        {
            Visit(node);
            BifyDebug.Log($"Module:\n{Module}");
            LLVMValueRef mainFunction = Module.GetNamedFunction("main");
            if (mainFunction.Handle == IntPtr.Zero)
            {
                Console.WriteLine("Main function not found.");
                return;
            }
            CompileFile("test.ll", "output");
        }

        private void CompileFile(string filePath, string outputDirectory)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            Directory.CreateDirectory(outputDirectory);
            try
            {
                Module.PrintToFile(filePath);
                Console.WriteLine($"LLVM IR written to: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing LLVM IR: {ex.Message}");
                return;
            }

            string exeFile = Path.Combine(outputDirectory, $"{fileName}.exe");

            try
            {
                if (File.Exists(exeFile))
                {
                    File.Delete(exeFile);
                }
                string clangCommand = $"clang {filePath} -o {exeFile} -nodefaultlibs -lmsvcrt -lkernel32 -luser32 -llegacy_stdio_definitions";
                ExecuteCommand(clangCommand);
                if (!File.Exists(exeFile))
                    throw new FileNotFoundException($"Executable not generated: {exeFile}");
                Console.WriteLine("Running executable...");
                RunExecutable(exeFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during compilation or execution: {ex.Message}");
            }
        }

        private static void ExecuteCommand(string command)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C {command}",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (Process process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string errorOutput = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(output))
                    {
                        Console.WriteLine(output);
                    }
                    if (!string.IsNullOrEmpty(errorOutput))
                    {
                        Console.WriteLine($"Error: {errorOutput}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing command: {ex.Message}");
            }
        }

        private static void RunExecutable(string exePath)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/K \"{exePath}\"",
                    CreateNoWindow = false,
                    UseShellExecute = true,
                };

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running executable: {ex.Message}");
            }
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
