using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
                    { "CountToOneMillionTest",CountToOneMillionTest}
                };

            Console.WriteLine("\nTest Results:");

            Stopwatch totalStopwatch = new Stopwatch(); // Для общего времени
            totalStopwatch.Start();

            foreach (var result in testResults)
            {
                try
                {
                    Stopwatch testStopwatch = new Stopwatch(); // Для времени одного теста
                    testStopwatch.Start();

                    bool passed = result.Value();
                    Console.WriteLine($"{result.Key}: {(passed ? "Passed" : "Failed")}");

                    testStopwatch.Stop();
                    Console.WriteLine($"Test Execution Time: {testStopwatch.ElapsedMilliseconds} ms");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{result.Key}: Failed");
                    Console.WriteLine($"Line: {Traceback.Instance.line}");
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            totalStopwatch.Stop();
            Console.WriteLine($"\nTotal Execution Time: {totalStopwatch.ElapsedMilliseconds} ms");
        }
        static bool CountToOneMillionTest()
        {
            var code = @"
                for (var i = 0; i < 10**6;i++){}
";
            return RunCode(code);
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

            Stopwatch compilationStopwatch = new Stopwatch();
            compilationStopwatch.Start();

            var lexer = new MyLexer(code);
            var tokens = lexer.Tokenize();
            var codeByLine = code.Split('\n');
            var astParser = new AstTree(codeByLine);
            var node = astParser.ParseTokens(tokens);
            var interpreter = new MyCompiler(codeByLine);

            compilationStopwatch.Stop();
            Console.WriteLine($"Compilation Time: {compilationStopwatch.ElapsedMilliseconds} ms");

            Stopwatch executionStopwatch = new Stopwatch(); 
            executionStopwatch.Start();

            interpreter.RunVM(node);

            executionStopwatch.Stop();
            Console.WriteLine($"Execution Time: {executionStopwatch.ElapsedMilliseconds} ms");

            return true;
        }
    }
}
