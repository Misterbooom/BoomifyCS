using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Compiler;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Tests
{
    class Tests
    {
        public static void RunTests()
        {
            var testResults = new Dictionary<string, bool>
            {
                { nameof(TestVariableDeclaration), TestVariableDeclaration() },
                { nameof(TestArithmeticOperations), TestArithmeticOperations() },
                { nameof(TestWhileLoop), TestWhileLoop() },
                { nameof(TestForLoop), TestForLoop() }
            };

            Console.WriteLine("\nTest Results:");
            foreach (var result in testResults)
            {
                Console.WriteLine($"{result.Key}: {(result.Value ? "Passed" : "Failed")}");
            }
        }

        static bool TestVariableDeclaration()
        {
            string code = "var x = 10;\nexplode(x);";
            return RunCode(code);
        }

        static bool TestArithmeticOperations()
        {
            string code = "var a = 5;\nvar b = 10;\nvar c = a + b;\nexplode(c);";
            return RunCode(code);
        }

        static bool TestWhileLoop()
        {
            string code = @"var i = 0;
while (i < 5) {
    explode(i);
    i = i + 1;
}";
            return RunCode(code);
        }

        static bool TestForLoop()
        {
            string code = @"for (var i = 0; i < 5; i = i + 1) {
    explode(i);
}";
            return RunCode(code);
        }

        static bool RunCode(string code)
        {
            Console.OutputEncoding = Encoding.UTF8;
            MyLexer lexer = new(code);
            try
            {
                List<Token> tokens = lexer.Tokenize();
                string[] codeByLine = code.Split('\n');
                AstTree astParser = new(codeByLine);
                AstNode node = astParser.ParseTokens(tokens);
                MyCompiler interpreter = new(codeByLine);
                interpreter.RunVM(node);
                return true;
            }
           
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return false;
            }
        }
    }
}
