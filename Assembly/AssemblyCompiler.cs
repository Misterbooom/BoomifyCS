using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using LLVMSharp;
using LLVMSharp.Interop;
using Microsoft.Win32;



namespace BoomifyCS.Assembly
{
    class AssemblyCompiler
    {
        private static AssemblyCompiler _instance;
        private static readonly object _lock = new();

        public AssemblerCodeManager assemblerCode = new();
        public AssemblyVariableManager variableManager;
        public LLVMContext context = new();
        public LLVMModuleRef module;
        public LLVMBuilderRef builder;
        public LLVMExecutionEngineRef engine;
        private Stack<IValue> stack = new();

        public BifyType returnType;

        private AssemblyCompiler()
        {
            LLVM.InitializeX86TargetMC();
            LLVM.InitializeX86Target();
            LLVM.InitializeX86TargetInfo();
            LLVM.InitializeX86AsmParser();
            LLVM.InitializeX86AsmPrinter();
            module = context.Handle.CreateModuleWithName("test");
            builder = context.Handle.CreateBuilder();
            variableManager = new();
            engine = module.CreateExecutionEngine();


        }
        public void StackPush(IValue value)
        {
            stack.Push(value);
        }

        public BifyValue StackPop()
        {
            return (BifyValue)stack.Pop();
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



        public void Visit(AstNode node)
        {
            NodeHandler handler = NodeHandlerFactory.CreateHandler(node, this);
            handler.HandleNode(node);
        }
        public void Compile(AstNode node)
        {
            Visit(node);
            BifyDebug.Log("\n" + module.ToString());
            CompileFile("test.ll", "output");
            var mainFunction = module.GetNamedFunction("main");
            if (mainFunction == null)
            {
                Console.WriteLine("Main function not found.");
                return;
            }

            // Execute the 'main' function
            //var result = engine.RunFunction(mainFunction, []);
        }



        private void CompileFile(string filePath, string outputDirectory)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            Directory.CreateDirectory(outputDirectory);
            // Write LLVM IR to filePath
            try
            {
                module.PrintToFile(filePath);
//                File.WriteAllText(filePath, @"
//; ModuleID = 'my_module'
//target triple = ""x86_64-pc-win32""
//target datalayout = ""e-m:w-i64:64-f80:128-n8:16:32:64-S128""

//@.scanf_fmt = private unnamed_addr constant [3 x i8] c""%d\00"", align 1
//@.printf_fmt = private unnamed_addr constant [4 x i8] c""%d\0A\00"", align 1

//declare dso_local i32 @scanf(i8*, ...)
//declare dso_local i32 @printf(i8*, ...)

//define dso_local i32 @main() {
//entry:
//  ; Allocate space for an integer variable 'num'
//  %num = alloca i32, align 4

//  ; Get pointer to the scanf format string (""%d"")
//  %fmt_sc = getelementptr inbounds [3 x i8], [3 x i8]* @.scanf_fmt, i32 0, i32 0
  
//  ; Call scanf(""%d"", &num)
//  %call_scanf = call i32 @scanf(i8* %fmt_sc, i32* %num)

//  ; Load the integer value read from input
//  %val = load i32, i32* %num, align 4

//  ; Get pointer to the printf format string (""%d\n"")
//  %fmt_pr = getelementptr inbounds [4 x i8], [4 x i8]* @.printf_fmt, i32 0, i32 0

//  ; Call printf(""%d\n"", val)
//  %call_printf = call i32 @printf(i8* %fmt_pr, i32 %val)

//  ret i32 0
//}


//");
  
            
                Console.WriteLine($"LLVM IR written to: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing LLVM IR: {ex.Message}");
                return;
            }

            string objFile = Path.Combine(outputDirectory, $"{fileName}.o");
            string exeFile = Path.Combine(outputDirectory, $"{fileName}.exe");

            try
            {
                if (File.Exists(exeFile))
                {
                    File.Delete(exeFile);
                }


                ExecuteCommand($"clang {filePath} -o {exeFile} -nodefaultlibs -lmsvcrt -lkernel32 -luser32 -llegacy_stdio_definitions ");
                if (!File.Exists(exeFile))
                    throw new FileNotFoundException($"Executable not generated: {exeFile}");

                Console.WriteLine("Running exe");
                RunExecutable(exeFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during compilation or execution: {ex.Message}");
                return;
            }
        }

        // Метод для выполнения команд в командной строке
        static void ExecuteCommand(string command)
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C {command}",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                Process process = Process.Start(processStartInfo);

                string output = process.StandardOutput.ReadToEnd();
                string errorOutput = process.StandardError.ReadToEnd();

                process.WaitForExit();

                // Выводим в консоль
                if (!string.IsNullOrEmpty(output))
                {
                    Console.WriteLine(output);
                }

                if (!string.IsNullOrEmpty(errorOutput))
                {
                    Console.WriteLine($"Error: {errorOutput}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing command: {ex.Message}");
            }
        }

        static void RunExecutable(string exePath)
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/K \"{exePath}\"",
                    CreateNoWindow = false,
                    UseShellExecute = true, 
                };

                Process process = Process.Start(processStartInfo);
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running executable: {ex.Message}");
            }
        }

    }


}
