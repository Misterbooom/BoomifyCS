using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using LLVMSharp;
using LLVMSharp.Interop;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using System.Threading.Tasks;

namespace BoomifyCS.Assembly
{
    [Flags]
    enum NodeVisitFlag
    {
        NONE,
        ASSIGNMENT_INDEX = 1 << 0,
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
        public LLVMBasicBlockRef ErrorBB;

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

            // Define paths
            string libPath = Path.Combine(outputDirectory, "stdc.lib");
            string objPath = Path.Combine(outputDirectory, "stdc.o");   
            string cSourceFile = "C:\\BoomifyCS\\Assembly\\stdc.c"; 
            string exeFile = Path.Combine(outputDirectory, $"{fileName}.exe");

            try
            {
                string compileCCommand = $"clang -c {cSourceFile} -o {objPath}";
                Console.WriteLine($"C command: {compileCCommand}");
                ExecuteCommand(compileCCommand);

                string createLibCommand = $"llvm-ar rcs {libPath} {objPath}";
                ExecuteCommand(createLibCommand);

                Module.PrintToFile(filePath);
//                File.WriteAllText(filePath, @"
//; ModuleID = 'test'
//source_filename = ""test""
//target datalayout = ""e-m:w-p270:32:32-p271:32:32-p272:64:64-i64:64-i128:128-f80:128-n8:16:32:64-S128""

//@.str = private unnamed_addr constant [19 x i8] c""Starting strlen %s\00"", align 1
//@.str.1 = private unnamed_addr constant [14 x i8] c""Hello, World!\00"", align 1
//@.str.2 = private unnamed_addr constant [15 x i8] c""String len: %d\00"", align 1

//define i32 @st(ptr %str_ptr) {
//entry:
//  %l = alloca i32, align 4
//  store i32 0, ptr %l, align 4
//  %str = load ptr, ptr %str_ptr, align 8     ; Правильное чтение указателя
//  %call = call i32 (ptr, ...) @printf(ptr @.str, ptr %str)
//  %retval = load i32, ptr %l, align 4
//  ret i32 %retval
//}

//define void @main() {
//entry:
//  %str = alloca ptr, align 8                ; Выделяем память для указателя
//  store ptr @.str.1, ptr %str, align 8       ; Сохраняем адрес строки
//  %len = call i32 @st(ptr %str)             ; Передаем адрес указателя
//  %call = call i32 (ptr, ...) @printf(ptr @.str.2, i32 %len)
//  ret void
//}

//declare i32 @printf(ptr, ...)");
                Console.WriteLine($"LLVM IR written to: {filePath}");

                if (File.Exists(exeFile))
                {
                    File.Delete(exeFile);
                }

                string clangCommand = $"clang {filePath} -o {exeFile} -lmsvcrt -lkernel32 -luser32 -llegacy_stdio_definitions";
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

                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();
                    Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                    Task<string> errorTask = process.StandardError.ReadToEndAsync();

                    process.WaitForExit();

                    string output = outputTask.Result;
                    string errorOutput = errorTask.Result;

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
                    UseShellExecute = false,
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
