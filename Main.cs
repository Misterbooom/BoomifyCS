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
namespace BoomifyCS 
{                   
    internal class Program
    {
        static void Main(string[] args)
        {
            string code;
            using (StreamReader reader = new StreamReader("C:/BoomifyCS/test.bify"))
            {
               code = reader.ReadToEnd();

            }
            MyLexer lexer = new MyLexer(code);
            try
            {
                List<Token> tokens = lexer.Tokenize();
                tokens.WriteTokensWithoutWhiteSpace();
                string[] codeByLine = code.Split('\n');
                //for (int i = 0;i < codeByLine.Length;i++) {
                //    Console.WriteLine($"{i}:{codeByLine[i]}");  
                //}

                AstTree astParser = new AstTree(codeByLine);
                astParser.runFrom = "main";
                string[] callStack = { "Main" };

                AstNode node = astParser.ParseTokens(tokens);
                //Console.WriteLine(AstParser.SimpleEval(node));
                Console.WriteLine(node);
                Console.WriteLine(string.Join("\n",codeByLine));
                MyCompiler interpreter = new MyCompiler(codeByLine);
                interpreter.runVM(node);

                BifyInteger a = new BifyInteger(new Token(TokenType.IDENTIFIER, "1"), 1);
                BifyInteger b = new BifyInteger(new Token(TokenType.IDENTIFIER, "1"), 1);

            }
            catch (BifyException e) 
            {
                e.PrintException();
                Environment.Exit(1);
            }

            
            //astParser.ParseTokens(tokens);
        }


    }
}
