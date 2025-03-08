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
        public Stack<BifyValue> stack = new();
        public BifyType returnType;

        private AssemblyCompiler()
        {
            LLVM.LinkInMCJIT();
            LLVM.InitializeX86TargetMC();
            LLVM.InitializeX86Target();
            LLVM.InitializeX86TargetInfo();
            LLVM.InitializeX86AsmParser();
            LLVM.InitializeX86AsmPrinter();
            module = context.Handle.CreateModuleWithName("test");
            builder = context.Handle.CreateBuilder();
            variableManager = new();


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
        }

    
       
        private void CompileFile(string filePath, string outputDirectory)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            Directory.CreateDirectory(outputDirectory);
            // Write LLVM IR to filePath
            try
            {
                module.PrintToFile(filePath);
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
                // Run Clang
                Console.WriteLine($"Generating object file: {objFile}");
                ExecuteCommand($"clang -c {filePath} -o {objFile}");
                if (!File.Exists(objFile)) throw new FileNotFoundException($"Object file not generated: {objFile}");

                // Run GCC
                Console.WriteLine($"Generating executable: {exeFile}");
                ExecuteCommand($"gcc -m64 {objFile} -o {exeFile} -lkernel32 -luser32 -e main");
                if (!File.Exists(exeFile)) throw new FileNotFoundException($"Executable not generated: {exeFile}");
                Console.WriteLine("Running exe");
                // Run the compiled executable
                ExecuteCommand(exeFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Compilation error: {ex.Message}");
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
                    RedirectStandardOutput = true,   // Перенаправляем стандартный вывод
                    RedirectStandardError = true     // Перенаправляем стандартную ошибку
                };

                Process process = Process.Start(processStartInfo);

                // Чтение вывода и ошибок в реальном времени
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
                    FileName = exePath,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
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
