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
                AstTree astParser = new AstTree();
                astParser.runFrom = "main";
                string[] callStack = { "Main" };

                AstNode node = astParser.ParseTokens(tokens);
                //Console.WriteLine(AstParser.SimpleEval(node));
                Console.WriteLine(node);
                string[] codeByLine = code.Split('\n');
                Console.WriteLine(string.Join("\n",codeByLine));
                MyInterpreter interpreter = new MyInterpreter(codeByLine);
                interpreter.runVM(node);
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
