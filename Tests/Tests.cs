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
            var testResults = new Dictionary<string, Func<bool>>
                {
                    { "PascalTriangleTest", PascalTriangleTest },
                };

            Console.WriteLine("\nTest Results:");
            foreach (var result in testResults)
            {
                try
                {
                    Console.WriteLine($"{result.Key}: {(result.Value() ? "Passed" : "Failed")}");

                }
                catch (Exception e)
                {
                    Console.WriteLine($"{result.Key}: Failed");
                    Console.WriteLine($"Line: {Traceback.Instance.line}");
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
        }

        static bool PascalTriangleTest()
        {
            var code = @"
    function pascal(row, col) {
        var result = 1;
        var i = 1;
        while (i <= col) {
            result = result * (row - i + 1) / i;
            i = i + 1;
        }
        return result;
    }

    var rows = 12;
    var i = 0;
    while (i < rows) {
        var j = 0;
        var line = '';
        while (j <= i) {
            line = line + parse(pascal(i, j), 'string') + ' ';
            j = j + 1;
        }
        explode(line);
        i = i + 1;
    }
    ";
            return RunCode(code);
        }

        static bool RunCode(string code)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var lexer = new MyLexer(code);

            var tokens = lexer.Tokenize();
            var codeByLine = code.Split('\n');
            var astParser = new AstTree(codeByLine);
            var node = astParser.ParseTokens(tokens);
            var interpreter = new MyCompiler(codeByLine);
            interpreter.RunVM(node);
            return true;
        }
    }
}
