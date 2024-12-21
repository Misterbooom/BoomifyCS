using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;
using BoomifyCS.Compiler;
using BoomifyCS.Objects;
using BoomifyCS.Exceptions;
namespace BoomifyCS 
{                   
    internal class Program
    {
        static void Main()
        {
            RunInterpreter();
        }
        static void RunTests()
        {
        }
        static void RunInterpreter()
        {
            Console.OutputEncoding = Encoding.UTF8;
            string code;
            string file = "C:/BoomifyCS/test.bify";
            using (StreamReader reader = new(file))
            {
                code = reader.ReadToEnd();

            }
            MyLexer lexer = new(code);

            try
            {
                List<Token> tokens = lexer.Tokenize();
                string[] codeByLine = code.Split('\n');
                AstTree astParser = new(codeByLine);
                AstNode node = astParser.ParseTokens(tokens);
                Console.WriteLine($"Final node:  \n{node.ToString()}");
                MyCompiler interpreter = new(codeByLine);
                interpreter.RunVM(node);

            }
            catch (BifyError e)
            {
                e.PrintException();

            }
            
        }

    }
}
