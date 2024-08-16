using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;
using BoomifyCS.Interpreter;
using BoomifyCS.Objects;
using BoomifyCS.Exceptions;
using BoomifyCS.Tests;
namespace BoomifyCS 
{                   
    internal class Program
    {
        static void Main()
        {
            RunInterpreter();
            //RunTests();
            //astParser.ParseTokens(tokens);
        }
        static void RunTests()
        {
            //ExceptionTest.CallStackTest();
            CodeTest codeTest = new();
            codeTest.SyntaxTest();
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
                //tokens.WriteTokens();
                string[] codeByLine = code.Split('\n');
                
                AstTree astParser = new(file,codeByLine);
                AstNode node = astParser.ParseTokens(tokens);
                Console.WriteLine(node);
                MyCompiler interpreter = new(codeByLine);
                interpreter.RunVM(node);



            }
            catch (BifyError e)
            {
                e.PrintException();
                Environment.Exit(1);
            }
            catch (Exception e)
            {

                ExceptionExtension.ParseError(e.GetType().Name, e.StackTrace, e.Message); ;
            }
        }

    }
}
