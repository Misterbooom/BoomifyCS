#define DEBUG_COMPILE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;
using BoomifyCS.Assembly;
using LLVMSharp;
using LLVMSharp.Interop;
using BoomifyCS.Exceptions;
using BoomifyCS.Assembly.BifyObject;


namespace BoomifyCS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Run(args);
        }

        static void Run(string[] args)
        {
            try
            {
                RunInterpreter();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error!");
                string errorText = e.ToString();
                new SimpleErrorWrapper(e).PrintStackTrace();
                //ProcessStartInfo psi = new ProcessStartInfo
                //{
                //    FilePath = "python",
                //    Arguments = $"C:/BoomifyCS/analyzer.py \"{errorText}\"",
                //    RedirectStandardOutput = true,
                //    UseShellExecute = false,
                //    CreateNoWindow = true
                //};

                //using (Process process = Process.Start(psi))
                //{
                //    string output = await process.StandardOutput.ReadToEndAsync();
                //    process.WaitForExit();
                //    Console.WriteLine("Error analysis:");
                //    Console.WriteLine(output);
                //}
            }
        }

        static void RunInterpreter()
        {
            Console.OutputEncoding = Encoding.Unicode;
            string file = "C:/BoomifyCS/test.bify";  
            Traceback.Instance.FilePath = file;
            string code = File.ReadAllText(file);
            MyLexer lexer = new(code);
            List<Token> tokens = lexer.Tokenize();
            tokens.WriteTokens();
            string[] codeByLine = code.Split('\n');
            AstTree astParser = new(codeByLine);
            AstNode node = astParser.ParseTokens(tokens);
            BifyDebug.Log(node.ToString());
            AssemblyCompiler compiler = AssemblyCompiler.Instance;
            //compiler.Compile(node);
        }
    }
}
