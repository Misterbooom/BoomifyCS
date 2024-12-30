using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using Microsoft.Win32;

namespace BoomifyCS.Assembly
{
    class AssemblyCompiler
    {
        public AssemblerCodeManager assemblerCode = new();
        public AssemblyVariableManager variableManager = new();
        public string LastLoadedConstant = "";
        public string LastUsedRegister = "";
        public bool ReturnStatement = false;
        private List<string> BusyRegisters = [];
        private List<string> registers64bit = new List<string> { "rax", "rbx", "rcx", "rdx" };

        public void Compile(AstNode node)
        {
            Visit(node);

            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "output");  // Абсолютный путь для выходной папки
            string asmFilePath = Path.Combine(outputPath, "generated.asm");

            Directory.CreateDirectory(outputPath);

            File.WriteAllText(asmFilePath, assemblerCode.GetCode());

            CompileAsmFile(asmFilePath, outputPath);
        }
        public void Visit(AstNode node)
        {
            NodeHandler handler = NodeHandlerFactory.CreateHandler(node, this);
            handler.HandleNode(node);
        }
        public string AllocateRegister64bit()
        {
            foreach (string reg in registers64bit)
            {
                if (!BusyRegisters.Contains(reg))
                {
                    BusyRegisters.Add(reg);
                    LastUsedRegister = reg;
                    return reg;
                }
            }
            throw new Exception("No free registers available!");
        }

        public void FreeRegister(string reg)
        {
            BusyRegisters.Remove(reg);
        }
        static void CompileAsmFile(string filePath, string outputDirectory)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            Directory.CreateDirectory(outputDirectory);

            string objFile = Path.Combine(outputDirectory, $"{fileName}.obj");
            string nasmCommand = $"nasm -f win64 {filePath} -o {objFile}";
            Console.WriteLine($"Running NASM command: {nasmCommand}");
            ExecuteCommand(nasmCommand);

            string exeFile = Path.Combine(outputDirectory, $"{fileName}.exe");
            string gccCommand = $"gcc -m64 {objFile} -o {exeFile} -lkernel32 -luser32 -e main";
            Console.WriteLine($"Running GCC command: {gccCommand}");
            ExecuteCommand(gccCommand);


            if (File.Exists(exeFile))
            {
                Console.WriteLine($"Compilation successful. Output file: {exeFile}");
                RunExecutable(exeFile);
            }
            else
            {
                Console.WriteLine($"Error: {exeFile} not found after compilation.");
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

        // Метод для запуска скомпилированного .exe файла
        static void RunExecutable(string exePath)
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    CreateNoWindow = false,
                    UseShellExecute = true
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
