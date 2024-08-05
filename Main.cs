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
        static void Main(string[] args)
        {
            runInterpreter();
            //runTests();
            //astParser.ParseTokens(tokens);
        }
        static void runTests()
        {
            ExceptionTest.CallStackTest();
        }
        static void runInterpreter()
        {
            Console.OutputEncoding = Encoding.UTF8;
            string code;
            string file = "C:/BoomifyCS/test.bify";
            using (StreamReader reader = new StreamReader(file))
            {
                code = reader.ReadToEnd();

            }
            MyLexer lexer = new MyLexer(code);

            try
            {
                List<Token> tokens = lexer.Tokenize();
                //tokens.WriteTokensWithoutWhiteSpace();
                string[] codeByLine = code.Split('\n');
                //for (int i = 0;i < codeByLine.Length;i++) {
                //    Console.WriteLine($"{i}:{codeByLine[i]}");  
                //}
                
                AstTree astParser = new AstTree(file,codeByLine);
                AstNode node = astParser.ParseTokens(tokens);
                //Console.WriteLine(AstParser.SimpleEval(node));
                Console.WriteLine(node);
                Console.WriteLine(string.Join("\n", codeByLine));
                MyCompiler interpreter = new MyCompiler(codeByLine);
                interpreter.runVM(node);

                BifyInteger a = new BifyInteger(new Token(TokenType.IDENTIFIER, "1"), 1);
                BifyInteger b = new BifyInteger(new Token(TokenType.IDENTIFIER, "1"), 1);

            }
            catch (BifyError e)
            {
                e.PrintException();
                Environment.Exit(1);
            }
        }

    }
}
