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
using System.Drawing;

//class Array
//{
//    int size;
//    private int* data;
//    int capacity;
//    constructor(int capacity)
//    {
//        this.capacity = capacity;
//        this.data = malloc(sizeof(int) * capacity);
//    }
//    void add(int x)
//    {
//        if (this.size >= this.capacity)
//        {
//            int newCapacity = this.capacity * 2;
//            this.data = realloc(this.data, sizeof(int) * newCapacity);
//            this.capacity = newCapacity;
//        }
//        else
//        {
//            this.data[this.size] = x;
//            this.size++;
//        }
//    }
//    void set(int x, int i)
//    {
//        this.data[i] = x;
//    }

//}
//int main()
//{
//    var arr = new Array(10);
//    arr.add(12);
//    explode("First : %d", arr.data[0]);

//    return 0;
//}

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
                //new SimpleErrorWrapper(e).PrintStackTrace();
                throw;
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
            string file = "C:/Projects/BoomifyCS/test.bify";
            Traceback.Instance.FilePath = file;
            string code = File.ReadAllText(file);

            Stopwatch stopwatch = new();
            stopwatch.Start();
            MyLexer lexer = new(code);
            List<Token> tokens = lexer.Tokenize();
            tokens.WriteTokens();
            stopwatch.Stop();
            Console.WriteLine($"Tokenization completed in {stopwatch.ElapsedMilliseconds} ms.");

            string[] codeByLine = code.Split('\n');
            stopwatch.Restart();
            AstTree astParser = new(codeByLine);
            AstNode node = astParser.ParseTokens(tokens);
            stopwatch.Stop();
            Console.WriteLine($"AST parsing completed in {stopwatch.ElapsedMilliseconds} ms.");

            BifyDebug.Log(node.ToString());
            AssemblyCompiler compiler = AssemblyCompiler.Instance;
            compiler.Compile(node);
            //Console.Write("\x1b[38;2;255;0;0mThis is bright red\x1b[0m\n");
            //Console.Write("\x1b[38;2;0;255;0mThis is bright green\x1b[0m\n");
            //Console.Write("\x1b[38;2;0;0;255mThis is bright blue\x1b[0m\n");
            //Colorful.Console.WriteLine("This Is Red from colorful",Color.Red);

        }
    }
}
